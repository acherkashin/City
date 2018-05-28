using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CyberCity.Models.BankModel
{
    public class Resident
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Patronymic { get; set; }
        [Required]
        public int Home { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public double Money { get; set; }
        [Required]
        public double MoneyInCourse { get; set; }
        [Required]
        public double Debt { get; set; }
    }
}