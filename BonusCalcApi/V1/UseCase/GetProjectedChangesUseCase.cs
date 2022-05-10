using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetProjectedChangesUseCase : IGetProjectedChangesUseCase
    {
        private readonly IBonusPeriodGateway _bonusPeriodGateway;
        private readonly IOperativeProjectionGateway _operativeProjectionGateway;

        public GetProjectedChangesUseCase(
            IBonusPeriodGateway bonusPeriodGateway,
            IOperativeProjectionGateway operativeProjectionGateway
        )
        {
            _bonusPeriodGateway = bonusPeriodGateway;
            _operativeProjectionGateway = operativeProjectionGateway;
        }

        public async Task<IEnumerable<OperativeProjection>> ExecuteAsync()
        {
            var bonusPeriod = await _bonusPeriodGateway.GetEarliestOpenBonusPeriod();

            if (bonusPeriod is null)
            {
                throw new ResourceNotFoundException($"Open bonus period not found");
            }
            else
            {
                return await _operativeProjectionGateway.GetAllByBonusPeriodIdAsync(bonusPeriod.Id);
            }
        }
    }
}
