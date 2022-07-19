using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Request;

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
        private readonly IStartBandChangeProcessUseCase _startBandChangeProcessUseCase;
        private readonly IGetBandChangesUseCase _getBandChangesUseCase;
        private readonly ISupervisorBandDecisionUseCase _supervisorBandDecisionUseCase;
        private readonly IManagerBandDecisionUseCase _managerBandDecisionUseCase;

        public BandChangesController(
            IGetBonusPeriodForChangesUseCase getBonusPeriodForChangesUseCase,
            IGetProjectedChangesUseCase getProjectedChangesUseCase,
            IStartBandChangeProcessUseCase startBandChangeProcessUseCase,
            IGetBandChangesUseCase getBandChangesUseCase,
            ISupervisorBandDecisionUseCase supervisorBandDecisionUseCase,
            IManagerBandDecisionUseCase managerBandDecisionUseCase
        )
        {
            _getBonusPeriodForChangesUseCase = getBonusPeriodForChangesUseCase;
            _getProjectedChangesUseCase = getProjectedChangesUseCase;
            _startBandChangeProcessUseCase = startBandChangeProcessUseCase;
            _getBandChangesUseCase = getBandChangesUseCase;
            _supervisorBandDecisionUseCase = supervisorBandDecisionUseCase;
            _managerBandDecisionUseCase = managerBandDecisionUseCase;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BonusPeriodResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("period")]
        public async Task<IActionResult> GetBonusPeriod()
        {
            try
            {
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

        [HttpPost]
        [ProducesResponseType(typeof(BonusPeriodResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("start")]
        public async Task<IActionResult> StartBandChanges()
        {
            try
            {
                var bonusPeriod = await _startBandChangeProcessUseCase.ExecuteAsync();
                return Ok(bonusPeriod.ToResponse());
            }
            catch (ResourceNotFoundException)
            {
                return Problem(
                    "There is no open bonus period",
                    $"/api/v1/band-changes/start",
                    StatusCodes.Status404NotFound, "Not Found"
                );
            }
            catch (ResourceNotProcessableException e)
            {
                return Problem(
                    e.Message,
                    $"/api/v1/band-changes/start",
                    StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity"
                );
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<BandChangeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBandChanges()
        {
            try
            {
                var bandChanges = await _getBandChangesUseCase.ExecuteAsync();
                return Ok(bandChanges.Select(bc => bc.ToResponse()).ToList());
            }
            catch (ResourceNotFoundException)
            {
                return Problem(
                    "There is no open bonus period",
                    $"/api/v1/band-changes",
                    StatusCodes.Status404NotFound, "Not Found"
                );
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BandChangeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("{operativeId}/supervisor")]
        public async Task<IActionResult> SupervisorBandDecision([FromRoute][Required] string operativeId, [FromBody] BandChangeRequest request)
        {
            try
            {
                var bandChange = await _supervisorBandDecisionUseCase.ExecuteAsync(operativeId, request);
                return Ok(bandChange.ToResponse());
            }
            catch (ResourceNotFoundException e)
            {
                return Problem(
                    e.Message,
                    $"/api/v1/band-changes/{operativeId}/supervisor",
                    StatusCodes.Status404NotFound, "Not Found"
                );
            }
            catch (ResourceNotProcessableException e)
            {
                return Problem(
                    e.Message,
                    $"/api/v1/band-changes/{operativeId}/supervisor",
                    StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity"
                );
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BandChangeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("{operativeId}/manager")]
        public async Task<IActionResult> ManagerBandDecision([FromRoute][Required] string operativeId, [FromBody] BandChangeRequest request)
        {
            try
            {
                var bandChange = await _managerBandDecisionUseCase.ExecuteAsync(operativeId, request);
                return Ok(bandChange.ToResponse());
            }
            catch (ResourceNotFoundException e)
            {
                return Problem(
                    e.Message,
                    $"/api/v1/band-changes/{operativeId}/manager",
                    StatusCodes.Status404NotFound, "Not Found"
                );
            }
            catch (ResourceNotProcessableException e)
            {
                return Problem(
                    e.Message,
                    $"/api/v1/band-changes/{operativeId}/manager",
                    StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity"
                );
            }
        }
    }
}
