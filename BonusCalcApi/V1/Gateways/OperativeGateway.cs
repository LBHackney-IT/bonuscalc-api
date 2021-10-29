using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using Microsoft.EntityFrameworkCore;
using BonusCalcApi.V1.Infrastructure;


namespace BonusCalcApi.V1.Gateways
{
    public class OperativeGateway : IOperativeGateway
    {
        private readonly BonusCalcContext _context;

        public OperativeGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<Operative> GetAsync(string operativePayrollNumber)
        {
            return await _context.Operatives
                .Include(o => o.Trade)
                .Include(o => o.Scheme)
                .ThenInclude(s => s.PayBands)
                .SingleOrDefaultAsync(o => o.Id == operativePayrollNumber);
        }
    }
}
