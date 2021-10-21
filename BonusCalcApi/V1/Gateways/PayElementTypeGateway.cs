using BonusCalcApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BonusCalcApi.V1.Gateways
{
    public class PayElementTypeGateway : IPayElementTypesGateway
    {
        private readonly BonusCalcContext _context;
        public PayElementTypeGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PayElementType>> GetPayElementTypesAsync()
        {
            return await _context.PayElementTypes.ToListAsync();
        }
    }
}
