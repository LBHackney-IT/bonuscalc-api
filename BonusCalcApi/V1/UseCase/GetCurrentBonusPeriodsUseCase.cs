using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetCurrentBonusPeriodsUseCase : IGetCurrentBonusPeriodsUseCase
    {
        private readonly IBonusPeriodGateway _bonusPeriodGateway;

        public GetCurrentBonusPeriodsUseCase(IBonusPeriodGateway bonusPeriodGateway)
        {
            _bonusPeriodGateway = bonusPeriodGateway;
        }

        public async Task<IEnumerable<BonusPeriod>> ExecuteAsync(DateTime currentDate)
        {
            return await _bonusPeriodGateway.GetCurrentBonusPeriodsAsync(currentDate);
        }
    }
}
