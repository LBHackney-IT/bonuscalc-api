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
using BonusCalcApi.V1.Boundary.Request;
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
        private Mock<IGetBandChangeAuthorisationsUseCase> _getBandChangeAuthorisationsUseCaseMock;
        private Mock<ISupervisorBandDecisionUseCase> _supervisorBandDecisionUseCaseMock;
        private Mock<IManagerBandDecisionUseCase> _managerBandDecisionUseCaseMock;
        private Mock<IUpdateBandChangeReportSentAtUseCase> _updateBandChangeReportSentAtUseCaseMock;
        private Mock<IGetBandChangeUseCase> _getBandChangeUseCaseMock;

        private BandChangesController _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fixture = FixtureHelpers.Fixture;
            _getBonusPeriodForChangesUseCaseMock = new Mock<IGetBonusPeriodForChangesUseCase>();
            _getProjectedChangesUseCaseMock = new Mock<IGetProjectedChangesUseCase>();
            _startBandChangeProcessUseCaseMock = new Mock<IStartBandChangeProcessUseCase>();
            _getBandChangesUseCaseMock = new Mock<IGetBandChangesUseCase>();
            _getBandChangeAuthorisationsUseCaseMock = new Mock<IGetBandChangeAuthorisationsUseCase>();
            _supervisorBandDecisionUseCaseMock = new Mock<ISupervisorBandDecisionUseCase>();
            _managerBandDecisionUseCaseMock = new Mock<IManagerBandDecisionUseCase>();
            _updateBandChangeReportSentAtUseCaseMock = new Mock<IUpdateBandChangeReportSentAtUseCase>();
            _getBandChangeUseCaseMock = new Mock<IGetBandChangeUseCase>();

            _classUnderTest = new BandChangesController(
                _getBonusPeriodForChangesUseCaseMock.Object,
                _getProjectedChangesUseCaseMock.Object,
                _startBandChangeProcessUseCaseMock.Object,
                _getBandChangesUseCaseMock.Object,
                _getBandChangeAuthorisationsUseCaseMock.Object,
                _supervisorBandDecisionUseCaseMock.Object,
                _managerBandDecisionUseCaseMock.Object,
                _updateBandChangeReportSentAtUseCaseMock.Object,
                _getBandChangeUseCaseMock.Object
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

        [Test]
        public async Task GetBandChangeAuthorisations()
        {
            // Arrange
            var expectedAuthorisations = _fixture.CreateMany<BandChange>();

            _getBandChangeAuthorisationsUseCaseMock
                .Setup(x => x.ExecuteAsync())
                .ReturnsAsync(expectedAuthorisations);

            // Act
            var objectResult = await _classUnderTest.GetBandChangeAuthorisations();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<List<BandChangeResponse>>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedAuthorisations.Select(bc => bc.ToResponse()).ToList());
        }

        [Test]
        public async Task GetBandChangeAuthorisationsReturnsNotFoundIfBonusPeriodNotOpen()
        {
            // Arrange
            _getBandChangeAuthorisationsUseCaseMock
                .Setup(x => x.ExecuteAsync())
                .ThrowsAsync(new ResourceNotFoundException("Open bonus period not found"));

            // Act
            var objectResult = await _classUnderTest.GetBandChangeAuthorisations();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            result.Status.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetBandChanges()
        {
            // Arrange
            var expectedBandChanges = _fixture.CreateMany<BandChange>();

            _getBandChangesUseCaseMock
                .Setup(x => x.ExecuteAsync(null))
                .ReturnsAsync(expectedBandChanges);

            // Act
            var objectResult = await _classUnderTest.GetBandChanges(null);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<List<BandChangeResponse>>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedBandChanges.Select(bc => bc.ToResponse()).ToList());
        }

        [Test]
        public async Task GetBandChangesForPeriod()
        {
            // Arrange
            var expectedBandChanges = _fixture.CreateMany<BandChange>();

            _getBandChangesUseCaseMock
                .Setup(x => x.ExecuteAsync("2021-08-02"))
                .ReturnsAsync(expectedBandChanges);

            // Act
            var objectResult = await _classUnderTest.GetBandChanges("2021-08-02");
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
                .Setup(x => x.ExecuteAsync(null))
                .ThrowsAsync(new ResourceNotFoundException("Open bonus period not found"));

            // Act
            var objectResult = await _classUnderTest.GetBandChanges(null);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            result.Status.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetBandChangesReturnsNotFoundIfBonusPeriodNotFound()
        {
            // Arrange
            _getBandChangesUseCaseMock
                .Setup(x => x.ExecuteAsync("2021-08-02"))
                .ThrowsAsync(new ResourceNotFoundException("Bonus period not found for 2021-08-02"));

            // Act
            var objectResult = await _classUnderTest.GetBandChanges("2021-08-02");
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            result.Status.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetBandChangesReturnsBadRequestIfBonusPeriodIdInvalid()
        {
            // Arrange
            _getBandChangesUseCaseMock
                .Setup(x => x.ExecuteAsync("20210802"))
                .ThrowsAsync(new BadRequestException("Bonus period is invalid - it should be YYYY-MM-DD"));

            // Act
            var objectResult = await _classUnderTest.GetBandChanges("20210802");
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            result.Status.Should().Be((int) HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetBandChange()
        {
            // Arrange
            var expectedBandChange = _fixture.Create<BandChange>();

            _getBandChangeUseCaseMock
                .Setup(x => x.ExecuteAsync(expectedBandChange.OperativeId))
                .ReturnsAsync(expectedBandChange);

            // Act
            var objectResult = await _classUnderTest.GetBandChange(expectedBandChange.OperativeId);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<BandChangeResponse>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedBandChange.ToResponse());
        }

        [Test]
        public async Task GetBandChangeReturnsNotFoundIfBonusPeriodNotOpen()
        {
            // Arrange
            _getBandChangeUseCaseMock
                .Setup(x => x.ExecuteAsync("123456"))
                .ThrowsAsync(new ResourceNotFoundException("Open bonus period not found"));

            // Act
            var objectResult = await _classUnderTest.GetBandChange("123456");
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            result.Status.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task SupervisorBandDecision()
        {
            // Arrange
            var request = _fixture.Create<BandChangeRequest>();
            var bandChange = _fixture.Create<BandChange>();

            _supervisorBandDecisionUseCaseMock
                .Setup(x => x.ExecuteAsync("123456", request))
                .ReturnsAsync(bandChange);

            // Act
            var objectResult = await _classUnderTest.SupervisorBandDecision("123456", request);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<BandChangeResponse>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Id.Should().Be(bandChange.Id);
        }

        [Test]
        public async Task SupervisorBandDecisionReturnsNotFoundIfBonusPeriodNotOpen()
        {
            // Arrange
            var request = _fixture.Create<BandChangeRequest>();

            _supervisorBandDecisionUseCaseMock
                .Setup(x => x.ExecuteAsync("123456", request))
                .ThrowsAsync(new ResourceNotFoundException("Open bonus period not found"));

            // Act
            var objectResult = await _classUnderTest.SupervisorBandDecision("123456", request);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            result.Status.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task SupervisorBandDecisionReturnsUnprocessableEntityIfRequestInvalid()
        {
            // Arrange
            var request = _fixture.Create<BandChangeRequest>();

            _supervisorBandDecisionUseCaseMock
                .Setup(x => x.ExecuteAsync("123456", request))
                .ThrowsAsync(new ResourceNotProcessableException("Bonus period has already been closed"));

            // Act
            var objectResult = await _classUnderTest.SupervisorBandDecision("123456", request);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.UnprocessableEntity);
            result.Status.Should().Be((int) HttpStatusCode.UnprocessableEntity);
            result.Detail.Should().Be("Bonus period has already been closed");
        }

        [Test]
        public async Task ManagerBandDecision()
        {
            // Arrange
            var request = _fixture.Create<BandChangeRequest>();
            var bandChange = _fixture.Create<BandChange>();

            _managerBandDecisionUseCaseMock
                .Setup(x => x.ExecuteAsync("123456", request))
                .ReturnsAsync(bandChange);

            // Act
            var objectResult = await _classUnderTest.ManagerBandDecision("123456", request);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<BandChangeResponse>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Id.Should().Be(bandChange.Id);
        }

        [Test]
        public async Task ManagerBandDecisionReturnsNotFoundIfBonusPeriodNotOpen()
        {
            // Arrange
            var request = _fixture.Create<BandChangeRequest>();

            _managerBandDecisionUseCaseMock
                .Setup(x => x.ExecuteAsync("123456", request))
                .ThrowsAsync(new ResourceNotFoundException("Open bonus period not found"));

            // Act
            var objectResult = await _classUnderTest.ManagerBandDecision("123456", request);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            result.Status.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task ManagerBandDecisionReturnsUnprocessableEntityIfRequestInvalid()
        {
            // Arrange
            var request = _fixture.Create<BandChangeRequest>();

            _managerBandDecisionUseCaseMock
                .Setup(x => x.ExecuteAsync("123456", request))
                .ThrowsAsync(new ResourceNotProcessableException("Bonus period has already been closed"));

            // Act
            var objectResult = await _classUnderTest.ManagerBandDecision("123456", request);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.UnprocessableEntity);
            result.Status.Should().Be((int) HttpStatusCode.UnprocessableEntity);
            result.Detail.Should().Be("Bonus period has already been closed");
        }

        [Test]
        public async Task UpdateReportSentAtReturnsOk()
        {
            // Arrange
            _updateBandChangeReportSentAtUseCaseMock
                .Setup(x => x.ExecuteAsync("123456"))
                .Returns(Task.CompletedTask);

            // Act
            var objectResult = await _classUnderTest.UpdateReportSentAt("123456");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
        }

        [Test]
        public async Task UpdateReportSentAtReturnsBadRequestIfOperativePrnInvalid()
        {
            // Arrange
            _updateBandChangeReportSentAtUseCaseMock
                .Setup(x => x.ExecuteAsync("1234"))
                .ThrowsAsync(new BadRequestException("Operative payroll number is invalid"));

            // Act
            var objectResult = await _classUnderTest.UpdateReportSentAt("1234");
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            result.Status.Should().Be((int) HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task UpdateReportSentAtReturnsNotFoundIfBonusPeriodNotOpen()
        {
            // Arrange
            _updateBandChangeReportSentAtUseCaseMock
                .Setup(x => x.ExecuteAsync("123456"))
                .ThrowsAsync(new ResourceNotFoundException("Open bonus period not found"));

            // Act
            var objectResult = await _classUnderTest.UpdateReportSentAt("123456");
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            result.Status.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task UpdateReportSentAtReturnsUnprocessableEntityIfRequestInvalid()
        {
            // Arrange
            _updateBandChangeReportSentAtUseCaseMock
                .Setup(x => x.ExecuteAsync("123456"))
                .ThrowsAsync(new ResourceNotProcessableException("Bonus period has already been closed"));

            // Act
            var objectResult = await _classUnderTest.UpdateReportSentAt("123456");
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.UnprocessableEntity);
            result.Status.Should().Be((int) HttpStatusCode.UnprocessableEntity);
            result.Detail.Should().Be("Bonus period has already been closed");
        }
    }
}
