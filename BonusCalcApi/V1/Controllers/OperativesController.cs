using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Controllers.Helpers;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/operatives")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class OperativesController : BaseController
    {
        private readonly IOperativeHelpers _operativeHelpers;
        private readonly IGetOperativeUseCase _getOperativeUseCase;
        private readonly IGetOperativesUseCase _getOperativesUseCase;
        private readonly IGetOperativeSummaryUseCase _getOperativeSummaryUseCase;
        private readonly IGetOperativeTimesheetUseCase _getOperativeTimesheetUseCase;
        private readonly IUpdateTimesheetUseCase _updateTimesheetUseCase;

        public OperativesController(
            IOperativeHelpers operativeHelpers,
            IGetOperativeUseCase getOperativeUseCase,
            IGetOperativesUseCase getOperativesUseCase,
            IGetOperativeSummaryUseCase getOperativeSummaryUseCase,
            IGetOperativeTimesheetUseCase getOperativeTimesheetUseCase,
            IUpdateTimesheetUseCase updateTimesheetUseCase
        )
        {
            _operativeHelpers = operativeHelpers;
            _getOperativeUseCase = getOperativeUseCase;
            _getOperativesUseCase = getOperativesUseCase;
            _getOperativeSummaryUseCase = getOperativeSummaryUseCase;
            _getOperativeTimesheetUseCase = getOperativeTimesheetUseCase;
            _updateTimesheetUseCase = updateTimesheetUseCase;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OperativeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOperatives([FromQuery][Required] string query, [FromQuery] int? page, [FromQuery] int? size)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Problem(
                    "The requested query is missing or invalid",
                    $"/api/v1/operatives?query={query}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            var operatives = await _getOperativesUseCase.ExecuteAsync(query, page, size);
            return Ok(operatives.Select(o => o.ToResponse()).ToList());
        }

        /// <summary>
        /// Searches RepairsHub for a given operative ID
        /// </summary>
        /// <response code="200">Operative has been found for the specified payroll ID</response>
        /// <response code="400">Bad request sent to controller</response>
        /// <response code="404">No operative found for the specified payroll ID</response>
        /// <response code="500">Server error searching for the operative</response>
        [HttpGet]
        [ProducesResponseType(typeof(OperativeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("{operativePayrollNumber}")]
        public async Task<IActionResult> GetOperative([FromRoute][Required] string operativePayrollNumber)
        {
            if (!IsValidPrn(operativePayrollNumber))
                return Problem(
                    "The requested payroll number is invalid",
                    $"/api/v1/operatives/{operativePayrollNumber}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            var operative = await _getOperativeUseCase.ExecuteAsync(operativePayrollNumber);

            if (operative is null)
            {
                return Problem(
                    "The requested payroll number was not found",
                    $"/api/v1/operatives/{operativePayrollNumber}",
                    StatusCodes.Status404NotFound, "Not Found"
                );
            }

            return Ok(operative.ToResponse());
        }

        [HttpGet]
        [ProducesResponseType(typeof(SummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [Route("{operativePayrollNumber}/summary")]
        public async Task<IActionResult> GetSummary([FromRoute][Required] string operativePayrollNumber, [FromQuery][Required] string bonusPeriod)
        {
            if (!IsValidPrn(operativePayrollNumber))
                return Problem(
                    "The requested payroll number is invalid",
                    $"/api/v1/operatives/{operativePayrollNumber}/summary?bonusPeriod={bonusPeriod}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            if (!IsValidDate(bonusPeriod))
                return Problem(
                    "The requested bonus period is invalid",
                    $"/api/v1/operatives/{operativePayrollNumber}/summary?bonusPeriod={bonusPeriod}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            var summary = await _getOperativeSummaryUseCase.ExecuteAsync(operativePayrollNumber, bonusPeriod);

            if (summary is null)
            {
                return Problem(
                    "The requested summary is not found",
                    $"/api/v1/operatives/{operativePayrollNumber}/summary?bonusPeriod={bonusPeriod}",
                    StatusCodes.Status404NotFound, "Not Found"
                );
            }

            return Ok(summary.ToResponse());
        }

        [HttpGet]
        [ProducesResponseType(typeof(TimesheetResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [Route("{operativePayrollNumber}/timesheet")]
        public async Task<IActionResult> GetTimesheet([FromRoute][Required] string operativePayrollNumber, [FromQuery][Required] string week)
        {
            if (!IsValidPrn(operativePayrollNumber))
                return Problem(
                    "The requested payroll number is invalid",
                    $"/api/v1/operatives/{operativePayrollNumber}/timesheet?week={week}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            if (!IsValidDate(week))
                return Problem(
                    "The requested week is invalid",
                    $"/api/v1/operatives/{operativePayrollNumber}/timesheet?week={week}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            var timesheet = await _getOperativeTimesheetUseCase.ExecuteAsync(operativePayrollNumber, week);

            if (timesheet is null)
            {
                return Problem(
                    "The requested timesheet is not found",
                    $"/api/v1/operatives/{operativePayrollNumber}/timesheet?week={week}",
                    StatusCodes.Status404NotFound, "Not Found"
                );
            }

            return Ok(timesheet.ToResponse());
        }

        [HttpPost]
        [Route("{operativePayrollNumber}/timesheet")]
        public async Task<IActionResult> UpdateTimesheet([FromBody] TimesheetUpdateRequest updateRequest, [FromRoute][Required] string operativePayrollNumber, [FromQuery][Required] string week)
        {
            if (!IsValidPrn(operativePayrollNumber))
                return Problem(
                    "The requested payroll number is invalid",
                    $"/api/v1/operatives/{operativePayrollNumber}/timesheet?week={week}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            if (!IsValidDate(week))
                return Problem(
                    "The requested week is invalid",
                    $"/api/v1/operatives/{operativePayrollNumber}/timesheet?week={week}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            await _updateTimesheetUseCase.ExecuteAsync(updateRequest, operativePayrollNumber, week);

            return Ok();
        }

        private bool IsValidPrn(string operativePayrollNumber)
        {
            return _operativeHelpers.IsValidPrn(operativePayrollNumber);
        }

        private bool IsValidDate(string date)
        {
            return _operativeHelpers.IsValidDate(date);
        }
    }
}
