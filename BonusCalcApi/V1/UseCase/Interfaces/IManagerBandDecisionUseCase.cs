using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IManagerBandDecisionUseCase
    {
        public Task<BandChange> ExecuteAsync(string operativeId, BandChangeRequest request);
    }
}
