using CyberCity.Models.AirportModels;
using CyberCity.Models.HouseModels;
using CyberCity.Models.MunicipalityModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace CyberCity.Models.BankModel
{
    /// <summary>
    /// Банк
    /// </summary>
    public class Bank : CityObject
    {
        /// <summary>
        /// Курск продажи доллара банком
        /// </summary>
        public double CourseSell { get; set; }

        /// <summary>
        /// Курс покупки доллара пользователем
        /// </summary>
        public double CourseBuy { get; set; }

        public Bank(DataBus bus) : base(bus)
        {
            UpdateCourses();
        }

        public override void ProcessPackage(Package package)
        {
            if (package.Method == Houses.SendMetricsMethod)
            {
                List<HouseMeter> housesMeters = JsonConvert.DeserializeObject<List<HouseMeter>>(package.Params);

                foreach (var houseMeters in housesMeters)
                {
                    EnterPackage enterPackage = new EnterPackage
                    {
                        Recipient = new Resident
                        {
                           Home = houseMeters.IdHome,
                        },
                        Sender = new Resident
                        {
                            Surname = "ЖКХ",
                            Name = "-",
                            Patronymic = "-",

                        },
                        Summa = houseMeters.Summa
                    };
                    MakeTransfer(enterPackage);
                }
                
            }
            else if(package.Method == Airport.CanFlyMethod)
            {

            }
            else if (package.Method == Airport.AirportInvoiceMethod)
            {

            }
            // если деньги идут от муниципалитета (значит перечисляют зарплату)
            else if (package.Method == Municipality.PaySalaryMethod)
            {

            }
        }

        public void MakeTransfer(EnterPackage needTransfer)
        {
            UpdateCourses();

            if (needTransfer.Sender.Surname == "Муниципалитет")
            {
                Resident sender = GetResidentByFIO(needTransfer.Sender);// отправитель денег муниципалитет
                Resident recipient = GetResidentByFIO(needTransfer.Recipient);// получатель денег

                MakeTransferForGetPackageByMunitsupalitet(sender, recipient, needTransfer.Summa);

                return;
            }
            if (needTransfer.Recipient.Surname == "ЖКХ")// если получатель денег ЖКХ (значит необходимо перечислить деньги за ком. услуги)
            {
                int numberOfHouse = needTransfer.Sender.Home;
                List<Resident> ResidentForPay = new List<Resident>();
                var allResident = _context.Residents;
                Resident jhk = new Resident();

                foreach (var person in allResident)// среди всех зарегистрированных клиентов ищем жителей необходимого дома
                {
                    if (person.Home == numberOfHouse)
                        ResidentForPay.Add(person);
                    if (person.Surname == "ЖКХ")
                        jhk = person;
                }

                int countOfResidentForPay = ResidentForPay.Count;// находим количество жителей данного дома
                double summaForPay = needTransfer.Summa / countOfResidentForPay;// сумма платежа ЖКХ делится на  всех жителей дома

                foreach (var person in ResidentForPay)// с каждого жителя списываем необходимую сумму
                {
                    MakeTransferForGetPackage(person, jhk, summaForPay, CourseBuy);
                }

                return;
            }
            else   // если этот пакет не для перечисления з/п и уплаты ком. услуг 
            {
                Resident sender = new Resident();
                sender = GetResidentByFIO(needTransfer.Sender);// определяем отправителя денег 
                Resident recipient = new Resident();
                recipient = GetResidentByFIO(needTransfer.Recipient);// определяетм получателя денег

                MakeTransferForGetPackage(sender, recipient, needTransfer.Summa, CourseBuy);// выполняем перевод
            }
        }

        public bool MakeTransferForGetPackageByMunitsupalitet(Resident sender, Resident recipient, double summa)
        {
            sender.Money = sender.Money - summa;  // счёт отправителя (муниципалитета) уменьшается на сумму заработной платы работника
            _context.SaveChanges();

            if (recipient.Debt > 0)  // если у получателя есть задолженность
            {
                if (recipient.Debt < summa)  // если задолженность меньше перечисляемой зарплаты
                {
                    summa = summa - recipient.Debt;   // зарплата уменьшается на сумму задолженности
                    recipient.Debt = 0;   // задолженность списывается
                    recipient.Money = recipient.Money + summa;   // оставшаяся сумма перечисляется на рублёвый счёт клиента
                    _context.SaveChanges();
                }
                else    // если задолженность больше или равна зарплате
                {
                    recipient.Debt = recipient.Debt - summa;
                    _context.SaveChanges();
                }
            }
            else   // если у получателя нет задолженности
            {
                recipient.Money = recipient.Money + summa;
                _context.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// Получения курса валют с сайта Центрального Банка России
        /// </summary>
        private double GetCourseFromCBR()
        {
            var day = DateTime.Now.ToString("dd");
            var month = DateTime.Now.ToString("MM");

            string slka = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + day + "." + month + "." + DateTime.Now.Year;

            WebRequest request = WebRequest.Create(slka);
            WebResponse response = request.GetResponse();
            string needToWork = "";
            var answer = string.Empty;
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        needToWork = needToWork + line;
                    }
                    answer = reader.ReadToEnd();
                }
            }
            response.Close();
            needToWork = needToWork.Substring(1722);
            int length = needToWork.Length - 5;

            // Чтобы корректно распарсить число заменяет "," на "."
            //https://stackoverflow.com/questions/11560465/parse-strings-to-double-with-comma-and-point
            needToWork = needToWork.Substring(0, needToWork.Length - length).Replace(',', '.');
            double course = double.Parse(needToWork, CultureInfo.InvariantCulture);

            return course;
        }

        private (double courseSell, double courseBuy) GetCourse()
        {
            Random rnd = new Random();
            int difference = rnd.Next(0, 5);
            int mn = rnd.Next(0, 9);

            double courseSell = GetCourseFromCBR();
            double courseBuy = courseSell - Convert.ToDouble(difference) - Convert.ToDouble((0.12) * mn);

            return (courseSell, courseBuy);
        }

        private void UpdateCourses()
        {
            var courses = GetCourse();

            CourseSell = courses.courseSell;
            CourseBuy = courses.courseBuy;
        }

        public Resident GetResidentByFIO(Resident getResident)
        {
            Resident findResident = new Resident();
            var resident = _context.Residents;
            foreach (var person in resident)
            {
                if ((person.Name == getResident.Name) && (person.Surname == getResident.Surname) && (person.Patronymic == getResident.Patronymic))
                    findResident = person;
            }
            return null;
        }

        public bool MakeTransferForGetPackage(Resident sender, Resident recipient, double summa, double courseBuy)
        {
            if (sender == null || recipient == null) return false;

            //    recipient.Money = recipient.Money + summa;  // счёт получателя увеличивается на сумму операции

            if (sender.Money >= summa)    //  если у отправителя хватает суммы в рублях
            {
                sender.Money = sender.Money - summa;    // счёт в рублях уменьшается на сумму операции
                recipient.Money = recipient.Money + summa;   // счёт получателя увеличивается на сумму операции
                _context.SaveChanges();
            }
            else    // если у отправителя не хватает денег в рублях для данной операции
            {
                if (sender.Money > 0) // если у пользователя есть какая-то сумма в рублях
                {
                    recipient.Money = recipient.Money + sender.Money;   // получатель на свой счёт в раблях получает всю сумму с рублёвого счёта отправителя
                    summa = summa - sender.Money;   // сумма операции уменьшается на сумму, уже отправленную получателю
                    sender.Money = 0;     // все деньги в рублях на счету отправителя списываются
                    _context.SaveChanges();
                }
                if (Math.Round(sender.MoneyInCourse * courseBuy) >= 0)  // если у отправителя имеется сумма в валюте 
                {
                    sender.MoneyInCourse = sender.MoneyInCourse - Math.Round(summa / courseBuy);    // сумма валюты на счету отправителя уменьшается на сумма операции (оставшаяся) в рублях / курс валюты;
                    recipient.MoneyInCourse = recipient.MoneyInCourse + Math.Round(summa / courseBuy);  // сумма валюты на счету получателя увеличивается на сумма операции (оставшаяся) в рублях / курс валюты;
                    summa = summa - Math.Round(summa / courseBuy); // сумма операции уменьшается на сумму переведённую в валюте
                    _context.SaveChanges();
                }
                sender.Debt = sender.Debt + summa; // оставшаяся сумма операции записывается в долг отправителя
                recipient.Money = recipient.Money + summa; // на  счёт получателя начисляется оставшаяся сумма в рублях
                _context.SaveChanges();
            }
            return true;
        }
    }
}
