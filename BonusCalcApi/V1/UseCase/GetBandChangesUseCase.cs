using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Controllers.Helpers;
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
        private readonly IOperativeHelpers _operativeHelpers;

        public GetBandChangesUseCase(
            IBonusPeriodGateway bonusPeriodGateway,
            IBandChangeGateway bandChangeGateway,
            IOperativeHelpers operativeHelpers
        )
        {
            _bonusPeriodGateway = bonusPeriodGateway;
            _bandChangeGateway = bandChangeGateway;
            _operativeHelpers = operativeHelpers;
        }

        public async Task<IEnumerable<BandChange>> ExecuteAsync(string bonusPeriodId)
        {
            var bonusPeriod = (BonusPeriod) null;

            if (bonusPeriodId is null)
            {
                bonusPeriod = await _bonusPeriodGateway.GetEarliestOpenBonusPeriodAsync();
            }
            else
            {
                if (_operativeHelpers.IsValidDate(bonusPeriodId))
                {
                    bonusPeriod = await _bonusPeriodGateway.GetBonusPeriodAsync(bonusPeriodId);
                }
                else
                {
                    throw new BadRequestException($"Bonus period is invalid - it should be YYYY-MM-DD");
                }
            }

            if (bonusPeriod is null)
            {
                if (bonusPeriodId is null)
                {
                    throw new ResourceNotFoundException($"Bonus period not found for ${bonusPeriodId}");
                }
                else
                {
                    throw new ResourceNotFoundException($"Open bonus period not found");
                }
            }
            else
            {
                return await _bandChangeGateway.GetBandChangesAsync(bonusPeriod.Id);
            }
        }
    }
}
