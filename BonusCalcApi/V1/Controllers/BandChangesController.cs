using System;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BonusCalcApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/band-changes")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class BandChangesController : BaseController
    {
        private readonly IGetBonusPeriodForChangesUseCase _getBonusPeriodForChangesUseCase;
        private readonly IGetProjectedChangesUseCase _getProjectedChangesUseCase;

        public BandChangesController(
            IGetBonusPeriodForChangesUseCase getBonusPeriodForChangesUseCase,
            IGetProjectedChangesUseCase getProjectedChangesUseCase
        )
        {
            _getBonusPeriodForChangesUseCase = getBonusPeriodForChangesUseCase;
            _getProjectedChangesUseCase = getProjectedChangesUseCase;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BonusPeriodResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("period")]
        public async Task<IActionResult> GetBonusPeriod()
        {
            try {
            var bonusPeriod = await _getBonusPeriodForChangesUseCase.ExecuteAsync();
                return Ok(bonusPeriod.ToResponse());
            }
            catch (ResourceNotFoundException)
            {
                return Problem(
                    "There is no open bonus period",
                    $"/api/v1/band-changes/period",
                    StatusCodes.Status404NotFound, "Not Found"
                );
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OperativeProjectionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("projected")]
        public async Task<IActionResult> GetProjectedChanges()
        {
            try
            {
                var projections = await _getProjectedChangesUseCase.ExecuteAsync();
                return Ok(projections.Select(p => p.ToResponse()).ToList());
            }
            catch (ResourceNotFoundException)
            {
                return Problem(
                    "There is no open bonus period",
                    $"/api/v1/band-changes/projected",
                    StatusCodes.Status404NotFound, "Not Found"
                );
            }
        }
    }
}
