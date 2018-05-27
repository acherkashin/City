using System;
using System.ComponentModel.DataAnnotations;

namespace CyberCity.Models.BankModel
{
    public class NeedOperation
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required]
        public int Sender { get; set; }
        [Required]
        public int Recipient { get; set; }
        [Required]
        public double Money { get; set; }
    }
}