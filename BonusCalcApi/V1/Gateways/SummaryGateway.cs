using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BonusCalcApi.V1.Gateways
{
    public class SummaryGateway : ISummaryGateway
    {
        private readonly BonusCalcContext _context;

        public SummaryGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<Summary> GetOperativeSummaryAsync(string operativeId, string bonusPeriodId)
        {
            return await _context.Summaries
                .Include(s => s.BonusPeriod)
                .Include(s => s.WeeklySummaries)
                .Where(s => s.OperativeId == operativeId && s.BonusPeriodId == bonusPeriodId)
                .SingleOrDefaultAsync();
        }
    }
}
