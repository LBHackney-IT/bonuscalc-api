using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways
{
    public class PayElementTypeGateway : IPayElementTypeGateway
    {
        private readonly BonusCalcContext _context;

        public PayElementTypeGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PayElementType>> GetPayElementTypesAsync()
        {
            return await _context.PayElementTypes.OrderBy(pet => pet.Id).ToListAsync();
        }
    }
}
