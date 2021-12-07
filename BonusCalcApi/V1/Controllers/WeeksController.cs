using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Controllers.Helpers;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Infrastructure;
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

        public WeeksController(
            IOperativeHelpers operativeHelpers,
            IGetWeekUseCase getWeekUseCase
        )
        {
            _operativeHelpers = operativeHelpers;
            _getWeekUseCase = getWeekUseCase;
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

        private bool IsValidDate(string date)
        {
            return _operativeHelpers.IsValidDate(date);
        }
    }
}
