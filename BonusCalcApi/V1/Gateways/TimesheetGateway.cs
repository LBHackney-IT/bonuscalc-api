using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BonusCalcApi.V1.Gateways
{
    public class TimesheetGateway : ITimesheetGateway
    {
        private readonly BonusCalcContext _context;
        public TimesheetGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<Timesheet> GetOperativesTimesheetAsync(string weekId, string operativeId)
        {
            return await _context.Timesheets
                .Include(t => t.Week)
                .ThenInclude(w => w.BonusPeriod)
                .Include(t => t.PayElements)
                .ThenInclude(pe => pe.PayElementType)
                .Include(t => t.Operative)
                .Where(x => x.WeekId == weekId && x.OperativeId == operativeId)
                .SingleOrDefaultAsync();
        }
    }
}
