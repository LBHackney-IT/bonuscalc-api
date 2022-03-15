using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways.Interfaces
{
    public interface IOutOfHoursSummaryGateway
    {
        public Task<IEnumerable<OutOfHoursSummary>> GetOutOfHoursSummariesAsync(string weekId);
    }
}
