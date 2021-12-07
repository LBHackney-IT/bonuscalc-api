using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways
{
    public class SchemeGateway : ISchemeGateway
    {
        private readonly BonusCalcContext _context;

        public SchemeGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Scheme>> GetSchemesAsync()
        {
            return await _context.Schemes
                .Include(s => s.PayBands.OrderBy(pb => pb.Band))
                .OrderBy(s => s.Id)
                .ToListAsync();
        }
    }
}
