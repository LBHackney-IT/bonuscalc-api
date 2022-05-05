using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NUnit.Framework;
using System.Security.Claims;

namespace BonusCalcApi.Tests.V1.Controllers
{
    [TestFixture]
    abstract public class ControllerTests
    {

        protected static T GetResultData<T>(IActionResult result)
        {
            return (T) (result as ObjectResult)?.Value;
        }

        protected static int? GetStatusCode(IActionResult result)
        {
            return (result as IStatusCodeActionResult).StatusCode;
        }
    }
}
