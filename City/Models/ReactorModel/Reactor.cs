using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace City.Models.ReactorModel
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
        /// <summary>
        /// Состояние ядерного взрыва:произошел он или нет
        /// </summary>
        private bool nuclearBlast = false;
        public bool NuclearBlast
        {
            get { return nuclearBlast; }
            set { nuclearBlast = value; }
        }
        /// <summary>
        /// Состояние урановых стержней
        /// </summary>
        private bool stateOfRod = false;
        public bool StateOfRod
        {
            get { return stateOfRod; }
            set { stateOfRod = value; }
        }
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
        /// <summary>
        /// Температура, при которой происходит взрыв реактора
        /// </summary>
        private int blastTemperature = 3000;
        public int BlastTemperature
        {
            get { return blastTemperature; }
        }
        /// <summary>
        /// Опускание урановых стержней
        /// </summary>
        public void DownRod()
        {
            stateOfRod = true;
        }
        /// <summary>
        /// Поднятие урановых стежней
        /// </summary>
        public void UpRod()
        {
            stateOfRod = false;
        }
        /// <summary>
        /// Изменение числа, на которое изменяется температура
        /// Если стержни опущены, то температура изменяется медленнее
        /// Если они подняты, то температура растет быстрее
        /// </summary>
        public void ChangeDlt(int percent)
        {
            if (stateOfRod == true)
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
            nuclearBlast = true;
            FlagVoid = true;
        }
    }
}
