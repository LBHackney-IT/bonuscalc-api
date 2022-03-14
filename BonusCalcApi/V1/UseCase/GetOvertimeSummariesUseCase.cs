using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetOvertimeSummariesUseCase : IGetOvertimeSummariesUseCase
    {
        private readonly IOvertimeSummaryGateway _overtimeSummaryGateway;

        public GetOvertimeSummariesUseCase(IOvertimeSummaryGateway overtimeSummaryGateway)
        {
            _overtimeSummaryGateway = overtimeSummaryGateway;
        }

        public async Task<IEnumerable<OvertimeSummary>> ExecuteAsync(string weekId)
        {
            return await _overtimeSummaryGateway.GetOvertimeSummariesAsync(weekId);
        }
    }
}
