using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.ReactorModel
{
    public class ReactorData
    {
        /// <summary>
        /// Текущая температура реактора
        /// </summary>
        public double Temperature { get; set; }
        /// <summary>
        /// Текущие обороты турбины
        /// </summary>
        public double RPM { get; set; }
        /// <summary>
        /// Текущая вибрация турбины
        /// </summary>
        public double Vibration { get; set; }
        /// <summary>
        /// Текущая энергия реактора
        /// </summary>
        public double Energy { get; set; }
        /// <summary>
        /// Состояние реактора
        /// </summary>
        public bool StReactor { get; set; }
        /// <summary>
        /// Состояние урановых стержней
        /// </summary>
        public bool StRod { get; set; }
        /// <summary>
        /// Состояние турбины
        /// </summary>
        public bool StTurbine { get; set; }

        /// <summary>
        /// Для записи состояния в БД
        /// </summary>
        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
