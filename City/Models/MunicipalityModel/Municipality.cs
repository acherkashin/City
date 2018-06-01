using CyberCity.Models.HouseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.MunicipalityModel
{
    public class Municipality : CityObject
    {
        public const string UpdateTarifMethod = "UpdateTarif";

        public Municipality(ApplicationContext context, DataBus bus) : base(context, bus)
        {
        }

        public void Start()
        {
            new Task(() =>
            {
                _bus.Send(new Package()
                {
                    From = Subject.Municipality,
                    To = Subject.Houses,
                    Method = UpdateTarifMethod,
                    Params = Newtonsoft.Json.JsonConvert.SerializeObject(new Tarifs()
                    {
                        Electric = Generator.GenerateValue(1, 10),
                        Gas = Generator.GenerateValue(1, 10),
                        Watter = Generator.GenerateValue(1, 10),
                    })
                });
            }).Start();
        }

        public override void ProcessPackage(Package package)
        {
            throw new NotImplementedException();
        }
    }
}
