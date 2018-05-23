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
        /// Переменная, которая передается на Arduino для включения/отключения белой лампочки
        /// </summary>
        public bool IsOnLamp { get; set; } = false;
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
            IsOnLamp = true;
            SendToArduino("NameOfMethodVoid", IsOnVoid);
            SendToArduino("NameOfMethodLamp", IsOnLamp);
        }
        /// <summary>
        /// Отправка данных на Arduino
        /// </summary>
        /// <param name="Method">Название метода</param>
        /// <param name="p">Параметр</param>
        public void SendToArduino(String Method,bool p)
        {
            ///<summary>
            ///TODO: Правильно ли формируются параметры и метод?
            /// </summary>
            String Parameters = Newtonsoft.Json.JsonConvert.SerializeObject(p);
            // Create a request using a URL that can receive a post.  
            ///<summary>
            ///TODO: Вместо # необходимо вписывать IP соответствующего объекта
            /// </summary>           
            String URL = "192.168.#.#/"+Method+Parameters;
            WebRequest request = WebRequest.Create(URL);

            // Set the Method property of the request to POST.  
            request.Method = "POST";
            // Create POST data and convert it to a byte array.  
            string postData = "This is a test that posts this string to a Web server.";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.  
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.  
            request.ContentLength = byteArray.Length;
            // Get the request stream.  
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.  
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.  
            dataStream.Close();
            // Get the response.  
            WebResponse response = request.GetResponse();
            // Clean up the streams.  
            dataStream.Close();
            response.Close();
        }

    }
}
