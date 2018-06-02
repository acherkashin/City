using CyberCity.Models.Core;
using CyberCity.Models.Reactor;
using CyberCity.Models.SubStationModel;
using System;

namespace CyberCity.Models.ReactorModel
{
    /// <summary>
    /// Атомная станция
    /// </summary>
    public class NuclearStation : CityObject
    {
        public override void ProcessPackage(Package package)
        {
            throw new NotImplementedException();
        }

        private Object lockObj = new Object();
        Reactor reactor = new Reactor();
        Turbine turbine = new Turbine();

        public NuclearStation(ApplicationContext context, DataBus bus) : base(context, bus)
        {
            reactor.IsOnReactor = true;
        }

        public void ChangeRodState(bool flag)
        {
            reactor.IsUpRod = flag;
        }

        public void Work(CityTime time)
        {
            if (!reactor.NuclearBlast)
            {
                ChangeTemperatue();
                _bus.SendStateChanged(Subject.NuclearStation, GetState());
                // Тут происходит отправка данных на Электрическую подстанцию
                SendEnergyForSubStation();
            }
        }

        /// <summary>
        /// Изменение температуры
        /// </summary>
        /// <remarks>
        /// Формулы придуманы для упрощения работы реактора
        /// При 180 градусах начинают работать турбины. Когда обороты достигают 3000, то энергия начинает вырабатываться на 100%.
        /// Когда обороты достигают 3000, то они начинают изменяться в диапазоне +-50 об. при каждой итерации цикла
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
            //Тут происходит отправка данных на Arduino Атомной станции для изменения цвета лампочек
            reactor.SendToArduino(Reactor.ArduinoLampColorMethod, Convert.ToString(reactor.currentTemperature));
            if (turbine.IsOnTurbine == false)
            {
                reactor.ChangeDlt(15);
            }
            else
            {
                reactor.ChangeDlt(5);
            }
            // Если температура >= температуры взрыва, то происходит взрыв(логично...)
            if (reactor.currentTemperature > reactor.BlastTemperature)
            {
                reactor.IsOnReactor = false;
                turbine.IsOnTurbine = false;
                turbine.Stop();
                OnSiren();
                reactor.BlastReactor();
            }
            else
            {
                // Если температура меньше или равна 0, то текущая температра = 0
                if (reactor.currentTemperature <= 0)
                {
                    reactor.currentTemperature = 0;

                }
                else
                {
                    //При рабочей температуре(от 180) начинают работать турбины
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
            if (turbine.currentRPM < (turbine.MaxRPM - 200) && reactor.IsUpRod == false)
            {
                turbine.currentRPM = (reactor.currentTemperature) * 0.05 + turbine.currentRPM * 1.5;
            }
            else
            {
                if (turbine.currentRPM < (turbine.MaxRPM - 200) && reactor.IsUpRod == true)
                {
                    turbine.currentRPM = (reactor.currentTemperature) * 0.05 + turbine.currentRPM * 1.5;
                }
                // При достижении "рабочих" оборотов в 3000 ед. обороты начинают изменяться в диапазоне [3000;3050]
                // Поломка возможна только в случае перехвата и изменения пакетов, где обороты будут больше 3200 ед.
                else
                {
                    turbine.currentRPM = (turbine.MaxRPM - 200) + randomValue.Next(0, 51);
                }
            }
            //Если текущие обороты меньше "рабочих", то энергия будет находиться в диапазоне 0-100%
            if (turbine.currentRPM < turbine.MinRPM)
            {
                reactor.energy = (turbine.currentRPM / turbine.MaxRPM) * 100;
            }
            else
            {
                //Если обороты больше максимальных, то начинает расти вибрация
                if (turbine.currentRPM > turbine.MaxRPM)
                {
                    ChangeVibration();
                    reactor.energy = 100;
                }
                else
                {
                    reactor.energy = 100;
                }
            }
            //Если вибрация достигает предела(300 ед.), то происходит поломка
            if (turbine.currentVibration > turbine.MaxVibration)
            {
                turbine.Stop();
                reactor.SendToArduino(Turbine.ArduinoOnOffTurbineMethod, "0");
                OnSiren();
                reactor.energy = 0;
            }
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
            _bus.Send(new Package()
            {
                From = Subject.NuclearStation,
                To = Subject.Substation,
                Method = SubStation.OnSirenMethod,
                Params = Newtonsoft.Json.JsonConvert.SerializeObject(turbine.IsOnSiren),
            });
        }

        /// <summary>
        /// Отправка данных на объект "Электрическая подстанция"
        /// </summary>
        private void SendEnergyForSubStation()
        {
            _bus.Send(new Package()
            {
                From = Subject.NuclearStation,
                To = Subject.Substation,
                Method = SubStation.GetPowerMethod,
                Params = Newtonsoft.Json.JsonConvert.SerializeObject(reactor.energy),
            });
        }

        /// <summary>
        /// Предоставление данных о состоянии атомной станции
        /// </summary>
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
