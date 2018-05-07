using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace City.Models.Reactor
{
    public class Turbine
    {
        // TODO: из названия не понятно назначение переменной
        public bool FlagForStation { get; set; } = false;
        public bool StateOfTurbine { get; set; } = false;
        public double currentRPM { get; set; }
        private int maxRPM = 3000;
        public int MaxRPM
        {
            get { return maxRPM; }
        }
        private int minRPM = 2500;
        public int MinRPM
        {
            get { return minRPM; }
        }
        public double currentVibration { get; set; }
        private int maxVibration = 300;
        public int MaxVibration
        {
            get { return maxVibration; }
        }
        private bool stateOfBroken = false;
        public bool StateOfBroken
        {
            get { return stateOfBroken; }
            set { stateOfBroken = value; }
        }

        public void Start()
        {
            stateOfBroken = false;
            StateOfTurbine = true;
            FlagForStation = false;
        }
        public void Stop()
        {
            TimerOfBroken();
            StateOfTurbine = false;
            StateOfBroken = true;
            currentRPM = 0;
            currentVibration = 0;
            FlagForStation = true;
        }
        public void TimerOfBroken()
        {
            TimerCallback tm = new TimerCallback(RepareTurbine);
            Timer timer = new Timer(tm, null, 150000, -1);
        }
        private void RepareTurbine(object obj)
        {
            Start();
        }
    }
}
