using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways
{
    public class WeekGateway : IWeekGateway
    {
        private readonly BonusCalcContext _context;

        public WeekGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<Week> GetWeekAsync(string weekId)
        {
            return await _context.Weeks
                .Include(w => w.BonusPeriod)
                .Include(w => w.OperativeSummaries.OrderBy(os => os.Id))
                .SingleOrDefaultAsync(w => w.Id == weekId);
        }
    }
}
