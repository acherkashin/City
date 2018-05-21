using CyberCity.Models.Reactor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CyberCity.Models.ReactorModel
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
        /// FLAG - глобальная переменная, которая отвечает за текущее состояние стержня, чтобы к ней можно было обращаться из метода StartReactor.
        /// т.е. при нажатии на кнопок на html-странице происходит изменение переменной FLAG, а после к ней обращается метод StartReactor и работает уже с ней
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

                    /// <summary>
                    /// Тут происходит обращение к глобальной переменной и это значение присваивается в текущее состояние стержня
                    /// </summary>
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
                        /// <summary>
                        /// Состояние турбины означает работает она или нет (работает или поломана)
                        /// </summary>
                        StTurbine = t1.StateOfTurbine,
                    });
                    /// TODO: Тут нужно все данные отправлять в базу данных


                    //TODO: для чего нужны переменные, объявленные ниже?
                    /// <summary>
                    /// Эти переменные были нужны для вывода данных на html-страницу. Они конкатенировались и передавались во вьюшку
                    /// А там они выводились в консоль и в соответствующие поля
                    /// </summary>
                    var currentE = r1.energy;
                    var currentR = t1.currentRPM;
                    var currentV = t1.currentVibration;
                    var currentT = r1.currentTemperature;
                    var stTurbine = t1.StateOfTurbine;
                    var stReactor = r1.StateOfRod;
                    var stStation = t1.FlagSiren;
                    Thread.Sleep(60000);
                }
            }
        }

        //TODO: добавить, откуда взяты формулы
        /// <summary>
        /// Формулы придуманы для упрощения работы реактора
        /// При 180 градусах начинают работать турбины. Когда обороты достигают 2500, то энергия начинает вырабатываться на 100%.
        /// Когда обороты достигают 2500, то они начинают изменяться в диапазоне +-50 об. при каждой итерации цикла
        /// Если обороты достигают 3200 и более, то начинает расти вибрация. Когда вибрация достигает 300 ед., то происходит поломка турбины на 150 секунд
        /// При поломке турбины температура начинает расти на 10% быстрее. Т.е. при работающей турбине она растет на 5%, а при поломанной на 15%
        /// </summary>
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
            /// <summary>
            /// Если температура >= температуры взрыва, то происходит взрыв(логично...)
            /// </summary>
            if (r1.currentTemperature > r1.BlastTemperature)
            {
                r1.FlagVoid = true;
                r1.stateOfReactor = false;
                t1.StateOfTurbine = false;
                t1.Stop();
                OnSiren();
                r1.BlastReactor();
            }
            else
            {
                /// <summary>
                /// Если температура меньше или равна 0, то текущая температра=0
                /// </summary>
                if (r1.currentTemperature <= 0)
                {
                    r1.currentTemperature = 0;
                }
                else
                {
                    /// <summary>
                    /// При 180 градусах начинают работать турбины
                    /// </summary>
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
                        /// <summary>
                        /// При 180 градусах начинают работать турбины
                        /// </summary>
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
                /// <summary>
                /// При достижении "рабочих" оборотов в 2500 ед. обороты изменяются на +-50 оборотов
                /// </summary>
                else
                {
                    t1.currentRPM = t1.MaxRPM + randomValue.Next(-1, 2) + randomValue.Next(0, 51);
                }
            }
            /// <summary>
            /// Если текущие обороты меньше "рабочих", то энергия будет находиться в диапазоне 0-100%
            /// </summary>
            if (t1.currentRPM < t1.MinRPM)
            {
                r1.energy = (t1.currentRPM / t1.MaxRPM) * 100;
            }
            else
            {
                /// <summary>
                /// Если обороты больше максимальных, то начинает расти вибрация
                /// </summary>
                if (t1.currentRPM > t1.MaxRPM)
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
            if (t1.currentVibration > t1.MaxVibration)
            {
                t1.Stop();
                OnSiren();
                r1.energy = 0;
            }
        }
        /// <summary>
        /// Изменение энергии на 100% при достижении турбиной "рабочих" оборотов в 2500-3200 ед. 100 означает 100%
        /// </summary>
        private void ChangeEnergy()
        {
            r1.energy = 100;
        }
        /// <summary>
        /// Изменение уровня вибрации
        /// Если турбина поломана, вибрация =0
        /// </summary>
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
        /// <summary>
        /// Включение сирены
        /// </summary>
        private void OnSiren()
        {
            /// <summary>
            /// TODO: Тут необходимо через хаб обращаться к Подстанции и передавать переменную FlagSiren для влючения сирены
            /// </summary>
        }
    }
}
