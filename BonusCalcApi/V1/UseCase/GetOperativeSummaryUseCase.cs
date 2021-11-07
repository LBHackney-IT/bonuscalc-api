using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetOperativeSummaryUseCase : IGetOperativeSummaryUseCase
    {
        private readonly ISummaryGateway _summaryGateway;

        public GetOperativeSummaryUseCase(ISummaryGateway summaryGateway)
        {
            _summaryGateway = summaryGateway;
        }

        public async Task<Summary> ExecuteAsync(string operativeId, string bonusPeriodId)
        {
            return await _summaryGateway.GetOperativeSummaryAsync(operativeId, bonusPeriodId);
        }
    }
}
