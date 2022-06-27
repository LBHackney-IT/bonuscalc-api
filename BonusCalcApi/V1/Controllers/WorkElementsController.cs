using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BonusCalcApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/work")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class WorkElementsController : BaseController
    {
        private readonly IGetWorkElementsUseCase _getWorkElementsUseCase;

        public WorkElementsController(
            IGetWorkElementsUseCase getWorkElementsUseCase
        )
        {
            _getWorkElementsUseCase = getWorkElementsUseCase;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<WorkElementResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("elements")]
        public async Task<IActionResult> GetWorkElements([FromQuery][Required] string query, [FromQuery] int? page, [FromQuery] int? size)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Problem(
                    "The requested query is missing or invalid",
                    $"/api/v1/work/elements?query={query}",
                    StatusCodes.Status400BadRequest, "Bad Request"
                );

            var workElements = await _getWorkElementsUseCase.ExecuteAsync(query, page, size);
            return Ok(workElements.Select(pe => pe.ToResponse()).ToList());
        }
    }
}
