using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways
{
    public class OvertimeSummaryGateway : IOvertimeSummaryGateway
    {
        private readonly BonusCalcContext _context;

        public OvertimeSummaryGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OvertimeSummary>> GetOvertimeSummariesAsync(string weekId)
        {
            return await _context.OvertimeSummaries
                .Where(os => os.WeekId == weekId)
                .OrderBy(os => os.Id)
                .ToListAsync();
        }
    }
}
