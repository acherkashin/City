using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CyberCity.Models.BankModel
{
    public class ValuteOpereation
    {
        [Required]
        public double Summa { get; set; }
    }
}