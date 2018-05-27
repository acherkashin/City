using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models
{
    public class Package
    {
        public int Id { get; set; }
        public Subject From { get; set; }
        public Subject To { get; set; }
        public string Method { get; set; }
        /// <summary>
        /// JSON объект
        /// </summary>
        /// <remarks>
        /// Необходимо сериализовать все пакеты в строку, для дальнейшего сохранения их в базу
        /// </remarks>
        public string Params { get; set; }

        public Package CreateEncreted()
        {
            var clone = MemberwiseClone();

            //TODO: Добавить шифрование

            return clone as Package;
        }
    }
}
