using System.Threading.Tasks;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetBonusPeriodForChangesUseCase : IGetBonusPeriodForChangesUseCase
    {
        private readonly IBonusPeriodGateway _bonusPeriodGateway;

        public GetBonusPeriodForChangesUseCase(IBonusPeriodGateway bonusPeriodGateway)
        {
            _bonusPeriodGateway = bonusPeriodGateway;
        }

        public async Task<BonusPeriod> ExecuteAsync()
        {
            var bonusPeriod = await _bonusPeriodGateway.GetEarliestOpenBonusPeriod();

            if (bonusPeriod is null)
            {
                throw new ResourceNotFoundException($"Open bonus period not found");
            }

            return bonusPeriod;
        }
    }
}
