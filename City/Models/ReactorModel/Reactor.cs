using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.ReactorModel
{
    public class Reactor
    {
        /// <summary>
        /// Переменная, которая передается на Arduino для включения дыма
        /// </summary>
        public bool FlagVoid { get; set; } = false;
        /// <summary>
        /// Состояние реактора
        /// </summary>
        public bool stateOfReactor { get; set; }
        public bool NuclearBlast { get; set; } = false;
        public bool StateOfRod { get; set; } = false;

        /// <summary>
        /// Текущая энергия
        /// </summary>
        public double energy { get; set; }
        /// <summary>
        /// Текущая температура
        /// </summary>
        public double currentTemperature { get; set; }
        /// <summary>
        /// Число, на которое изменяется температура
        /// </summary>
        public double dlt { get; set; }
        /// <summary>
        /// Температура, при которой включаются турбины
        /// </summary>
        public int MinTemperature = 180;
        public int BlastTemperature { get; } = 3000;

        /// <summary>
        /// Опускание урановых стержней
        /// </summary>
        public void DownRod()
        {
            StateOfRod = true;
        }
        /// <summary>
        /// Поднятие урановых стежней
        /// </summary>
        public void UpRod()
        {
            StateOfRod = false;
        }
        /// <summary>
        /// Изменение числа, на которое изменяется температура
        /// Если стержни опущены, то температура изменяется медленнее
        /// Если они подняты, то температура растет быстрее
        /// </summary>
        public void ChangeDlt(int percent)
        {
            if (StateOfRod == true)
            {
                dlt = 5 + 0.01 * percent * currentTemperature * 0.1;
            }
            else
            {
                dlt = 5 + 0.01 * percent * currentTemperature;
            }

        }
        /// <summary>
        /// Взрыв реактора
        /// </summary>
        public void BlastReactor()
        {
            NuclearBlast = true;
            FlagVoid = true;
            UseVoid();
        }
        /// <summary>
        /// Включение дыма
        /// </summary>
        public void UseVoid()
        {
            /// <summary>
            /// TODO: Тут необходимо отправлять переменную FlagVoid на Arduino для включения дыма
            /// </summary>
        }
    }
}
