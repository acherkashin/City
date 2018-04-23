using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace City.Models
{
    public class SubjectState
    {
        public int Id { get; set; }

        public Subject Subject { get; set; }

        /// <summary>
        /// Модель оюъекта в формате JSON
        /// </summary>
        public string State { get; set; }
    }
}
