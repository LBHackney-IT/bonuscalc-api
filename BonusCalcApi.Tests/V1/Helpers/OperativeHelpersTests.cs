using BonusCalcApi.V1.Controllers.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.Helpers
{
    public class OperativeHelpersTests
    {
        private OperativeHelpers _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new OperativeHelpers();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("123")]
        [TestCase("1234567")]
        [TestCase("ABCDEF")]
        public void ReturnsFalseWhenInvalidPrn(string operativePayrollNumber)
        {
            var result = _classUnderTest.IsValidPrn(operativePayrollNumber);

            result.Should().BeFalse();
        }

        [Test]
        public void ReturnsTrueWhenValidPrn()
        {
            var result = _classUnderTest.IsValidPrn("123456");

            result.Should().BeTrue();
        }
    }
}
