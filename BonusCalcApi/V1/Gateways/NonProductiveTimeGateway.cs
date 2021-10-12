using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BonusCalcApi.V1.Gateways
{
    public class NonProductiveTimeGateway : INonProductiveTimeGateway
    {
        private readonly BonusCalcContext _context;

        public NonProductiveTimeGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NonProductiveTime>> GetNonProductiveTimeAsync(string operativePayrollNumber)
        {
            var operative = await _context.Operatives.SingleOrDefaultAsync(o => o.Id == operativePayrollNumber);

            return operative.NonProductiveTimeList;
        }
    }
}
