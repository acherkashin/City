using CyberCity.Models.AirportModels;
using CyberCity.Models.HouseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.BankModel
{
    public class Bank : CityObject
    {

        public Bank(ApplicationContext context, DataBus bus) : base(context, bus)
        {
        }

        public override void ProcessPackage(Package package)
        {
            if (package.Method == Houses.SendMetricsMethod)
            {
                //TODO Черкашин: добавить обработку
            }
            else if (package.Method == Airport.AirportInvoiceMethod)
            {

            }
        }
    }
}
