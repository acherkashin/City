using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.SubStationModel
{
    public class SubstationState
    {
        /// <summary>
        /// Передаваемая мощность в город
        /// </summary>
        public double Power { get; set; }
        /// <summary>
        /// Текущее состояние сирены (вкл/выкл)
        /// </summary>
        public bool StSiren { get; set; }
        /// <summary>
        /// Текущее состояние реле (вкл/выкл)
        /// </summary>
        public bool StRele { get; set; }
        /// <summary>
        /// Для записи состояния в БД
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
