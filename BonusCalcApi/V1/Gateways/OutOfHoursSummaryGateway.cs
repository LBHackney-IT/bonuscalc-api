using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using X.PagedList;

namespace BonusCalcApi.V1.Gateways
{
    public class OutOfHoursSummaryGateway : IOutOfHoursSummaryGateway
    {
        private readonly BonusCalcContext _context;

        public OutOfHoursSummaryGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OutOfHoursSummary>> GetOutOfHoursSummariesAsync(string weekId)
        {
            return await _context.OutOfHoursSummaries
                .Where(os => os.WeekId == weekId)
                .OrderBy(os => os.Id)
                .ToListAsync();
        }
    }
}
