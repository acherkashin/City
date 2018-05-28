using System.ComponentModel.DataAnnotations;

namespace CyberCity.Models.BankModel
{
    public class Moneytransfer
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Sender { get; set; }
        [Required]
        public int Recipient { get; set; }
        [Required]
        public double Summa { get; set; }
    }
}