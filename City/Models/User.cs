using CyberCity.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Models
{
    public class User
    {
        public int Id { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Login { get; set; }

        public Subject Subject { get; set; }

        public string Password { get; set; }

        [NotMapped]
        public bool IsOnline => NetHub.OnlineUsersIds.Contains(Id);

        [NotMapped]
        
        public string SubjectName => Subject.GetDisplayName();
    }
}
