using City.Models.Reactor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace City.Models.ReactorModel
{
    public class ReactorRunner
    {
        private ApplicationContext _context;

        public ReactorRunner(ApplicationContext context)
        {
            _context = context;
        }

        public event Action<ReactorData> OnStateChanged;

        /// <summary>
        /// Что такое FLAG?
        /// </summary>
        bool FLAG = false;

        private Object thisLock = new Object();

        Reactor r1 = new Reactor();
        Turbine t1 = new Turbine();

        public void StartReactor()
        {

            lock (thisLock)
            {
                r1.stateOfReactor = true;
                while (r1.NuclearBlast == false)
                {
                    /// <summary>
                    /// TODO: Тут нужно все данные получать из базы данных
                    /// </summary>
                    var FL = FLAG;
                    r1.StateOfRod = FLAG;
                    ChangeTemperatue();

                    OnStateChanged?.Invoke(new ReactorData()
                    {
                        Temperature = r1.currentTemperature,
                        Energy = r1.energy,
                        RPM = t1.currentRPM,
                        Vibration = t1.currentVibration,
                        StReactor = r1.stateOfReactor,
                        StRod = r1.StateOfRod,
                        //TODO: Что значит состояние турбины??? 
                        StTurbine = t1.StateOfTurbine,
                    });
                    /// TODO: Тут нужно все данные отправлять в базу данных

                    //TODO: для чего нужны переменные, объявленные ниже?
                    var currentE = r1.energy;
                    var currentR = t1.currentRPM;
                    var currentV = t1.currentVibration;
                    var currentT = r1.currentTemperature;
                    var stTurbine = t1.StateOfTurbine;
                    var stReactor = r1.StateOfRod;
                    var stStation = t1.FlagForStation;
                    Thread.Sleep(60000);
                }
                var RESULT = r1.NuclearBlast;
            }
        }

        //TODO: добавить, откуда взяты формулы
        /// <summary>
        /// Изменение температуры
        /// </summary>
        private void ChangeTemperatue()
        {
            if (r1.StateOfRod)
            {
                r1.currentTemperature = r1.currentTemperature - r1.dlt;
            }
            else
            {
                r1.currentTemperature = r1.currentTemperature + r1.dlt;
            }

            if (t1.StateOfTurbine == false)
            {
                r1.ChangeDlt(15);
            }
            else
            {
                r1.ChangeDlt(5);
            }
            if (r1.currentTemperature > r1.BlastTemperature)
            {
                r1.stateOfReactor = false;
                t1.StateOfTurbine = false;
                t1.Stop();
                r1.BlastReactor();
            }
            //Если текущаяя температура > температуры для взрыва, то происходит взрыв
            else
            {
                if (r1.currentTemperature <= 0)
                {
                    r1.currentTemperature = 0;
                }
                //Если температура меньше или равна 0, то текущая температра=0
                else
                {
                    if (r1.currentTemperature >= r1.MinTemperature)
                    {
                        if (t1.currentRPM == 0 && t1.StateOfBroken != true)
                        {
                            t1.Start();
                        }
                        if (t1.StateOfTurbine == true)
                        {
                            ChangeRPM();
                        }
                    }
                    else
                    {
                        if (t1.currentRPM < 10)
                        {
                            t1.currentRPM = 0;
                            r1.energy = (t1.currentRPM / t1.MaxRPM) * 100;
                        }
                        else
                        {
                            t1.currentRPM = t1.currentRPM / 2;
                            r1.energy = (t1.currentRPM / t1.MaxRPM) * 100;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Изменение оборотов турбины
        /// </summary>
        private void ChangeRPM()
        {
            Random randomValue = new Random();
            //тут нужно добавить счетчик, который проверяет у турбины вышло ли время на починку
            if (t1.currentRPM < t1.MaxRPM && r1.StateOfRod == false)
            {
                t1.currentRPM = (r1.currentTemperature) * 0.05 + t1.currentRPM * 1.5;
            }
            else
            {
                if (t1.currentRPM < t1.MaxRPM && r1.StateOfRod == true)
                {
                    t1.currentRPM = (r1.currentTemperature) * 0.05 + t1.currentRPM * 1.5;
                }
                else
                {
                    t1.currentRPM = t1.MaxRPM + randomValue.Next(-1, 2) + randomValue.Next(0, 51);
                }
            }
            if (t1.currentRPM < t1.MinRPM)
            {
                r1.energy = (t1.currentRPM / t1.MaxRPM) * 100;
            }
            else
            {
                if (t1.currentRPM > t1.MaxRPM + 200)
                {
                    ChangeVibration();
                    ChangeEnergy();
                }
                else
                {
                    ChangeEnergy();
                }
            }
            if (t1.currentVibration > t1.MaxVibration)
            {
                t1.Stop();
                r1.energy = 0;
            }
        }

        //Почему change eneregy, если устанавливает значение 100. Почему именно 100?
        private void ChangeEnergy()
        {
            r1.energy = 100;
        }

        private void ChangeVibration()
        {
            if (t1.StateOfBroken != true)
            {
                t1.currentVibration += 100;
            }
            else
            {
                t1.currentVibration = 0;
            }

        }
    }
}
