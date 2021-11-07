#nullable enable
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
            var existingTimesheet = await _timesheetGateway.GetOperativeTimesheetAsync(operativeId, weekId);

            if (existingTimesheet is null) ThrowHelper.ThrowNotFound($"Timesheet not found for operative: {operativeId} and week: {weekId}");

            existingTimesheet!.PayElements ??= new List<PayElement>();

            // remove old elements
            existingTimesheet.PayElements.RemoveAll(pe => !ExistsInRequest(request, pe) && !pe.ReadOnly);

            if (request.PayElements != null)
            {
                // update elements
                var payElementsToUpdate = request.PayElements.Where(pe => pe.Id != null);
                foreach (var payElement in payElementsToUpdate)
                {
                    var existingPayElement = existingTimesheet!.PayElements.SingleOrDefault(pe => pe.Id == payElement.Id);

                    if (existingPayElement is null) ThrowHelper.ThrowNotFound($"Pay element not found {payElement.Id}");
                    existingPayElement?.UpdateFrom(payElement);
                }

                // add new elements
                var newElements = request.PayElements?.Where(pe => pe.Id is null).Select(pe => pe.ToDb());
                if (newElements != null) existingTimesheet!.PayElements.AddRange(newElements);
            }

            await _dbSaver.SaveChangesAsync();
        }

        private static bool ExistsInRequest(TimesheetUpdateRequest request, PayElement pe)
        {
            if (request.PayElements is null) return false;

            return request.PayElements.Exists(pe2 => pe2.Id == pe.Id);
        }
    }
}
