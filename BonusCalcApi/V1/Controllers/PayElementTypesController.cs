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
    public class PayElementTypesController : BaseController
    {
        private readonly IGetPayElementTypeUseCase _getPayElementTypeUseCase;

        public PayElementTypesController(IGetPayElementTypeUseCase getPayElementTypeUseCase)
        {
            _getPayElementTypeUseCase = getPayElementTypeUseCase;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PayElementTypeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("types")]
        public async Task<IActionResult> GetPayElementTypes()
        {
            var payElementTypes = await _getPayElementTypeUseCase.ExecuteAsync();
            return Ok(payElementTypes.Select(pet => pet.ToResponse()).ToList());
        }
    }
}
