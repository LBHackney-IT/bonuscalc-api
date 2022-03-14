
using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetOutOfHoursSummariesUseCase : IGetOutOfHoursSummariesUseCase
    {
        private readonly IOutOfHoursSummaryGateway _outOfHoursSummaryGateway;

        public GetOutOfHoursSummariesUseCase(IOutOfHoursSummaryGateway outOfHoursSummaryGateway)
        {
            _outOfHoursSummaryGateway = outOfHoursSummaryGateway;
        }

        public async Task<IEnumerable<OutOfHoursSummary>> ExecuteAsync(string weekId)
        {
            return await _outOfHoursSummaryGateway.GetOutOfHoursSummariesAsync(weekId);
        }
    }
}
