using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using CyberCity.Models;
using CyberCity.Models.BankModel;
using CyberCity.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controllers
{
    [Authorize(Roles = "Bank")]
    public class BankController: BaseController
    {
        public BankController(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получения курса валют
        /// </summary>
        public double GetCourse()
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

        public bool MakeNeedOperation()
        {
            var clients = _context.Residents;
            var needOperations = _context.NeedOprerations;

            Resident sender = new Resident();
            Resident recipient = new Resident();
            List<Resident> ClientsOfBank = new List<Resident>();
            foreach (var person in clients)
            {
                ClientsOfBank.Add(person);
            }
            DateTime currentTime = DateTime.Now;

            foreach (var operation in needOperations)
            {
                if (currentTime >= operation.Time)
                {
                    sender = GetResidentFromListById(ClientsOfBank, operation.Sender);
                    recipient = GetResidentFromListById(ClientsOfBank, operation.Recipient);
                    sender.Money = sender.Money - operation.Money;
                    recipient.Money = recipient.Money + operation.Money;

                    _context.NeedOprerations.Remove(operation);
                }
            }
            _context.SaveChanges();
            return true;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Банк";
            MakeNeedOperation();
            return View();
        }

        public Resident GetResidentFromListById(List<Resident> ClientsOfBank, int findId)
        {
            foreach (var person in ClientsOfBank)
            {
                if (person.Id == findId)
                    return person;
            }
            return null;
        }

        public Resident GetResidentById(int findId)
        {
            var clients = _context.Residents;
            foreach (var person in clients)
            {
                if (person.Id == findId)
                    return person;
            }
            return null;
        }

        [HttpGet]
        public ActionResult Enter()
        {
            ViewBag.Title = "Вход";
            MakeNeedOperation();
            double Course = GetCourse();
            TempData["CourseSel"] = Course;
            
            Random rnd = new Random();

            int difference = rnd.Next(0, 5);
            int mn = rnd.Next(0, 9);

            TempData["Month"] = DateUtils.GetMonth(DateTime.Now.Month);
            TempData["CourseBuy"] = Course - Convert.ToDouble(difference) - Convert.ToDouble((0.12) * mn);

            TempData.Keep();

            return View();
        }

        [HttpGet]
        public ActionResult EnterToSystem()
        {
            ViewBag.Title = "Вход в систему";
            MakeNeedOperation();
            return View();
        }

        [HttpPost]
        public ActionResult EnterToSystem(Resident resident)
        {
            var clients = _context.Residents;
            MakeNeedOperation();

            foreach (var person in clients)
            {
                if ((person.Login == resident.Login) && (person.Password == resident.Password))
                {
                    TempData["Id"] = person.Id;
                    TempData["Name"] = person.Name;
                    TempData["Surname"] = person.Surname;
                    TempData["Patronymic"] = person.Patronymic;
                    TempData["Login"] = person.Login;
                    TempData["Password"] = person.Password;
                    TempData["Money"] = person.Money;
                    TempData["MoneyInCourse"] = person.MoneyInCourse;

                    return Redirect("/Bank/ClientPage");
                }
            }
            return Redirect("/Bank/DoubleEnterToSystem");
        }

        [HttpGet]
        public ActionResult DoubleEnterToSystem()
        {
            ViewBag.Title = "Повторный вход в систему";
            MakeNeedOperation();
            return View();
        }

        [HttpPost]
        public ActionResult DoubleEnterToSystem(Resident resident)
        {
            var clients = _context.Residents;
            MakeNeedOperation();
            foreach (var person in clients)
            {
                if ((person.Login == resident.Login) && (person.Password == resident.Password))
                {
                    TempData["Id"] = person.Id;
                    TempData["Name"] = person.Name;
                    TempData["Surname"] = person.Surname;
                    TempData["Patronymic"] = person.Patronymic;
                    TempData["Login"] = person.Login;
                    TempData["Password"] = person.Password;
                    TempData["Money"] = person.Money;
                    TempData["MoneyInCourse"] = person.MoneyInCourse;
                    return Redirect("/Bank/ClientPage");
                }
            }
            return Redirect("/Bank/DoubleEnterToSystem");
        }

        [HttpGet]
        public ActionResult Registration()
        {
            ViewBag.Title = "Регистрация в системе";
            MakeNeedOperation();
            return View();
        }

        [HttpPost]
        public string Registration(Resident resident)
        {

            var clients = _context.Residents;
            MakeNeedOperation();
            foreach (var person in clients)
            {
                if (person.Login == resident.Login)
                    return "Данный логин уже занят";
            }
            _context.Residents.Add(resident);
            _context.SaveChanges();
            ViewBag.Residents = clients;
            return "Клиент " + resident.Surname + " " + resident.Name + " успешно зарегистрирован";
        }

        [HttpGet]
        public ActionResult ClientPage()
        {
            MakeNeedOperation();
            //            ViewBag.Title = "Личный кабинет";
            if (TempData["Possible"] != null)
            {
                ViewBag.Possible = TempData["Possible"];
                TempData["Possible"] = ViewBag.Possible;
            }
            else TempData["Possible"] = "";
            ViewBag.Id = TempData["Id"];
            var clients = _context.Residents;
            double courseSel = Convert.ToDouble(TempData["CourseSel"]);
            double courseBuy = Convert.ToDouble(TempData["CourseBuy"]);
            string month = Convert.ToString(TempData["Month"]);
            TempData["CourseSel"] = courseSel;
            TempData["CourseBuy"] = courseBuy;
            TempData["Month"] = month;
            //  TempData["CourseSel"] = 61.32;//////////////////////////////
            //TempData["CourseBuy"] = 59.17;//////////////////////////////
            TempData.Keep();
            foreach (var person in clients)
            {
                if (person.Id == ViewBag.Id)
                {
                    TempData["Id"] = person.Id;
                    TempData["Name"] = person.Name;
                    TempData["Surname"] = person.Surname;
                    TempData["Patronymic"] = person.Patronymic;
                    TempData["Login"] = person.Login;
                    TempData["Password"] = person.Password;
                    TempData["Money"] = person.Money;
                    TempData["MoneyInCourse"] = person.MoneyInCourse;
                    TempData.Keep();
                    return View();
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult PersonalData()
        {
            MakeNeedOperation();
            ViewBag.Title = "Персональные данные";
            ViewBag.Id = TempData["Id"];
            TempData["Possible"] = "";
            var clients = _context.Residents;
            foreach (var person in clients)
            {
                if (person.Id == ViewBag.Id)
                {
                    TempData["Id"] = person.Id;
                    TempData["Name"] = person.Name;
                    TempData["Surname"] = person.Surname;
                    TempData["Patronymic"] = person.Patronymic;
                    TempData["Login"] = person.Login;
                    TempData["Password"] = person.Password;
                    TempData["Money"] = person.Money;
                    TempData["MoneyInCourse"] = person.MoneyInCourse;
                    TempData.Keep();
                    return View();
                }
            }
            return View();
        }


        [HttpGet]
        public ActionResult MoneyContribution()
        {
            MakeNeedOperation();
            ViewBag.Title = "Внести вклад";
            ViewBag.Id = TempData["Id"];
            var clients = _context.Residents;
            foreach (var person in clients)
            {
                if (person.Id == ViewBag.Id)
                {
                    if (person.Money == 0)
                    {
                        TempData["Possible"] = "Данная операция недоступна, так как на Вашем счёте недостаточно средств";
                        return Redirect("/Bank/ClientPage");
                    }
                    TempData["Id"] = person.Id;
                    TempData["Name"] = person.Name;
                    TempData["Surname"] = person.Surname;
                    TempData["Patronymic"] = person.Patronymic;
                    TempData["Login"] = person.Login;
                    TempData["Password"] = person.Password;
                    TempData["Money"] = person.Money;
                    TempData["MoneyInCourse"] = person.MoneyInCourse;
                    TempData.Keep();
                    return View();
                }
            }
            return View();
        }


        [HttpPost]
        public String MoneyContribution(MoneyContribution moneyContribution)
        {
            MakeNeedOperation();
            ViewBag.Title = "Сделать вклад";
            ViewBag.Id = TempData["Id"];
            var clients = _context.Residents;
            Resident currentResident = new Resident();
            Resident bank = new Resident();

            foreach (var person in clients)
                if (person.Surname == "Банк")
                    bank = person;

            foreach (var person in clients)
            {
                if (person.Id == ViewBag.Id)
                {
                    TempData["Id"] = person.Id;
                    TempData["Name"] = person.Name;
                    TempData["Surname"] = person.Surname;
                    TempData["Patronymic"] = person.Patronymic;
                    TempData["Login"] = person.Login;
                    TempData["Password"] = person.Password;
                    TempData["Money"] = person.Money;
                    TempData["MoneyInCourse"] = person.MoneyInCourse;
                    TempData.Keep();
                    currentResident = person;
                }
            }
            if (moneyContribution.Summa == 0)
                return "Сумма вклада не может соствлять 0 рублей";
            if (moneyContribution.Summa > currentResident.Money)
                return "Вы не располагаете такой суммой";
            if (moneyContribution.Duration > 31)
                return "Вы указали слишком большой срок вклада";
            double summaForGetClient = Math.Round(moneyContribution.Summa + moneyContribution.Summa * moneyContribution.Duration * 7 / 36500, 2);
            if (summaForGetClient == moneyContribution.Summa)
                return "Данная операция не имеет смысла - Вы получите менее 1 копейки";
            _context.MoneyContributions.Add(moneyContribution);
            currentResident.Money = currentResident.Money - moneyContribution.Summa;
            bank.Money = bank.Money + moneyContribution.Summa;
            NeedOperation newOperation = new NeedOperation();
            newOperation.Id = 0;
            newOperation.Sender = 1;
            newOperation.Recipient = currentResident.Id;
            newOperation.Money = summaForGetClient;
            int currentTime = moneyContribution.Duration * 24;
            int currentHours = currentTime / 60;
            int currentMinutes = currentTime - currentHours * 60;
            newOperation.Time = DateTime.Now.AddHours(currentHours).AddMinutes(currentMinutes);
            _context.NeedOprerations.Add(newOperation);
            _context.SaveChanges();
            return "Ваш вклад составил " + moneyContribution.Summa + " рублей. Через " + moneyContribution.Duration + " дней Вы получите " + Convert.ToString(summaForGetClient) + " рублей.";
        }

        [HttpGet]
        public ActionResult Credit()
        {
            MakeNeedOperation();
            ViewBag.Title = "Взять кредит";
            ViewBag.Id = TempData["Id"];
            var clients = _context.Residents;
            foreach (var person in clients)
            {
                if (person.Id == ViewBag.Id)
                {
                    if (person.Money == 0)
                    {
                        TempData["Possible"] = "Данная операция недоступна, так как на Вашем счёте недостаточно средств";
                        return Redirect("/Bank/ClientPage");
                    }
                    TempData["Id"] = person.Id;
                    TempData["Name"] = person.Name;
                    TempData["Surname"] = person.Surname;
                    TempData["Patronymic"] = person.Patronymic;
                    TempData["Login"] = person.Login;
                    TempData["Password"] = person.Password;
                    TempData["Money"] = person.Money;
                    TempData["MoneyInCourse"] = person.MoneyInCourse;
                    TempData.Keep();
                    return View();
                }
            }
            return View();
        }


        [HttpPost]
        public String Credit(Credit credit)
        {
            MakeNeedOperation();
            ViewBag.Title = "Взять кредит";
            ViewBag.Id = TempData["Id"];
            var clients = _context.Residents;
            Resident currentResident = new Resident();
            Resident bank = new Resident();

            foreach (var person in clients)
                if (person.Surname == "Банк")
                    bank = person;

            foreach (var person in clients)
            {
                if (person.Id == ViewBag.Id)
                {
                    TempData["Id"] = person.Id;
                    TempData["Name"] = person.Name;
                    TempData["Surname"] = person.Surname;
                    TempData["Patronymic"] = person.Patronymic;
                    TempData["Login"] = person.Login;
                    TempData["Password"] = person.Password;
                    TempData["Money"] = person.Money;
                    TempData["MoneyInCourse"] = person.MoneyInCourse;
                    TempData.Keep();
                    currentResident = person;
                }
            }

            if (credit.Summa == 0)
                return "Сумма кредита не может соствлять 0 рублей";

            double summaForGetBank = Math.Round(credit.Summa + credit.Summa * credit.Duration * 13 / 36500, 2);
            if (summaForGetBank == credit.Summa)
                return "Данная операция не имеет смысла - Банк получит ни 1 копейки";
            _context.Credits.Add(credit);
            currentResident.Money = currentResident.Money + credit.Summa;
            bank.Money = bank.Money - credit.Summa;
            NeedOperation newOperation = new NeedOperation();
            newOperation.Id = 0;
            newOperation.Sender = currentResident.Id;
            newOperation.Recipient = 1;
            newOperation.Money = summaForGetBank;
            int currentTime = credit.Duration * 24;
            int currentHours = currentTime / 60;
            int currentMinutes = currentTime - currentHours * 60;
            newOperation.Time = DateTime.Now.AddHours(currentHours).AddMinutes(currentMinutes);
            _context.NeedOprerations.Add(newOperation);
            _context.SaveChanges();
            return "Вы получили кредит суммой " + credit.Summa + " рублей. Через " + credit.Duration + " дней Вы должны вернуть банку " + Convert.ToString(summaForGetBank) + " рублей.";
        }


        [HttpGet]
        public ActionResult MoneyTransfer()
        {
            MakeNeedOperation();
            ViewBag.Id = TempData["Id"];
            var clients = _context.Residents;
            ViewBag.Residents = clients;
            foreach (var person in clients)
            {
                if (person.Id == ViewBag.Id)
                {
                    if (person.Money == 0)
                    {
                        TempData["Possible"] = "Данная операция недоступна, так как на Вашем счёте недостаточно средств";
                        return Redirect("/Bank/ClientPage");
                    }
                    TempData["Id"] = person.Id;
                    TempData["Name"] = person.Name;
                    TempData["Surname"] = person.Surname;
                    TempData["Patronymic"] = person.Patronymic;
                    TempData["Login"] = person.Login;
                    TempData["Password"] = person.Password;
                    TempData["Money"] = person.Money;
                    TempData["MoneyInCourse"] = person.MoneyInCourse;
                    TempData.Keep();
                    ViewBag.Title = "Выполнить перевод";
                    return View();
                }
            }
            ViewBag.Title = "Выполнить перевод";

            return View();
        }


        [HttpPost]
        public String MoneyTransfer(Moneytransfer moneytransfer)
        {
            MakeNeedOperation();
            ViewBag.Title = "Взять кредит";
            ViewBag.Id = TempData["Id"];
            var clients = _context.Residents;
            Resident sender = new Resident();
            Resident recipient = new Resident();

            foreach (var person in clients)
            {
                if (person.Id == moneytransfer.Sender)
                    sender = person;
                if (person.Id == moneytransfer.Recipient)
                    recipient = person;
            }

            foreach (var person in clients)
            {
                if (person.Id == ViewBag.Id)
                {
                    TempData["Id"] = person.Id;
                    TempData["Name"] = person.Name;
                    TempData["Surname"] = person.Surname;
                    TempData["Patronymic"] = person.Patronymic;
                    TempData["Login"] = person.Login;
                    TempData["Password"] = person.Password;
                    TempData["Money"] = person.Money;
                    TempData["MoneyInCourse"] = person.MoneyInCourse;
                    TempData.Keep();
                }
            }

            if (moneytransfer.Summa == 0)
                return "Сумма перевода не может соствлять 0 рублей";
            if (moneytransfer.Summa > sender.Money)
                return "Вы не располагаете такой суммой";

            sender.Money = sender.Money - moneytransfer.Summa;
            recipient.Money = recipient.Money + moneytransfer.Summa;

            _context.Moneytransfers.Add(moneytransfer);
            _context.SaveChanges();
            return "Перевод суммой " + moneytransfer.Summa + " рублей успешно выполнен! " + recipient.Surname + " " + recipient.Name + " " + recipient.Patronymic + " получила данную сумму на свой счёт.";
        }

        [HttpGet]
        public ActionResult SellValute()
        {
            MakeNeedOperation();
            ViewBag.Id = TempData["Id"];
            var clients = _context.Residents;
            ViewBag.Residents = clients;
            foreach (var person in clients)
            {
                if (person.Id == ViewBag.Id)
                {
                    if (person.MoneyInCourse == 0)
                    {
                        TempData["Possible"] = "Данная операция недоступна, так как на Вашем счёте недостаточно средств";
                        return Redirect("/Bank/ClientPage");
                    }
                    TempData["Id"] = person.Id;
                    TempData["Name"] = person.Name;
                    TempData["Surname"] = person.Surname;
                    TempData["Patronymic"] = person.Patronymic;
                    TempData["Login"] = person.Login;
                    TempData["Password"] = person.Password;
                    TempData["Money"] = person.Money;
                    TempData["MoneyInCourse"] = person.MoneyInCourse;
                    TempData.Keep();
                    ViewBag.Title = "Продать валюту";
                    return View();
                }
            }
            ViewBag.Title = "Продать валюту";

            return View();
        }

        [HttpPost]
        public String SellValute(ValuteOpereation operation)
        {
            MakeNeedOperation();
            double sumOfValute = operation.Summa;
            ViewBag.Title = "Сделать вклад";
            ViewBag.Id = TempData["Id"];
            var clients = _context.Residents;
            Resident currentResident = new Resident();
            Resident bank = new Resident();

            double courseSel = Convert.ToDouble(TempData["CourseSel"]);
            double courseBuy = Convert.ToDouble(TempData["CourseBuy"]);
            string month = Convert.ToString(TempData["Month"]);
            TempData["CourseSel"] = courseSel;
            TempData["CourseBuy"] = courseBuy;
            TempData["Month"] = month;
            TempData.Keep();

            foreach (var person in clients)
                if (person.Surname == "Банк")
                    bank = person;

            foreach (var person in clients)
            {
                if (person.Id == ViewBag.Id)
                {
                    TempData["Id"] = person.Id;
                    TempData["Name"] = person.Name;
                    TempData["Surname"] = person.Surname;
                    TempData["Patronymic"] = person.Patronymic;
                    TempData["Login"] = person.Login;
                    TempData["Password"] = person.Password;
                    TempData["Money"] = person.Money;
                    TempData["MoneyInCourse"] = person.MoneyInCourse;
                    TempData.Keep();
                    currentResident = person;
                }
            }
            if (sumOfValute == 0)
                return "Сумма валюты для продажи не может соствлять 0 единиц";
            if (sumOfValute > currentResident.Money)
                return "Вы не располагаете такой суммой";
            double getRubles = Math.Round(sumOfValute * courseBuy, 2);
            currentResident.MoneyInCourse = currentResident.MoneyInCourse - sumOfValute;
            bank.MoneyInCourse = bank.MoneyInCourse + sumOfValute;
            currentResident.Money = currentResident.Money + getRubles;
            bank.Money = bank.Money - getRubles;
            _context.SaveChanges();
            return "Операция успешно выполнена! Вы продали " + Convert.ToString(sumOfValute) + " долларов за " + Convert.ToString(getRubles) + " рублей.";
        }

        [HttpGet]
        public ActionResult BuyValute()
        {
            MakeNeedOperation();
            ViewBag.Id = TempData["Id"];
            var clients = _context.Residents;
            ViewBag.Residents = clients;

            double courseSel = Convert.ToDouble(TempData["CourseSel"]);
            double courseBuy = Convert.ToDouble(TempData["CourseBuy"]);
            string month = Convert.ToString(TempData["Month"]);
            TempData["CourseSel"] = courseSel;
            TempData["CourseBuy"] = courseBuy;
            TempData["Month"] = month;
            TempData.Keep();

            foreach (var person in clients)
            {
                if (person.Id == ViewBag.Id)
                {
                    if (person.Money < courseSel)
                    {
                        TempData["Possible"] = "Данная операция недоступна, так как на Вашем счёте недостаточно средств";
                        return Redirect("/Bank/ClientPage");
                    }
                    TempData["Id"] = person.Id;
                    TempData["Name"] = person.Name;
                    TempData["Surname"] = person.Surname;
                    TempData["Patronymic"] = person.Patronymic;
                    TempData["Login"] = person.Login;
                    TempData["Password"] = person.Password;
                    TempData["Money"] = person.Money;
                    TempData["MoneyInCourse"] = person.MoneyInCourse;
                    TempData["MaxToBuy"] = Math.Round(person.Money / courseSel, 2);
                    TempData.Keep();
                    ViewBag.Title = "Продать валюту";
                    return View();
                }
            }
            ViewBag.Title = "Купить валюту";

            return View();
        }

        [HttpPost]
        public String BuyValute(ValuteOpereation operation)
        {
            MakeNeedOperation();
            double sumOfValute = operation.Summa;
            ViewBag.Title = "Сделать вклад";
            ViewBag.Id = TempData["Id"];
            var clients = _context.Residents;
            Resident currentResident = new Resident();
            Resident bank = new Resident();

            double courseSel = Convert.ToDouble(TempData["CourseSel"]);
            double courseBuy = Convert.ToDouble(TempData["CourseBuy"]);
            string month = Convert.ToString(TempData["Month"]);
            TempData["CourseSel"] = courseSel;
            TempData["CourseBuy"] = courseBuy;
            TempData["Month"] = month;
            TempData.Keep();

            foreach (var person in clients)
                if (person.Surname == "Банк")
                    bank = person;

            foreach (var person in clients)
            {
                if (person.Id == ViewBag.Id)
                {
                    TempData["Id"] = person.Id;
                    TempData["Name"] = person.Name;
                    TempData["Surname"] = person.Surname;
                    TempData["Patronymic"] = person.Patronymic;
                    TempData["Login"] = person.Login;
                    TempData["Password"] = person.Password;
                    TempData["Money"] = person.Money;
                    TempData["MoneyInCourse"] = person.MoneyInCourse;
                    TempData.Keep();
                    currentResident = person;
                }
            }
            if (sumOfValute == 0)
                return "Сумма покупки валюты не может соствлять 0 единиц";
            if (sumOfValute > (currentResident.Money / courseSel))
                return "Вы не располагаете такой суммой";
            double spendRubles = Math.Round(sumOfValute * courseSel, 2);
            currentResident.MoneyInCourse = currentResident.MoneyInCourse + sumOfValute;
            bank.MoneyInCourse = bank.MoneyInCourse - sumOfValute;
            currentResident.Money = currentResident.Money - spendRubles;
            bank.Money = bank.Money + spendRubles;
            _context.SaveChanges();
            return "Операция успешно выполнена! Вы купили " + Convert.ToString(sumOfValute) + " долларов за " + Convert.ToString(spendRubles) + " рублей.";
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

        public bool MakeTransferForGetPackage(Resident sender, Resident recipient, double summa, double courseBuy)
        {
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

        public bool MakeTransfer(EnterPackage needTransfer)
        {
            double Course = GetCourse();
            Random rnd = new Random();
            int difference = rnd.Next(0, 5);
            int mn = rnd.Next(0, 9);
            double courseSel = Course;
            double courseBuy = Course - Convert.ToDouble(difference) - Convert.ToDouble((0.12) * mn);
            TempData["CourseSel"] = courseSel;
            TempData["CourseBuy"] = courseBuy;
            TempData.Keep();
            string month = Convert.ToString(TempData["Month"]);
            TempData["Month"] = month;
            //          TempData["CourseSel"] = 61.32;//////////////////////////////
            //     TempData["CourseBuy"] = 59.17;//////////////////////////////
            //   double courseBuy = 59.17;//////////////////////////////
            TempData.Keep();
            if (needTransfer.Sender.Surname == "Муниципалитет")    // если деньги идут от муниципалитета (значит перечисляют зарплату)
            {
                Resident sender = new Resident();
                sender = GetResidentByFIO(needTransfer.Sender);   // отправитель денег муниципалитет
                Resident recipient = new Resident();
                recipient = GetResidentByFIO(needTransfer.Recipient);   // получатель денег

                MakeTransferForGetPackageByMunitsupalitet(sender, recipient, needTransfer.Summa);
                return true;
            }
            if (needTransfer.Recipient.Surname == "ЖКХ")  // если получатель денег ЖКХ (значит необходимо перечислить деньги за ком. услуги)
            {
                int numberOfHouse = needTransfer.Sender.Home;
                List<Resident> ResidentForPay = new List<Resident>();
                var allResident = _context.Residents;
                Resident jhk = new Resident();
                foreach (var person in allResident)   // среди всех зарегистрированных клиентов ищем жителей необходимого дома
                {
                    if (person.Home == numberOfHouse)
                        ResidentForPay.Add(person);
                    if (person.Surname == "ЖКХ")
                        jhk = person;
                }
                int countOfResidentForPay = ResidentForPay.Count;   // находим количество жителей данного дома
                double summaForPay = needTransfer.Summa / countOfResidentForPay;   // сумма платежа ЖКХ делится на  всех жителей дома

                foreach (var person in ResidentForPay)    // с каждого жителя списываем необходимую сумму
                    MakeTransferForGetPackage(person, jhk, summaForPay, courseBuy);
                return true;
            }
            else   // если этот пакет не для перечисления з/п и уплаты ком. услуг 
            {
                Resident sender = new Resident();
                sender = GetResidentByFIO(needTransfer.Sender);   // определяем отправителя денег 
                Resident recipient = new Resident();
                recipient = GetResidentByFIO(needTransfer.Recipient);   // определяетм получателя денег

                MakeTransferForGetPackage(sender, recipient, needTransfer.Summa, courseBuy); // выполняем перевод
            }

            return true;
        }
    }
}