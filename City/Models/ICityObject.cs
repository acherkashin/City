using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models
{
    public interface ICityObject
    {
        void ProcessPackage(Package package);
    }
}
