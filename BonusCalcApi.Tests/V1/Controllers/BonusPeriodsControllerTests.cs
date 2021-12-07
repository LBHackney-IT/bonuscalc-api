using AutoFixture;
using BonusCalcApi.Tests.V1.Controllers.Mocks;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Controllers;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BonusCalcApi.Tests.V1.Controllers
{
    [TestFixture]
    public class BonusPeriodsControllerTests : ControllerTests
    {
        private Fixture _fixture;
        private Mock<IGetCurrentBonusPeriodsUseCase> _getCurrentBonusPeriodsUseCaseMock;
        private MockProblemDetailsFactory _problemDetailsFactoryMock;
        private DateTime _currentDate;

        private BonusPeriodsController _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fixture = FixtureHelpers.Fixture;
            _getCurrentBonusPeriodsUseCaseMock = new Mock<IGetCurrentBonusPeriodsUseCase>();
            _problemDetailsFactoryMock = new MockProblemDetailsFactory();
            _currentDate = new DateTime(2021, 12, 5, 16, 0, 0, DateTimeKind.Utc);

            _classUnderTest = new BonusPeriodsController(_getCurrentBonusPeriodsUseCaseMock.Object);

            // .NET 3.1 doesn't set ProblemDetailsFactory so we need to mock it
            _classUnderTest.ProblemDetailsFactory = _problemDetailsFactoryMock.Object;
        }

        [Test]
        public async Task GetCurrentBonusPeriods()
        {
            // Arrange
            var expectedBonusPeriods = FixtureHelpers.CreateBonusPeriods();
            _getCurrentBonusPeriodsUseCaseMock.Setup(x => x.ExecuteAsync(_currentDate))
                .ReturnsAsync(expectedBonusPeriods);

            // Act
            var objectResult = await _classUnderTest.GetCurrentBonusPeriods(_currentDate);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<List<BonusPeriodResponse>>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedBonusPeriods.Select(bp => bp.ToResponse()).ToList());
        }
    }
}
