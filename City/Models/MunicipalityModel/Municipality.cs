using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.MunicipalityModel
{
    public class Municipality : CityObject
    {
        public Municipality(ApplicationContext context, DataBus bus) : base(context, bus)
        {
        }

        public override void ProcessPackage(Package package)
        {
            throw new NotImplementedException();
        }
    }
}
