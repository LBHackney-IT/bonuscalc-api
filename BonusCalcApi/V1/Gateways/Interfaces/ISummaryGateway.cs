using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways.Interfaces
{
    public interface ISummaryGateway
    {
        public Task<Summary> GetOperativeSummaryAsync(string operativeId, string bonusPeriodId);
    }
}
