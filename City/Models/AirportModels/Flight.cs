using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.AirportModels
{
    public class Flight
    {
        /// <summary>
        /// Номер рейса (не несет важной информации)
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Дата вылета
        /// </summary>
        public string DepartureDate { get; set; }
        /// <summary>
        /// Дата прилета
        /// </summary>
        public string ArrivalDate { get; set; }
        /// <summary>
        /// Пассажиры, летящие этим рейсом
        /// </summary>
        public virtual ICollection<Passenger> Passengers { get; set; }
        public Flight()
        {
            Passengers = new List<Passenger>();
        }
    }
}
