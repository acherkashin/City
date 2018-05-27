using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CyberCity.Models.BankModel
{
    public class PaymentUtilit
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Client { get; set; }
        [Required]
        public double Summa { get; set; }
    }
}