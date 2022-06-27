using System.Threading.Tasks;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IUpdateOperativeReportSentAtUseCase
    {
        public Task ExecuteAsync(string operativeId, string weekId);
    }
}
