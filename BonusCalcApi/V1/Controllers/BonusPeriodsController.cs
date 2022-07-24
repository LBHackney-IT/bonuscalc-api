using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/periods")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class BonusPeriodsController : BaseController
    {
        private readonly IGetBonusPeriodsUseCase _getBonusPeriodsUseCase;
        private readonly IGetCurrentBonusPeriodsUseCase _getCurrentBonusPeriodsUseCase;

        public BonusPeriodsController(
            IGetBonusPeriodsUseCase getBonusPeriodsUseCase,
            IGetCurrentBonusPeriodsUseCase getCurrentBonusPeriodsUseCase
        )
        {
            _getBonusPeriodsUseCase = getBonusPeriodsUseCase;
            _getCurrentBonusPeriodsUseCase = getCurrentBonusPeriodsUseCase;
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

        private static DateTime EnsureValidDate(DateTime? date)
        {
            return (date ?? DateTime.UtcNow);
        }
    }
}
