using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.HouseModels
{
    public static class Generator
    {

        public static int GenerateValue(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
    }
}
