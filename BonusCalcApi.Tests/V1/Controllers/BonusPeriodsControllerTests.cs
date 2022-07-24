using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Controllers;
using BonusCalcApi.V1.Exceptions;
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
using Microsoft.AspNetCore.Mvc;

namespace BonusCalcApi.Tests.V1.Controllers
{
    [TestFixture]
    public class BonusPeriodsControllerTests : ControllerTests
    {
        private Mock<ICreateBonusPeriodUseCase> _createBonusPeriodUseCaseMock;
        private Mock<IGetBonusPeriodsUseCase> _getBonusPeriodsUseCaseMock;
        private Mock<IGetCurrentBonusPeriodsUseCase> _getCurrentBonusPeriodsUseCaseMock;
        private DateTime _currentDate;

        private BonusPeriodsController _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _createBonusPeriodUseCaseMock = new Mock<ICreateBonusPeriodUseCase>();
            _getBonusPeriodsUseCaseMock = new Mock<IGetBonusPeriodsUseCase>();
            _getCurrentBonusPeriodsUseCaseMock = new Mock<IGetCurrentBonusPeriodsUseCase>();
            _currentDate = new DateTime(2021, 12, 5, 16, 0, 0, DateTimeKind.Utc);

            _classUnderTest = new BonusPeriodsController(
                _createBonusPeriodUseCaseMock.Object,
                _getBonusPeriodsUseCaseMock.Object,
                _getCurrentBonusPeriodsUseCaseMock.Object
            );
        }

        [Test]
        public async Task CreateBonusPeriod()
        {
            // Arrange
            var expectedBonusPeriod = FixtureHelpers.CreateBonusPeriod();
            var request = new BonusPeriodRequest { Id = "2022-01-31" };
            _createBonusPeriodUseCaseMock.Setup(x => x.ExecuteAsync(request))
                .ReturnsAsync(expectedBonusPeriod);

            // Act
            var objectResult = await _classUnderTest.CreateBonusPeriod(request);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<BonusPeriodResponse>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedBonusPeriod.ToResponse());
        }

        [Test]
        public async Task CreateBonusPeriodReturnsBadRequestIfInvalid()
        {
            // Arrange
            var request = new BonusPeriodRequest { Id = "20220131" };
            _createBonusPeriodUseCaseMock.Setup(x => x.ExecuteAsync(request))
                .ThrowsAsync(new BadRequestException("Date format is invalid - it should be YYYY-MM-DD"));

            // Act
            var objectResult = await _classUnderTest.CreateBonusPeriod(request);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            result.Status.Should().Be((int) HttpStatusCode.BadRequest);
            result.Detail.Should().Be("Date format is invalid - it should be YYYY-MM-DD");
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
