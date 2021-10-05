using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BonusCalcApi.V1.Controllers
{
    [ApiController]

    [Route("api/v1/operatives")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class OperativesController : BaseController
    {
        private readonly IOperativesGateway _operativesGateway;
        public OperativesController(IOperativesGateway operativesGateway)
        {
            _operativesGateway = operativesGateway;
        }

        /// <summary>
        /// Searches RepairsHub for a given operative ID
        /// </summary>
        /// <response code="200">Operative has been found for the specified payroll ID</response>
        /// <response code="400">Bad request sent to controller</response>
        /// <response code="404">No operative found for the specified payroll ID</response>
        /// <response code="500">Server error searching for the operative</response>
        [ProducesResponseType(typeof(OperativeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperativeResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperativeResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(OperativeResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("{operativePayrollNumber}")]
        public IActionResult GetOperative([FromRoute] string operativePayrollNumber)
        {
            if (string.IsNullOrWhiteSpace(operativePayrollNumber))
            {
                return BadRequest("The requested payroll number is invalid");
            }

            return Ok(_operativesGateway.Execute(operativePayrollNumber));
        }
    }
}
