using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Request;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IUpdateTimesheetUseCase
    {
        public Task Execute(TimesheetUpdateRequest request, string operativeId, string weekId);
    }
}
