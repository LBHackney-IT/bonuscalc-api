using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Controllers.Helpers;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/weeks")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class WeeksController : BaseController
    {
        private readonly IOperativeHelpers _operativeHelpers;
        private readonly IGetWeekUseCase _getWeekUseCase;
        private readonly IUpdateWeekUseCase _updateWeekUseCase;
        private readonly IUpdateWeekReportsSentAtUseCase _updateWeekReportsSentAtUseCase;
        private readonly IGetOutOfHoursSummariesUseCase _getOutOfHoursSummariesUseCase;
        private readonly IGetOvertimeSummariesUseCase _getOvertimeSummariesUseCase;

        public WeeksController(
            IOperativeHelpers operativeHelpers,
            IGetWeekUseCase getWeekUseCase,
            IUpdateWeekUseCase updateWeekUseCase,
            IUpdateWeekReportsSentAtUseCase updateWeekReportsSentAtUseCase,
            IGetOutOfHoursSummariesUseCase getOutOfHoursSummariesUseCase,
            IGetOvertimeSummariesUseCase getOvertimeSummariesUseCase
        )
        {
            _operativeHelpers = operativeHelpers;
            _getWeekUseCase = getWeekUseCase;
            _updateWeekUseCase = updateWeekUseCase;
            _updateWeekReportsSentAtUseCase = updateWeekReportsSentAtUseCase;
            _getOutOfHoursSummariesUseCase = getOutOfHoursSummariesUseCase;
            _getOvertimeSummariesUseCase = getOvertimeSummariesUseCase;
        }

        [HttpGet]
        [ProducesResponseType(typeof(WeekResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("{weekId}")]
        public async Task<IActionResult> GetWeek([FromRoute][Required] string weekId)
        {
            if (!IsValidDate(weekId))
                return Problem(
                    "The requested week is invalid",
                    $"/api/v1/weeks/{weekId}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            var week = await _getWeekUseCase.ExecuteAsync(weekId);

            if (week is null)
            {
                return Problem(
                    "The requested week was not found",
                    $"/api/v1/weeks/{weekId}",
                    StatusCodes.Status404NotFound, "Not Found"
                );
            }

            return Ok(week.ToResponse());
        }

        [HttpPost]
        [ProducesResponseType(typeof(WeekResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("{weekId}")]
        public async Task<IActionResult> UpdateWeek([FromRoute][Required] string weekId, [FromBody] WeekUpdate weekUpdate)
        {
            if (!IsValidDate(weekId))
                return Problem(
                    "The requested week is invalid",
                    $"/api/v1/weeks/{weekId}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            var week = await _updateWeekUseCase.ExecuteAsync(weekId, weekUpdate);

            if (week is null)
            {
                return Problem(
                    "The requested week was not found",
                    $"/api/v1/weeks/{weekId}",
                    StatusCodes.Status404NotFound, "Not Found"
                );
            }

            return Ok(week.ToResponse());
        }

        [HttpPost]
        [Route("{weekId}/reports")]
        public async Task<IActionResult> UpdateReportsSentAt([FromRoute][Required] string weekId)
        {
            if (!IsValidDate(weekId))
                return Problem(
                    "The requested week is invalid",
                    $"/api/v1/weeks/{weekId}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            await _updateWeekReportsSentAtUseCase.ExecuteAsync(weekId);

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OutOfHoursSummaryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("{weekId}/out-of-hours")]
        public async Task<IActionResult> GetOutOfHoursSummaries([FromRoute][Required] string weekId)
        {
            if (!IsValidDate(weekId))
                return Problem(
                    "The requested week is invalid",
                    $"/api/v1/weeks/{weekId}/out-of-hours",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            var outOfHoursSummaries = await _getOutOfHoursSummariesUseCase.ExecuteAsync(weekId);
            return Ok(outOfHoursSummaries.Select(os => os.ToResponse()).ToList());
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OvertimeSummaryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("{weekId}/overtime")]
        public async Task<IActionResult> GetOvertimeSummaries([FromRoute][Required] string weekId)
        {
            if (!IsValidDate(weekId))
                return Problem(
                    "The requested week is invalid",
                    $"/api/v1/weeks/{weekId}/overtime",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            var overtimeSummaries = await _getOvertimeSummariesUseCase.ExecuteAsync(weekId);
            return Ok(overtimeSummaries.Select(os => os.ToResponse()).ToList());
        }

        private bool IsValidDate(string date)
        {
            return _operativeHelpers.IsValidDate(date);
        }
    }
}
