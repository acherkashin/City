using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace City.Models
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
        public string Params { get; set; }

        public Package CreateEncreted()
        {
            var clone = MemberwiseClone();

            //TODO: Добавить шифрование

            return clone as Package;
        }
    }
}
