using System.Collections.Generic;
using System.Linq;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static OperativeResponse ToResponse(this Operative operative)
        {
            return new OperativeResponse
            {
                Id = operative.Id,
                Name = operative.Name,
                Trade = operative.Trade,
                Section = operative.Section,
                Scheme = operative.Scheme,
                SalaryBand = operative.SalaryBand,
                FixedBand = operative.FixedBand,
                IsArchived = operative.IsArchived
            };
        }
    }
}
