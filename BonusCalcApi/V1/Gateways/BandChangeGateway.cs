using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BonusCalcApi.V1.Gateways
{
    public class BandChangeGateway : IBandChangeGateway
    {
        private readonly BonusCalcContext _context;

        public BandChangeGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BandChange>> GetBandChangesAsync(string bonusPeriodId)
        {
            return await _context.BandChanges
                .Include(bc => bc.Operative)
                .Include(bc => bc.BonusPeriod)
                .Where(bc => bc.BonusPeriodId == bonusPeriodId)
                .OrderBy(bc => bc.OperativeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BandChange>> GetBandChangeAuthorisationsAsync(string bonusPeriodId)
        {
            return await _context.BandChanges
                .Include(bc => bc.Operative)
                .Include(bc => bc.BonusPeriod)
                .Where(bc => bc.BonusPeriodId == bonusPeriodId)
                .Where(bc => bc.Supervisor.Decision == BandChangeDecision.Rejected)
                .Where(bc => bc.Supervisor.SalaryBand > bc.ProjectedBand)
                .OrderBy(bc => bc.OperativeId)
                .ToListAsync();
        }

        public async Task<int> CountRemainingBandChangesAsync(string bonusPeriodId)
        {
            return await _context.BandChanges
                .Where(bc => bc.BonusPeriodId == bonusPeriodId)
                .Where(bc => bc.FinalBand == null)
                .CountAsync();
        }

        public async Task<BandChange> GetBandChangeAsync(string bonusPeriodId, string operativeId)
        {
            return await _context.BandChanges
                .Include(bc => bc.Operative)
                .Include(bc => bc.BonusPeriod)
                .Include(bc => bc.WeeklySummaries.OrderBy(ws => ws.Number))
                .Where(bc => bc.BonusPeriodId == bonusPeriodId)
                .Where(bc => bc.OperativeId == operativeId)
                .SingleOrDefaultAsync();
        }
    }
}
