using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetWeekUseCase : IGetWeekUseCase
    {
        private readonly IWeekGateway _weekGateway;

        public GetWeekUseCase(IWeekGateway weekGateway)
        {
            _weekGateway = weekGateway;
        }

        public async Task<Week> ExecuteAsync(string weekId)
        {
            return await _weekGateway.GetWeekAsync(weekId);
        }
    }
}
