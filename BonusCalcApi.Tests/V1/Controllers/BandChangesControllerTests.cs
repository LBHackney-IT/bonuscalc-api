using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Controllers;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BonusCalcApi.Tests.V1.Controllers
{
    [TestFixture]
    public class BandChangesControllerTests : ControllerTests
    {
        private Fixture _fixture;
        private Mock<IGetBonusPeriodForChangesUseCase> _getBonusPeriodForChangesUseCaseMock;
        private Mock<IGetProjectedChangesUseCase> _getProjectedChangesUseCaseMock;
        private Mock<IStartBandChangeProcessUseCase> _startBandChangeProcessUseCaseMock;
        private Mock<IGetBandChangesUseCase> _getBandChangesUseCaseMock;

        private BandChangesController _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fixture = FixtureHelpers.Fixture;
            _getBonusPeriodForChangesUseCaseMock = new Mock<IGetBonusPeriodForChangesUseCase>();
            _getProjectedChangesUseCaseMock = new Mock<IGetProjectedChangesUseCase>();
            _startBandChangeProcessUseCaseMock = new Mock<IStartBandChangeProcessUseCase>();
            _getBandChangesUseCaseMock = new Mock<IGetBandChangesUseCase>();

            _classUnderTest = new BandChangesController(
                _getBonusPeriodForChangesUseCaseMock.Object,
                _getProjectedChangesUseCaseMock.Object,
                _startBandChangeProcessUseCaseMock.Object,
                _getBandChangesUseCaseMock.Object
            );
        }

        [Test]
        public async Task GetBonusPeriod()
        {
            // Arrange
            var expectedBonusPeriod = _fixture.Build<BonusPeriod>()
                .Without(bp => bp.Weeks)
                .Create();

            _getBonusPeriodForChangesUseCaseMock
                .Setup(x => x.ExecuteAsync())
                .ReturnsAsync(expectedBonusPeriod);

            // Act
            var objectResult = await _classUnderTest.GetBonusPeriod();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<BonusPeriodResponse>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedBonusPeriod.ToResponse());
        }

        [Test]
        public async Task GetBonusPeriodReturnsNotFoundIfBonusPeriodNotOpen()
        {
            // Arrange
            _getBonusPeriodForChangesUseCaseMock
                .Setup(x => x.ExecuteAsync())
                .ThrowsAsync(new ResourceNotFoundException("Open bonus period not found"));

            // Act
            var objectResult = await _classUnderTest.GetBonusPeriod();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            result.Status.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetProjectedChanges()
        {
            // Arrange
            var expectedProjections = _fixture.CreateMany<OperativeProjection>();

            _getProjectedChangesUseCaseMock
                .Setup(x => x.ExecuteAsync())
                .ReturnsAsync(expectedProjections);

            // Act
            var objectResult = await _classUnderTest.GetProjectedChanges();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<List<OperativeProjectionResponse>>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedProjections.Select(p => p.ToResponse()).ToList());
        }

        [Test]
        public async Task GetProjectedChangesReturnsNotFoundIfBonusPeriodNotOpen()
        {
            // Arrange
            _getProjectedChangesUseCaseMock
                .Setup(x => x.ExecuteAsync())
                .ThrowsAsync(new ResourceNotFoundException("Open bonus period not found"));

            // Act
            var objectResult = await _classUnderTest.GetProjectedChanges();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            result.Status.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task StartBandChange()
        {
            // Arrange
            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .Without(bp => bp.Weeks)
                .Create();

            _startBandChangeProcessUseCaseMock
                .Setup(x => x.ExecuteAsync())
                .ReturnsAsync(bonusPeriod);

            // Act
            var objectResult = await _classUnderTest.StartBandChanges();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<BonusPeriodResponse>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Id.Should().Be(bonusPeriod.Id);
        }

        [Test]
        public async Task StartBandChangesChangesReturnsNotFoundIfBonusPeriodNotOpen()
        {
            // Arrange
            _startBandChangeProcessUseCaseMock
                .Setup(x => x.ExecuteAsync())
                .ThrowsAsync(new ResourceNotFoundException("Open bonus period not found"));

            // Act
            var objectResult = await _classUnderTest.StartBandChanges();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            result.Status.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task StartBandChangesChangesReturnsUnprocessableEntityIfBonusPeriodInvalid()
        {
            // Arrange
            _startBandChangeProcessUseCaseMock
                .Setup(x => x.ExecuteAsync())
                .ThrowsAsync(new ResourceNotProcessableException("Bonus period has already been closed"));

            // Act
            var objectResult = await _classUnderTest.StartBandChanges();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.UnprocessableEntity);
            result.Status.Should().Be((int) HttpStatusCode.UnprocessableEntity);
            result.Detail.Should().Be("Bonus period has already been closed");
        }

        public async Task GetBandChanges()
        {
            // Arrange
            var expectedBandChanges = _fixture.CreateMany<BandChange>();

            _getBandChangesUseCaseMock
                .Setup(x => x.ExecuteAsync())
                .ReturnsAsync(expectedBandChanges);

            // Act
            var objectResult = await _classUnderTest.GetBandChanges();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<List<BandChangeResponse>>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedBandChanges.Select(bc => bc.ToResponse()).ToList());
        }

        [Test]
        public async Task GetBandChangesReturnsNotFoundIfBonusPeriodNotOpen()
        {
            // Arrange
            _getBandChangesUseCaseMock
                .Setup(x => x.ExecuteAsync())
                .ThrowsAsync(new ResourceNotFoundException("Open bonus period not found"));

            // Act
            var objectResult = await _classUnderTest.GetBandChanges();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            result.Status.Should().Be((int) HttpStatusCode.NotFound);
        }
    }
}
