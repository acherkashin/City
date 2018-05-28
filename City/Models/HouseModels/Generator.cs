using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.HouseModels
{
    public static class Generator
    {

        public static float GenerateValue(float min, float max)
        {
            Random random = new Random();

            return (float) random.NextDouble() * (max-min) + min;
        }
    }
}
