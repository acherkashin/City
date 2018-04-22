using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace City.Models
{
    public class User
    {
        public int Id { get; set; }
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Login { get; set; }

        public Subject Subject { get; set; }

        public string Password { get; set; }
    }
}
