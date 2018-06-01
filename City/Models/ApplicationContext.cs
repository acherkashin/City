using CyberCity.Models.AccountModel;
using CyberCity.Models.BankModel;
using Microsoft.EntityFrameworkCore;

namespace CyberCity.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Package> Packages { get; set; }

        #region Таблицы банка
        public DbSet<Resident> Residents { get; set; }
        public DbSet<Moneytransfer> Moneytransfers { get; set; }
        public DbSet<MoneyContribution> MoneyContributions { get; set; }
        public DbSet<PaymentUtilit> PaymentUtilits { get; set; }
        public DbSet<Salary> Salarys { get; set; }
        public DbSet<Credit> Credits { get; set; }
        public DbSet<NeedOperation> NeedOprerations { get; set; }
        #endregion

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
    }
}
