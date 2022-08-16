using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways.Interfaces
{
    public interface IBandChangeGateway
    {
        public Task<IEnumerable<BandChange>> GetBandChangesAsync(string bonusPeriodId);
        public Task<IEnumerable<BandChange>> GetBandChangeAuthorisationsAsync(string bonusPeriodId);
        public Task<int> CountRemainingBandChangesAsync(string bonusPeriodId);
        public Task<BandChange> GetBandChangeAsync(string bonusPeriodId, string operativeId);
    }
}
