using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetBandChangesUseCase : IGetBandChangesUseCase
    {
        private readonly IBonusPeriodGateway _bonusPeriodGateway;
        private readonly IBandChangeGateway _bandChangeGateway;

        public GetBandChangesUseCase(
            IBonusPeriodGateway bonusPeriodGateway,
            IBandChangeGateway bandChangeGateway
        )
        {
            _bonusPeriodGateway = bonusPeriodGateway;
            _bandChangeGateway = bandChangeGateway;
        }

        public async Task<IEnumerable<BandChange>> ExecuteAsync()
        {
            var bonusPeriod = await _bonusPeriodGateway.GetEarliestOpenBonusPeriodAsync();

            if (bonusPeriod is null)
            {
                throw new ResourceNotFoundException($"Open bonus period not found");
            }
            else
            {
                return await _bandChangeGateway.GetBandChangesAsync(bonusPeriod.Id);
            }
        }
    }
}
