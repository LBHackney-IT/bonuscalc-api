using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.UseCase
{
    public class GetPaymentUseCaseTests
    {
        //private GetPaymentUseCase _classUnderTest;
        private Mock<IPayElementTypesGateway> _mockPaymentGateway;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _mockPaymentGateway = new Mock<IPayElementTypesGateway>();
            //_classUnderTest = new GetPaymentUseCase(_mockPaymentGateway.Object);
        }
        /*[Test]
        public async Task GetOperativeTimesheet()
        {
            // Arrange
            var expectedTimesheet = _fixture.Create<Timesheet>();
            _mockPaymentGateway.Setup(x => x.GetPaymentAsync(expectedTimesheet.WeekId, expectedTimesheet.OperativeId))
                .ReturnsAsync(expectedTimesheet);

            // Act
            var result = await _classUnderTest.Execute(expectedTimesheet.WeekId, expectedTimesheet.OperativeId);

            // Assert
            result.Should().BeEquivalentTo(expectedTimesheet);
        }*/
    }
}
