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

        public async Task<PayElementType> GetPayTypesAsync(string weekId)
        {
            var d = weekId;
            return await _context.PayElementTypes
                //.Where(x => x. == weekId)
                .SingleOrDefaultAsync();
        }
    }
}
