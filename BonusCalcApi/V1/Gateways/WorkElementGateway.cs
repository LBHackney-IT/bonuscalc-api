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
    public class WorkElementGateway : IWorkElementGateway
    {
        private readonly BonusCalcContext _context;

        public WorkElementGateway(BonusCalcContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkElement>> GetWorkElementsAsync(string query, int? page, int? size)
        {
            int pageNumber = Math.Clamp((page ?? 1), 1, 1000);
            int pageSize = Math.Clamp((size ?? 25), 1, 50);

            return await _context.WorkElements
                .Include(we => we.Week)
                .ThenInclude(w => w.BonusPeriod)
                .Where(we => we.SearchVector.Matches(EF.Functions.PlainToTsQuery("simple", query)))
                .OrderByDescending(we => we.ClosedAt)
                .ToPagedListAsync(pageNumber, pageSize);
        }
    }
}
