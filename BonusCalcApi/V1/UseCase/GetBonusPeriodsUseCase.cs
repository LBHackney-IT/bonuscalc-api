using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetBonusPeriodsUseCase : IGetBonusPeriodsUseCase
    {
        private readonly IBonusPeriodGateway _bonusPeriodGateway;

        public GetBonusPeriodsUseCase(IBonusPeriodGateway bonusPeriodGateway)
        {
            _bonusPeriodGateway = bonusPeriodGateway;
        }

        public async Task<IEnumerable<BonusPeriod>> ExecuteAsync()
        {
            return await _bonusPeriodGateway.GetBonusPeriodsAsync();
        }
    }
}
