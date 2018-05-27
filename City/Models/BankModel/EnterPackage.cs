using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CyberCity.Models.BankModel
{
    public class EnterPackage
    {
        [Required]
        public Resident Sender { get; set; }
        [Required]
        public Resident Recipient { get; set; }
        [Required]
        public double Summa { get; set; } 
    }
}