using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetOperativeTimesheetUseCase : IGetOperativeTimesheetUseCase
    {
        private readonly ITimesheetGateway _timesheetGateway;
        public GetOperativeTimesheetUseCase(ITimesheetGateway timesheetGateway)
        {
            _timesheetGateway = timesheetGateway;
        }

        public Task<Timesheet> Execute(string weekId, string operativeId)
        {
            return _timesheetGateway.GetOperativesTimesheetAsync(weekId, operativeId);
        }
    }
}
