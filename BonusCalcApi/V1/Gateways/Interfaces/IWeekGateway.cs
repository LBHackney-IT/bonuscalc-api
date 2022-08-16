using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways.Interfaces
{
    public interface IWeekGateway
    {
        public Task<Week> GetWeekAsync(string weekId);
        public Task<int> CountOpenWeeksAsync(string bonusPeriodId);
    }
}
