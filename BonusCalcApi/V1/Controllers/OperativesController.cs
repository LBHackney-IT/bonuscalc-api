using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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

        public OperativesController(
            IOperativeHelpers operativeHelpers,
            IGetOperativeUseCase getOperativeUseCase
        )
        {
            _operativeHelpers = operativeHelpers;
            _getOperativeUseCase = getOperativeUseCase;
        }

        /// <summary>
        /// Searches RepairsHub for a given operative ID
        /// </summary>
        /// <response code="200">Operative has been found for the specified payroll ID</response>
        /// <response code="400">Bad request sent to controller</response>
        /// <response code="404">No operative found for the specified payroll ID</response>
        /// <response code="500">Server error searching for the operative</response>
        [ProducesResponseType(typeof(OperativeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("{operativePayrollNumber}")]
        public async Task<IActionResult> GetOperative([FromRoute][Required] string operativePayrollNumber)
        {
            if (!IsValid(operativePayrollNumber))
                return Problem(
                    "The requested payroll number is invalid",
                    $"/api/v2/operatives/{operativePayrollNumber}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            var operative = await _getOperativeUseCase.ExecuteAsync(operativePayrollNumber);

            if (operative is null)
            {
                return Problem(
                    "The requested payroll number was not found",
                    $"/api/v2/operatives/{operativePayrollNumber}",
                    StatusCodes.Status404NotFound, "Not Found"
                );
            }
            return Ok(operative.ToResponse());
        }

        [HttpGet]
        [Route("{operativePayrollNumber}/time/unproductive")]
        public async Task<IActionResult> GetNonProductiveTime([FromRoute][Required] string operativePayrollNumber)
        {
            if (!IsValid(operativePayrollNumber))
                return Problem(
                    "The requested payroll number is invalid",
                    $"/api/v2/operatives/{operativePayrollNumber}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            return await Task.FromResult(BadRequest($"not implemented {operativePayrollNumber}"));

        }

        private bool IsValid(string operativePayrollNumber)
        {
            return _operativeHelpers.IsValidPrn(operativePayrollNumber);
        }
    }
}
