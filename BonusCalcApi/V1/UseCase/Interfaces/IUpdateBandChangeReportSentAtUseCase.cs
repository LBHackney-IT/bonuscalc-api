using System.Threading.Tasks;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IUpdateBandChangeReportSentAtUseCase
    {
        public Task ExecuteAsync(string operativeId);
    }
}
