using System;
using System.Collections.Generic;
using CyberCity.Models;
using CyberCity.Models.BankModel;
using CyberCity.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controllers
{
    [Authorize(Roles = "Bank")]
    public class BankController : BaseController
    {
        private readonly City _city;

        public BankController(ApplicationContext context, City city)
        {
            _context = context;
            _city = city;
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

            TempData["CourseSel"] = _city.Bank.CourseSell;
            TempData["CourseBuy"] = _city.Bank.CourseBuy;
            TempData["Month"] = DateUtils.GetMonth(DateTime.Now.Month);

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

            if (TempData["Possible"] != null)
            {
                ViewBag.Possible = TempData["Possible"];
                TempData["Possible"] = ViewBag.Possible;
            }
            else
            {
                TempData["Possible"] = "";
            }

            ViewBag.Id = TempData["Id"];
            var clients = _context.Residents;
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
            {
                return "Сумма покупки валюты не может соствлять 0 единиц";
            }

            if (sumOfValute > (currentResident.Money / courseSel))
            {
                return "Вы не располагаете такой суммой";
            }

            double spendRubles = Math.Round(sumOfValute * courseSel, 2);
            currentResident.MoneyInCourse = currentResident.MoneyInCourse + sumOfValute;
            bank.MoneyInCourse = bank.MoneyInCourse - sumOfValute;
            currentResident.Money = currentResident.Money - spendRubles;
            bank.Money = bank.Money + spendRubles;
            _context.SaveChanges();

            return "Операция успешно выполнена! Вы купили " + Convert.ToString(sumOfValute) + " долларов за " + Convert.ToString(spendRubles) + " рублей.";
        }
    }
}