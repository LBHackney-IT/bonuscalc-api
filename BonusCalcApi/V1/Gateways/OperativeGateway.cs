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
    public class OperativeGateway : IOperativeGateway
    {
        private readonly BonusCalcContext _context;

        public OperativeGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<Operative> GetOperativeAsync(string operativeId)
        {
            return await _context.Operatives
                .Include(o => o.Trade)
                .Include(o => o.Scheme)
                .ThenInclude(s => s.PayBands)
                .SingleOrDefaultAsync(o => o.Id == operativeId);
        }

        public async Task<IEnumerable<Operative>> GetOperativesAsync(string query, int? page, int? size)
        {
            int pageNumber = Math.Clamp((page ?? 1), 1, 100);
            int pageSize = Math.Clamp((size ?? 25), 1, 50);

            return await _context.Operatives
                .Include(o => o.Trade)
                .Include(o => o.Scheme)
                .ThenInclude(s => s.PayBands)
                .Where(o => o.SearchVector.Matches(query))
                .OrderBy(o => o.Name)
                .ToPagedListAsync(pageNumber, pageSize);
        }
    }
}
