using CyberCity.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberCity.Models.AccountModel
{
    public class User
    {
        public int Id { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Login { get; set; }

        public Subject Subject { get; set; }

        public string Password { get; set; }

        [Url]
        public string ArduinoUrl { get; set; }

        [NotMapped]
        public bool IsOnline => NetHub.OnlineUsersIds.Contains(Id);

        [NotMapped]

        public string SubjectName => Subject.GetDisplayName();
    }
}
