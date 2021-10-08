using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BonusCalcApi.V1.Boundary.Response;
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
        private readonly IGetOperativeUseCase _getOperativeUseCase;
        private static readonly Regex _prnMatcher = new Regex("^[0-9]{6}$");

        public OperativesController(
            IGetOperativeUseCase getOperativeUseCase
        )
        {
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
            if (IsValid(operativePayrollNumber))
            {
                var operative = await _getOperativeUseCase.ExecuteAsync(operativePayrollNumber);

                if (operative is null)
                {
                    return Problem(
                        "The requested payroll number was not found",
                        $"/api/v2/operatives/{operativePayrollNumber}",
                        StatusCodes.Status404NotFound, "Not Found"
                    );
                }
                else
                {
                    return Ok(operative.ToResponse());
                }
            }
            else
            {
                return Problem(
                    "The requested payroll number is invalid",
                    $"/api/v2/operatives/{operativePayrollNumber}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );
            }
        }

        private static bool IsValid(string operativePayrollNumber)
        {
            return !string.IsNullOrWhiteSpace(operativePayrollNumber) && _prnMatcher.IsMatch(operativePayrollNumber);
        }
    }
}
