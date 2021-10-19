using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class UpdateTimesheetUseCase : IUpdateTimesheetUseCase
    {
        private readonly ITimesheetGateway _timesheetGateway;
        private readonly IDbSaver _dbSaver;
        public UpdateTimesheetUseCase(ITimesheetGateway timesheetGateway, IDbSaver dbSaver)
        {
            _timesheetGateway = timesheetGateway;
            _dbSaver = dbSaver;
        }
        public async Task Execute(TimesheetUpdateRequest request, string operativeId, string weekId)
        {
            var existingTimesheet = await _timesheetGateway.GetOperativesTimesheetAsync(weekId, operativeId);

            if (existingTimesheet is null) ThrowHelper.ThrowNotFound($"Timesheet not found for operative: {operativeId} and week: {weekId}");

            existingTimesheet.PayElements ??= new List<PayElement>();

            // add new elements
            var newElements = request.PayElements.Where(pe => !existingTimesheet.PayElements.Exists(pe2 => pe2.Id == pe.Id)).Select(pe => pe.ToDb());
            existingTimesheet.PayElements.AddRange(newElements);

            // update elements
            foreach (var payElement in request.PayElements)
            {
                var existingPayElement = existingTimesheet.PayElements?.SingleOrDefault(pe => pe.Id == payElement.Id);
                existingPayElement?.UpdateFrom(payElement);
            }

            // remove old elements
            existingTimesheet.PayElements.RemoveAll(pe => !request.PayElements.Exists(pe2 => pe2.Id == pe.Id));

            await _dbSaver.SaveChangesAsync();
        }
    }
}
