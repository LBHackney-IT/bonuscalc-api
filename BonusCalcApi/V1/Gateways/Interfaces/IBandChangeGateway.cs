using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways.Interfaces
{
    public interface IBandChangeGateway
    {
        public Task<IEnumerable<BandChange>> GetBandChangesAsync(string bonusPeriodId);
    }
}
