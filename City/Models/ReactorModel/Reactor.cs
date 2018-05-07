using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace City.Models.ReactorModel
{
    //TODO: добавить полное описание 
    public class Reactor
    {
        public bool stateOfReactor { get; set; }
        private bool nuclearBlast = false;
        public bool NuclearBlast
        {
            get { return nuclearBlast; }
            set { nuclearBlast = value; }
        }
        private bool stateOfRod = false;
        public bool StateOfRod
        {
            get { return stateOfRod; }
            set { stateOfRod = value; }
        }
        public double energy { get; set; }
        public double currentTemperature { get; set; }
        public double dlt { get; set; }
        public int MinTemperature = 180;
        private int blastTemperature = 3000;
        public int BlastTemperature
        {
            get { return blastTemperature; }
        }

        public void DownRod()
        {
            stateOfRod = true;
        }

        public void UpRod()
        {
            stateOfRod = false;
        }

        // Что такое Dlt?
        public void ChangeDlt(int percent)
        {
            if (stateOfRod == true)
            {
                dlt = 5 + 0.01 * percent * currentTemperature * 0.1;
            }
            else
            {
                dlt = 5 + 0.01 * percent * currentTemperature;
            }

        }

        public void BlastReactor()
        {
            nuclearBlast = true;
            bool TEMp = NuclearBlast;
        }
    }
}
