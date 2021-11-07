using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.UseCase
{
    public class GetOperativeTimesheetUseCaseTests
    {
        private GetOperativeTimesheetUseCase _classUnderTest;
        private Mock<ITimesheetGateway> _mockTimeSheetGateway;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _mockTimeSheetGateway = new Mock<ITimesheetGateway>();
            _classUnderTest = new GetOperativeTimesheetUseCase(_mockTimeSheetGateway.Object);
        }
        [Test]
        public async Task GetOperativeTimesheet()
        {
            // Arrange
            var expectedTimesheet = _fixture.Create<Timesheet>();
            _mockTimeSheetGateway.Setup(x => x.GetOperativeTimesheetAsync(expectedTimesheet.OperativeId, expectedTimesheet.WeekId))
                .ReturnsAsync(expectedTimesheet);

            // Act
            var result = await _classUnderTest.Execute(expectedTimesheet.OperativeId, expectedTimesheet.WeekId);

            // Assert
            result.Should().BeEquivalentTo(expectedTimesheet);
        }
    }
}
