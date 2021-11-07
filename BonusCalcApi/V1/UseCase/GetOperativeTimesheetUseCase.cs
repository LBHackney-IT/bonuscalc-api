using System.Threading.Tasks;
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

        public async Task<Timesheet> ExecuteAsync(string operativeId, string weekId)
        {
            return await _timesheetGateway.GetOperativeTimesheetAsync(operativeId, weekId);
        }
    }
}
