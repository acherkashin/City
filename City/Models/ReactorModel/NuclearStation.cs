using CyberCity.Models.Reactor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CyberCity.Models.ReactorModel
{
    public class NuclearStation : ICityObject
    {
        public void ProcessPackage(Package package)
        {
            throw new NotImplementedException();
        }

        private ApplicationContext _context;
        private NetHub _hub;

        /// <summary>
        /// IsUpRodGlobal - глобальная переменная, которая отвечает за текущее состояние стержня, чтобы к ней можно было обращаться из метода StartReactor.
        /// т.е. при нажатии на кнопок на html-странице происходит изменение переменной IsUpRodGlobal, а после к ней обращается метод StartReactor и работает уже с ней
        /// </summary>
        bool IsUpRodGlobal = false;
        private Object lockObj = new Object();
        Reactor reactor = new Reactor();
        Turbine turbine = new Turbine();

        public NuclearStation(ApplicationContext context, NetHub hub)
        {
            _context = context;
            _hub = hub;
        }

        public void Start()
        {
            new Task(StartReactor).Start();
        }

        public void ChangeRodState(bool flag)
        {
            IsUpRodGlobal = flag;
            reactor.IsUpRod = IsUpRodGlobal;
        }

        private void StartReactor()
        {
            lock (lockObj)
            {
                reactor.IsOnReactor = true;
                while (reactor.NuclearBlast == false)
                {
                    /// <summary>
                    /// TODO: Тут нужно все данные получать из базы данных
                    /// </summary>

                    /// <summary>
                    /// Тут происходит обращение к глобальной переменной и это значение присваивается в текущее состояние стержня
                    /// </summary>
                    reactor.IsUpRod = IsUpRodGlobal;
                    ChangeTemperatue();

                    _hub.SendStateChanged(Subject.NuclearStation, GetState());

                    /// TODO: Тут нужно все данные отправлять в базу данных

                    Thread.Sleep(60000);
                }
            }
        }

        /// <summary>
        /// Изменение температуры
        /// </summary>
        /// <remarks>
        /// Формулы придуманы для упрощения работы реактора
        /// При 180 градусах начинают работать турбины. Когда обороты достигают 2500, то энергия начинает вырабатываться на 100%.
        /// Когда обороты достигают 2500, то они начинают изменяться в диапазоне +-50 об. при каждой итерации цикла
        /// Если обороты достигают 3200 и более, то начинает расти вибрация. Когда вибрация достигает 300 ед., то происходит поломка турбины на 150 секунд
        /// При поломке турбины температура начинает расти на 10% быстрее. Т.е. при работающей турбине она растет на 5%, а при поломанной на 15%
        /// </remarks>
        private void ChangeTemperatue()
        {
            if (reactor.IsUpRod)
            {
                reactor.currentTemperature = reactor.currentTemperature - reactor.dlt;
            }
            else
            {
                reactor.currentTemperature = reactor.currentTemperature + reactor.dlt;
            }

            if (turbine.IsOnTurbine == false)
            {
                reactor.ChangeDlt(15);
            }
            else
            {
                reactor.ChangeDlt(5);
            }
            /// <summary>
            /// Если температура >= температуры взрыва, то происходит взрыв(логично...)
            /// </summary>
            if (reactor.currentTemperature > reactor.BlastTemperature)
            {
                reactor.IsOnVoid = true;
                reactor.IsOnReactor = false;
                turbine.IsOnTurbine = false;
                turbine.Stop();
                OnSiren();
                reactor.BlastReactor();
            }
            else
            {
                /// <summary>
                /// Если температура меньше или равна 0, то текущая температра=0
                /// </summary>
                if (reactor.currentTemperature <= 0)
                {
                    reactor.currentTemperature = 0;
                }
                else
                {
                    /// <summary>
                    /// При 180 градусах начинают работать турбины
                    /// </summary>
                    if (reactor.currentTemperature >= reactor.MinTemperature)
                    {
                        if (turbine.currentRPM == 0 && turbine.IsBroken != true)
                        {
                            turbine.Start();
                        }
                        if (turbine.IsOnTurbine == true)
                        {
                            ChangeRPM();
                        }
                    }
                    else
                    {
                        /// <summary>
                        /// При 180 градусах начинают работать турбины
                        /// </summary>
                        if (turbine.currentRPM < 10)
                        {
                            turbine.currentRPM = 0;
                            reactor.energy = (turbine.currentRPM / turbine.MaxRPM) * 100;
                        }
                        else
                        {
                            turbine.currentRPM = turbine.currentRPM / 2;
                            reactor.energy = (turbine.currentRPM / turbine.MaxRPM) * 100;
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
            if (turbine.currentRPM < turbine.MaxRPM && reactor.IsUpRod == false)
            {
                turbine.currentRPM = (reactor.currentTemperature) * 0.05 + turbine.currentRPM * 1.5;
            }
            else
            {
                if (turbine.currentRPM < turbine.MaxRPM && reactor.IsUpRod == true)
                {
                    turbine.currentRPM = (reactor.currentTemperature) * 0.05 + turbine.currentRPM * 1.5;
                }
                /// <summary>
                /// При достижении "рабочих" оборотов в 2500 ед. обороты изменяются на +-50 оборотов
                /// </summary>
                else
                {
                    turbine.currentRPM = turbine.MaxRPM + randomValue.Next(-1, 2) + randomValue.Next(0, 51);
                }
            }
            /// <summary>
            /// Если текущие обороты меньше "рабочих", то энергия будет находиться в диапазоне 0-100%
            /// </summary>
            if (turbine.currentRPM < turbine.MinRPM)
            {
                reactor.energy = (turbine.currentRPM / turbine.MaxRPM) * 100;
            }
            else
            {
                /// <summary>
                /// Если обороты больше максимальных, то начинает расти вибрация
                /// </summary>
                if (turbine.currentRPM > turbine.MaxRPM)
                {
                    ChangeVibration();
                    ChangeEnergy();
                }
                else
                {
                    ChangeEnergy();
                }
            }
            /// <summary>
            /// Если вибрация достигает предела(300 ед.), то происходит поломка
            /// </summary>
            if (turbine.currentVibration > turbine.MaxVibration)
            {
                turbine.Stop();
                OnSiren();
                reactor.energy = 0;
            }
        }
        /// <summary>
        /// Изменение энергии на 100% при достижении турбиной "рабочих" оборотов в 2500-3200 ед. 100 означает 100%
        /// </summary>
        private void ChangeEnergy()
        {
            reactor.energy = 100;
        }
        /// <summary>
        /// Изменение уровня вибрации
        /// Если турбина поломана, вибрация =0
        /// </summary>
        private void ChangeVibration()
        {
            if (turbine.IsBroken != true)
            {
                turbine.currentVibration += 100;
            }
            else
            {
                turbine.currentVibration = 0;
            }

        }
        /// <summary>
        /// Включение сирены
        /// </summary>
        private void OnSiren()
        {
            _hub.Send(new Package()
            {
                From = Subject.NuclearStation,
                To = Subject.Substation,
                Method = "Siren",
                Params = Newtonsoft.Json.JsonConvert.SerializeObject(turbine.IsOnSiren),
            });
        }

        public ReactorData GetState()
        {
            return new ReactorData()
            {
                Temperature = reactor.currentTemperature,
                Energy = reactor.energy,
                RPM = turbine.currentRPM,
                Vibration = turbine.currentVibration,
                StReactor = reactor.IsOnReactor,
                StRod = reactor.IsUpRod,
                StTurbine = turbine.IsOnTurbine,
            };
        }
    }
}
