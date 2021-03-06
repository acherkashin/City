﻿using CyberCity.Models.AccountModel;
using CyberCity.Models.Core;

namespace CyberCity.Models
{
    public abstract class CityObject
    {
        /// <summary>
        /// Идентификатор пользователя, который является защитником для данного объекта
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Контекст данных.
        /// </summary>
        protected ApplicationContext _context => ContextFactory.GetContext();

        /// <summary>
        /// Шина передачи пакетов
        /// </summary>
        protected DataBus _bus;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="context"> Контекст данных.</param>
        /// <param name="bus"> Шина передачи пакетов.</param>
        protected CityObject(DataBus bus)
        {
            _bus = bus;
        }

        public abstract void ProcessPackage(Package package);

        public User GetUser()
        {
            return _context.Users.Find(UserId);
        }
    }
}
