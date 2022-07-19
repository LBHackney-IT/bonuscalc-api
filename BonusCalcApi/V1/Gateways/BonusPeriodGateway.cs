using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BonusCalcApi.V1.Gateways
{
    public class BonusPeriodGateway : IBonusPeriodGateway
    {
        private readonly BonusCalcContext _context;

        public BonusPeriodGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BonusPeriod>> GetCurrentBonusPeriodsAsync(DateTime currentDate)
        {
            return await _context.BonusPeriods
                .Include(bp => bp.Weeks.OrderBy(w => w.StartAt))
                .Where(bp => bp.ClosedAt == null && bp.StartAt < currentDate)
                .OrderBy(bp => bp.StartAt)
                .ToListAsync();
        }

        public async Task<BonusPeriod> GetEarliestOpenBonusPeriodAsync()
        {
            return await _context.BonusPeriods
                .Include(bp => bp.Weeks.OrderBy(w => w.StartAt))
                .Where(bp => bp.ClosedAt == null)
                .OrderBy(bp => bp.StartAt)
                .FirstOrDefaultAsync();
        }
    }
}
