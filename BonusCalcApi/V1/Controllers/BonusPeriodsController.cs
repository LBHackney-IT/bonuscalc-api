using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.UseCase.Interfaces;
using Asp.Versioning;

namespace BonusCalcApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/periods")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class BonusPeriodsController : BaseController
    {
        private readonly ICreateBonusPeriodUseCase _createBonusPeriodUseCase;
        private readonly IGetBonusPeriodsUseCase _getBonusPeriodsUseCase;
        private readonly IGetBonusPeriodUseCase _getBonusPeriodUseCase;
        private readonly IGetCurrentBonusPeriodsUseCase _getCurrentBonusPeriodsUseCase;
        private readonly ICloseBonusPeriodUseCase _closeBonusPeriodUseCase;

        public BonusPeriodsController(
            ICreateBonusPeriodUseCase createBonusPeriodUseCase,
            IGetBonusPeriodsUseCase getBonusPeriodsUseCase,
            IGetBonusPeriodUseCase getBonusPeriodUseCase,
            IGetCurrentBonusPeriodsUseCase getCurrentBonusPeriodsUseCase,
            ICloseBonusPeriodUseCase closeBonusPeriodUseCase
        )
        {
            _createBonusPeriodUseCase = createBonusPeriodUseCase;
            _getBonusPeriodsUseCase = getBonusPeriodsUseCase;
            _getBonusPeriodUseCase = getBonusPeriodUseCase;
            _getCurrentBonusPeriodsUseCase = getCurrentBonusPeriodsUseCase;
            _closeBonusPeriodUseCase = closeBonusPeriodUseCase;
        }

        [HttpPost]
        [ProducesResponseType(typeof(BonusPeriodResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBonusPeriod([FromBody] BonusPeriodRequest request)
        {
            try
            {
                var bonusPeriod = await _createBonusPeriodUseCase.ExecuteAsync(request);
                return Ok(bonusPeriod.ToResponse());
            }
            catch (BadRequestException e)
            {
                return Problem(
                    e.Message,
                    $"/api/v1/periods",
                    StatusCodes.Status400BadRequest, "BadRequest"
                );
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<BonusPeriodResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBonusPeriods()
        {
            var bonusPeriods = await _getBonusPeriodsUseCase.ExecuteAsync();
            return Ok(bonusPeriods.Select(bp => bp.ToResponse()).ToList());
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<BonusPeriodResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("current")]
        public async Task<IActionResult> GetCurrentBonusPeriods([FromQuery] DateTime? date)
        {
            var bonusPeriods = await _getCurrentBonusPeriodsUseCase.ExecuteAsync(EnsureValidDate(date));
            return Ok(bonusPeriods.Select(bp => bp.ToResponse()).ToList());
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<BonusPeriodResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("{id}")]
        public async Task<IActionResult> GetBonusPeriod([FromRoute] string id)
        {

            var bonusPeriod = await _getBonusPeriodUseCase.ExecuteAsync(id);

            if (bonusPeriod is null)
            {
                return Problem(
                    "The requested bonus period was not found",
                    $"/api/v1/periods/{id}",
                    StatusCodes.Status404NotFound, "Not Found"
                );
            }

            return Ok(bonusPeriod.ToResponse());
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<BonusPeriodResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("{id}")]
        public async Task<IActionResult> UpdateBonusPeriod([FromRoute] string id, [FromBody] BonusPeriodUpdate request)
        {
            try
            {
                var bonusPeriod = await _closeBonusPeriodUseCase.ExecuteAsync(id, request);
                return Ok(bonusPeriod.ToResponse());
            }
            catch (BadRequestException e)
            {
                return Problem(
                    e.Message,
                    $"/api/v1/periods/{id}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );
            }
            catch (ResourceNotFoundException e)
            {
                return Problem(
                    e.Message,
                    $"/api/v1/periods/{id}",
                    StatusCodes.Status404NotFound, "Not Found"
                );
            }
            catch (ResourceNotProcessableException e)
            {
                return Problem(
                    e.Message,
                    $"/api/v1/periods/{id}",
                    StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity"
                );
            }
        }

        private static DateTime EnsureValidDate(DateTime? date)
        {
            return (date ?? DateTime.UtcNow);
        }
    }
}
