using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetBonusPeriodUseCase : IGetBonusPeriodUseCase
    {
        private readonly IBonusPeriodGateway _bonusPeriodGateway;

        public GetBonusPeriodUseCase(IBonusPeriodGateway bonusPeriodGateway)
        {
            _bonusPeriodGateway = bonusPeriodGateway;
        }

        public async Task<BonusPeriod> ExecuteAsync(string id)
        {
            return await _bonusPeriodGateway.GetBonusPeriodIncludingWeeksAsync(id);
        }
    }
}
