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
        #region Названия методов для Arduino
        /// <summary>
        /// Название метода на Arduino для включения дыма, белого цвета лампы и отключения вентилятора(турбины)
        /// </summary>
        public const string ArduinoBlastMethod = "NuclearBlast";
        /// <summary>
        /// Название метода на Arduino для изменения цвета подсветки
        /// </summary>
        public const string ArduinoLampColorMethod = "ChangeColorLamp";
        #endregion

        public override void ProcessPackage(Package package)
        {
            throw new NotImplementedException();
        }

        private readonly Reactor _reactor = new Reactor();
        private readonly Turbine _turbine = new Turbine();

        public NuclearStation(DataBus bus) : base(bus)
        {   
            _reactor.IsOnReactor = true;
        }

        public void ChangeRodState(bool flag)
        {
            _reactor.IsUpRod = flag;
        }

        public void Work(CityTime time)
        {
            if (!_reactor.NuclearBlast)
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
            if (_reactor.IsUpRod)
            {
                _reactor.currentTemperature = _reactor.currentTemperature - _reactor.dlt;
            }
            else
            {
                _reactor.currentTemperature = _reactor.currentTemperature + _reactor.dlt;
            }
            
            //Тут происходит отправка данных на Arduino Атомной станции для изменения цвета лампочек
            SendToArduino(ArduinoLampColorMethod, Convert.ToString(_reactor.currentTemperature));

            if (_turbine.IsOnTurbine == false)
            {
                _reactor.ChangeDlt(15);
            }
            else
            {
                _reactor.ChangeDlt(5);
            }
            // Если температура >= температуры взрыва, то происходит взрыв(логично...)
            if (_reactor.currentTemperature > _reactor.BlastTemperature)
            {
                _reactor.IsOnReactor = false;
                _turbine.IsOnTurbine = false;
                _turbine.Stop();
                OnSiren();
                _reactor.BlastReactor();

                SendToArduino(ArduinoBlastMethod, "");
            }
            else
            {
                // Если температура меньше или равна 0, то текущая температра = 0
                if (_reactor.currentTemperature <= 0)
                {
                    _reactor.currentTemperature = 0;
                }
                else
                {
                    //При рабочей температуре(от 180) начинают работать турбины
                    if (_reactor.currentTemperature >= _reactor.MinTemperature)
                    {
                        if (_turbine.currentRPM == 0 && _turbine.IsBroken != true)
                        {
                            _turbine.Start();
                        }
                        if (_turbine.IsOnTurbine == true)
                        {
                            ChangeRPM();
                        }
                    }
                    else
                    {
                        if (_turbine.currentRPM < 10)
                        {
                            _turbine.currentRPM = 0;
                            _reactor.energy = (_turbine.currentRPM / _turbine.MaxRPM) * 100;
                        }
                        else
                        {
                            _turbine.currentRPM = _turbine.currentRPM / 2;
                            _reactor.energy = (_turbine.currentRPM / _turbine.MaxRPM) * 100;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Отправка данных на Arduino
        /// </summary>
        /// <param name="Method">Название метода</param>
        /// <param name="p">Параметр</param>

        private void SendToArduino(string method, string p)
        {
            if (GetUser() == null)
                return;

            string url = $"{GetUser().ArduinoUrl}/method?p={p}";
            _bus.SendToArduino(url);
        }

        /// <summary>
        /// Изменение оборотов турбины
        /// </summary>
        private void ChangeRPM()
        {
            Random randomValue = new Random();
            if (_turbine.currentRPM < (_turbine.MaxRPM - 200) && _reactor.IsUpRod == false)
            {
                _turbine.currentRPM = (_reactor.currentTemperature) * 0.05 + _turbine.currentRPM * 1.5;
            }
            else
            {
                if (_turbine.currentRPM < (_turbine.MaxRPM - 200) && _reactor.IsUpRod == true)
                {
                    _turbine.currentRPM = (_reactor.currentTemperature) * 0.05 + _turbine.currentRPM * 1.5;
                }
                // При достижении "рабочих" оборотов в 3000 ед. обороты начинают изменяться в диапазоне [3000;3050]
                // Поломка возможна только в случае перехвата и изменения пакетов, где обороты будут больше 3200 ед.
                else
                {
                    _turbine.currentRPM = (_turbine.MaxRPM - 200) + randomValue.Next(0, 51);
                }
            }
            //Если текущие обороты меньше "рабочих", то энергия будет находиться в диапазоне 0-100%
            if (_turbine.currentRPM < _turbine.MinRPM)
            {
                _reactor.energy = (_turbine.currentRPM / _turbine.MaxRPM) * 100;
            }
            else
            {
                //Если обороты больше максимальных, то начинает расти вибрация
                if (_turbine.currentRPM > _turbine.MaxRPM)
                {
                    ChangeVibration();
                    _reactor.energy = 100;
                }
                else
                {
                    _reactor.energy = 100;
                }
            }
            //Если вибрация достигает предела(300 ед.), то происходит поломка
            if (_turbine.currentVibration > _turbine.MaxVibration)
            {
                _turbine.Stop();

                SendToArduino(Turbine.ArduinoOnOffTurbineMethod, "0");

                OnSiren();
                _reactor.energy = 0;
            }
        }

        /// <summary>
        /// Изменение уровня вибрации
        /// Если турбина поломана, вибрация =0
        /// </summary>

        private void ChangeVibration()
        {
            if (_turbine.IsBroken != true)
            {
                _turbine.currentVibration += 100;
            }
            else
            {
                _turbine.currentVibration = 0;
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
                Params = Newtonsoft.Json.JsonConvert.SerializeObject(_turbine.IsOnSiren),
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
                Params = Newtonsoft.Json.JsonConvert.SerializeObject(_reactor.energy),
            });
        }

        /// <summary>
        /// Предоставление данных о состоянии атомной станции
        /// </summary>
        public ReactorData GetState()
        {
            return new ReactorData()
            {
                Temperature = _reactor.currentTemperature,
                Energy = _reactor.energy,
                RPM = _turbine.currentRPM,
                Vibration = _turbine.currentVibration,
                StReactor = _reactor.IsOnReactor,
                StRod = _reactor.IsUpRod,
                StTurbine = _turbine.IsOnTurbine,
            };
        }
    }
}
