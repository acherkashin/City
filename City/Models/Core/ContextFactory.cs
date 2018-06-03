using Microsoft.EntityFrameworkCore;

namespace CyberCity.Models.Core
{
    public static class ContextFactory
    {
        /// <summary>
        /// Для надежной работы в многопоточной среде нельзя использовать Singletion контекста EntityFramework'а.
        /// Необходимо на каждый запрос создавать новый экземпляр.
        /// </summary>
        public static ApplicationContext GetContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseSqlite("Data Source=city.db");
            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}
