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
        /// Название метода на Arduino для включения дыма, белого цвета лампы и отключения вентилятора(турбины)
        /// </summary>
        public const string ArduinoBlastMethod = "NuclearBlast";
        /// <summary>
        /// Название метода на Arduino для изменения цвета подсветки
        /// </summary>
        public const string ArduinoLampColorMethod = "ChangeColorLamp";
        /// <summary>
        /// Состояние реактора
        /// </summary>
        public bool IsOnReactor { get; set; }
        /// <summary>
        /// Взорван реактор или нет(false-нет/true-да) 
        /// </summary>
        public bool NuclearBlast { get; set; } = false;
        /// <summary>
        /// Подняты стержни или нет(true-опущены/false-подняты)
        /// </summary>
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
        /// <summary>
        /// Температура, при которой происходит взрыв реактора
        /// </summary>
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
        /// </summary>
        /// <remarks>
        /// Если стержни опущены, то температура изменяется медленнее
        /// Если они подняты, то температура растет быстрее
        /// </remarks>
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
            SendToArduino(ArduinoBlastMethod, "");
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
                //TODO: Вместо # необходимо вписывать IP соответствующего объекта
                //IP 192.168.0.2
                String URL = "http://192.168.0.2/" + Method + "?p=" + p;
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
