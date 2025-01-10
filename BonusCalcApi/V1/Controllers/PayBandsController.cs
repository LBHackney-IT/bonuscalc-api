using Asp.Versioning;
using BonusCalcApi.V1.Boundary.Response;
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
    [Route("api/v1/pay")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class PayBandsController : BaseController
    {
        private readonly IGetSchemesUseCase _getSchemesUseCase;

        public PayBandsController(IGetSchemesUseCase getSchemesUseCase)
        {
            _getSchemesUseCase = getSchemesUseCase;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PayElementTypeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("bands")]
        public async Task<IActionResult> GetPayBands()
        {
            var schemes = await _getSchemesUseCase.ExecuteAsync();
            return Ok(schemes.Select(s => s.ToResponse()).ToList());
        }
    }
}
