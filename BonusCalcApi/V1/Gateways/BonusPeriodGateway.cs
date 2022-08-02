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

        public async Task<BonusPeriod> CloseBonusPeriodAsync(string id, int payElementTypeId, DateTime closedAt, string closedBy)
        {
            // At this point the use case has a tracked copy of the bonus period we're
            // closing using the database function. This means that it will not see the
            // changes returned by this query below. Since this DbContext object is about
            // to be destroyed when the request ends we clear all the tracked entities so
            // that the response includes the correct closed_at and closed_by values.
            _context.ChangeTracker.Clear();

            return await _context.BonusPeriods
                .FromSqlRaw("SELECT * FROM close_bonus_period({0}, {1}, {2}, {3})", id, payElementTypeId, closedAt, closedBy)
                .SingleAsync();
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
