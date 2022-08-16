using System;
using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Controllers.Helpers;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class CloseBonusPeriodUseCase : ICloseBonusPeriodUseCase
    {
        private readonly IBonusPeriodGateway _bonusPeriodGateway;
        private readonly IBandChangeGateway _bandChangeGateway;
        private readonly IWeekGateway _weekGateway;
        private readonly IOperativeHelpers _operativeHelpers;

        public CloseBonusPeriodUseCase(
            IBonusPeriodGateway bonusPeriodGateway,
            IBandChangeGateway bandChangeGateway,
            IWeekGateway weekGateway,
            IOperativeHelpers operativeHelpers
        )
        {
            _bonusPeriodGateway = bonusPeriodGateway;
            _bandChangeGateway = bandChangeGateway;
            _weekGateway = weekGateway;
            _operativeHelpers = operativeHelpers;
        }

        public async Task<BonusPeriod> ExecuteAsync(string bonusPeriodId, BonusPeriodUpdate request)
        {
            if (!_operativeHelpers.IsValidDate(bonusPeriodId))
            {
                throw new BadRequestException($"Bonus period is invalid - it should be YYYY-MM-DD");
            }

            var bonusPeriod = await _bonusPeriodGateway.GetBonusPeriodAsync(bonusPeriodId);
            var payElementTypeId = int.Parse(Environment.GetEnvironmentVariable("BBF_PAY_ELEMENT_TYPE_ID") ?? "202");

            if (bonusPeriod is null)
            {
                throw new ResourceNotFoundException($"Bonus period not found");
            }

            if (bonusPeriod.IsClosed)
            {
                throw new ResourceNotProcessableException($"Bonus period is already closed");
            }

            var openWeeks = await _weekGateway.CountOpenWeeksAsync(bonusPeriod.Id);

            if (openWeeks > 0)
            {
                throw new ResourceNotProcessableException($"Bonus period still has open weeks");
            }

            var remainingBandChanges = await _bandChangeGateway.CountRemainingBandChangesAsync(bonusPeriod.Id);

            if (remainingBandChanges > 0)
            {
                throw new ResourceNotProcessableException($"Bonus period still has band changes that have not been processed");
            }

            return await _bonusPeriodGateway.CloseBonusPeriodAsync(bonusPeriod.Id, payElementTypeId, request.ClosedAt, request.ClosedBy);
        }
    }
}
