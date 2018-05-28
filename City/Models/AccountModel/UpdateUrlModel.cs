using System.ComponentModel.DataAnnotations;

namespace CyberCity.Models.AccountModel
{
    public class UpdateUrlModel
    {
        [DataType(DataType.Url)]
        public string ArduinoUrl { get; set; }
    }
}
