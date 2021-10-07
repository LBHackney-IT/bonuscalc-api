using Microsoft.EntityFrameworkCore;

namespace BonusCalcApi.V1.Infrastructure
{

    public class BonusCalcContext : DbContext
    {
        public BonusCalcContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<DatabaseEntity> DatabaseEntities { get; set; }
    }
}
