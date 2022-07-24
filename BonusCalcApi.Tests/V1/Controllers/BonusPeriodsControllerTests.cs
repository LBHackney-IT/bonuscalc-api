using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Controllers;
using BonusCalcApi.V1.Factories;
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
        private Mock<IGetBonusPeriodsUseCase> _getBonusPeriodsUseCaseMock;
        private Mock<IGetCurrentBonusPeriodsUseCase> _getCurrentBonusPeriodsUseCaseMock;
        private DateTime _currentDate;

        private BonusPeriodsController _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _getBonusPeriodsUseCaseMock = new Mock<IGetBonusPeriodsUseCase>();
            _getCurrentBonusPeriodsUseCaseMock = new Mock<IGetCurrentBonusPeriodsUseCase>();
            _currentDate = new DateTime(2021, 12, 5, 16, 0, 0, DateTimeKind.Utc);

            _classUnderTest = new BonusPeriodsController(
                _getBonusPeriodsUseCaseMock.Object,
                _getCurrentBonusPeriodsUseCaseMock.Object
            );
        }

        [Test]
        public async Task GetBonusPeriods()
        {
            // Arrange
            var expectedBonusPeriods = FixtureHelpers.CreateBonusPeriods();
            _getBonusPeriodsUseCaseMock.Setup(x => x.ExecuteAsync())
                .ReturnsAsync(expectedBonusPeriods);

            // Act
            var objectResult = await _classUnderTest.GetBonusPeriods();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<List<BonusPeriodResponse>>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedBonusPeriods.Select(bp => bp.ToResponse()).ToList());
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
