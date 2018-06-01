using CyberCity.Models.AirportModels;
using CyberCity.Models.HouseModels;
using CyberCity.Models.MunicipalityModel;

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
            else if (package.Method == Municipality.PaySalaryMethod)
            {

            }
        }
    }
}
