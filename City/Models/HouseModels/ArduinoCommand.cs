using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models.HouseModels
{
   public enum ArduinoCommands
    {
        LedOn,
        LedOff,
        LedOnRed,
        LedOnGreen,
        LedOnBlue,
        LedOnRnd
    }

    public static class ArduinoCommand
    {
        public static Dictionary<ArduinoCommands, string> CommandDictionary;

        static ArduinoCommand()
        {
            CommandDictionary = new Dictionary<ArduinoCommands, string>
            {
                {ArduinoCommands.LedOn,"ledOn"},
                { ArduinoCommands.LedOff,"ledOff"},
                { ArduinoCommands.LedOnRed,"ledOnRed"},
                { ArduinoCommands.LedOnGreen,"ledOnGreen"},
                { ArduinoCommands.LedOnBlue,"ledOnBlue"},
                { ArduinoCommands.LedOnRnd,"ledOnRnd"}
            };
        }
    }
}
