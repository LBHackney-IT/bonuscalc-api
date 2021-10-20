using BonusCalcApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BonusCalcApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/pay")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class PayElementTypeController : BaseController
    {
        private readonly IGetPayElementTypeUseCase _getPaymentUseCase;

        public PayElementTypeController(IGetPayElementTypeUseCase getPaymentUseCase)
        {
            _getPaymentUseCase = getPaymentUseCase;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [Route("type")]
        public async Task<IActionResult> GetPayElementType()

        {
            var payment = await _getPaymentUseCase.ExecuteAsync();
            if (payment is null)
            {
                return Problem(
                    "The requested timesheet is not found",
                    $"/api/v2/payElementTypes",
                    StatusCodes.Status404NotFound, "Not Found"
                    );
            }
            return Ok(payment);
        }
    }
}
