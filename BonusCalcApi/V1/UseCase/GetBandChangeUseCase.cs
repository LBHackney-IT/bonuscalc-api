using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetBandChangeUseCase : IGetBandChangeUseCase
    {
        private readonly IBonusPeriodGateway _bonusPeriodGateway;
        private readonly IBandChangeGateway _bandChangeGateway;

        public GetBandChangeUseCase(
            IBonusPeriodGateway bonusPeriodGateway,
            IBandChangeGateway bandChangeGateway
        )
        {
            _bonusPeriodGateway = bonusPeriodGateway;
            _bandChangeGateway = bandChangeGateway;
        }

        public async Task<BandChange> ExecuteAsync(string operativeId)
        {
            var bonusPeriod = await _bonusPeriodGateway.GetEarliestOpenBonusPeriodAsync();

            if (bonusPeriod is null)
            {
                throw new ResourceNotFoundException($"Open bonus period not found");
            }

            var bandChange = await _bandChangeGateway.GetBandChangeAsync(bonusPeriod.Id, operativeId);

            if (bandChange is null)
            {
                throw new ResourceNotFoundException($"Band change not found");
            }

            return bandChange;
        }
    }
}
