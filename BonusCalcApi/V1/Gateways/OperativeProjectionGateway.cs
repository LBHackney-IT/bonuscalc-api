using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways
{
    public class OperativeProjectionGateway : IOperativeProjectionGateway
    {
        private readonly BonusCalcContext _context;

        public OperativeProjectionGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OperativeProjection>> GetAllByBonusPeriodIdAsync(string bonusPeriodId)
        {
            return await _context.OperativeProjections
                .Include(op => op.Operative)
                .Include(op => op.BonusPeriod)
                .Where(op => op.BonusPeriodId == bonusPeriodId)
                .OrderBy(op => op.OperativeId)
                .ToListAsync();
        }
    }
}
