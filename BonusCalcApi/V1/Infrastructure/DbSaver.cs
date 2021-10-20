using System.Threading.Tasks;

namespace BonusCalcApi.V1.Infrastructure
{
    public class DbSaver : IDbSaver
    {
        private readonly BonusCalcContext _context;
        public DbSaver(BonusCalcContext context)
        {
            _context = context;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
