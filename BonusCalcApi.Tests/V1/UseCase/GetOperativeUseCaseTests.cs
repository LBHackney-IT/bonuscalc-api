using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.UseCase
{
    public class GetOperativeUseCaseTests
    {
        private GetOperativeUseCase _classUnderTest;
        private Mock<IOperativeGateway> _mockOperativeGateway;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _mockOperativeGateway = new Mock<IOperativeGateway>();
            _classUnderTest = new GetOperativeUseCase(_mockOperativeGateway.Object);
        }

        [Test]
        public async Task GetOperative()
        {
            // Arrange
            var expectedOperative = _fixture.Create<Operative>();
            _mockOperativeGateway.Setup(x => x.GetOperativeAsync(expectedOperative.Id))
                .ReturnsAsync(expectedOperative);

            // Act
            var result = await _classUnderTest.ExecuteAsync(expectedOperative.Id);

            // Assert
            result.Should().BeEquivalentTo(expectedOperative);
        }
    }
}
