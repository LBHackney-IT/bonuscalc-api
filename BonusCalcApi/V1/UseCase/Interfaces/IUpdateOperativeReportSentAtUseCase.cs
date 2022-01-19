using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Request;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IUpdateOperativeReportSentAtUseCase
    {
        public Task ExecuteAsync(string operativeId, string weekId);
    }
}
