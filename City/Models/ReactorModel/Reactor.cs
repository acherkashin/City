using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Models.ReactorModel
{
    public class Reactor
    {
        /// <summary>
        /// Переменная, которая передается на Arduino для включения дыма
        /// </summary>
        public bool IsOnVoid { get; set; } = false;
        /// <summary>
        /// Переменная, которая передается на Arduino для включения лампочки соответствующего цвета:
        /// 1 - Синяя, 2 - Зеленая, 3 - Красная, 4 - Белая
        /// </summary>
        public string LampColor { get; set; } = "";
        /// <summary>
        /// Состояние реактора
        /// </summary>
        public bool IsOnReactor { get; set; }
        public bool NuclearBlast { get; set; } = false;
        public bool IsUpRod { get; set; } = false;

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
            IsUpRod = true;
        }

        /// <summary>
        /// Поднятие урановых стежней
        /// </summary>
        public void UpRod()
        {
            IsUpRod = false;
        }

        /// <summary>
        /// Изменение числа, на которое изменяется температура
        /// Если стержни опущены, то температура изменяется медленнее
        /// Если они подняты, то температура растет быстрее
        /// </summary>
        public void ChangeDlt(int percent)
        {
            if (IsUpRod == true)
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
            IsOnVoid = true;
            LampColor = "4";
            SendToArduino("NameOfMethodVoid", Convert.ToString(IsOnVoid));
            SendToArduino("NameOfMethodLamp", LampColor);
        }

        /// <summary>
        /// Отправка данных на Arduino
        /// </summary>
        /// <param name="Method">Название метода</param>
        /// <param name="p">Параметр</param>
        public void SendToArduino(String Method, string p)
        {
            try
            {
                ///<summary>
                ///TODO: Вместо # необходимо вписывать IP соответствующего объекта
                ///IP 192.168.0.2
                ///</summary>
                String URL = "http://192.168.#.#/" + Method + "?p=" + p;
                WebRequest request = WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                WebResponse response = request.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {

            }
        }

    }
}
