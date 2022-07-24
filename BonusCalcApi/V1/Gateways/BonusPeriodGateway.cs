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

        public async Task<BonusPeriod> CreateBonusPeriodAsync(string id)
        {
            return await _context.BonusPeriods
                .FromSqlRaw("SELECT * FROM create_bonus_period({0})", id)
                .SingleAsync();
        }

        public async Task<BonusPeriod> GetBonusPeriodAsync(string id)
        {
            return await _context.BonusPeriods
                .SingleOrDefaultAsync(bp => bp.Id == id);
        }

        public async Task<BonusPeriod> GetLastBonusPeriodAsync()
        {
            return await _context.BonusPeriods
                .OrderByDescending(bp => bp.StartAt)
                .Take(1)
                .SingleAsync();
        }

        public async Task<IEnumerable<BonusPeriod>> GetBonusPeriodsAsync()
        {
            return await _context.BonusPeriods
                .OrderBy(bp => bp.StartAt)
                .ToListAsync();
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
                .Include(bp => bp.BandChanges.OrderBy(bc => bc.OperativeId))
                .Where(bp => bp.ClosedAt == null)
                .OrderBy(bp => bp.StartAt)
                .FirstOrDefaultAsync();
        }
    }
}
