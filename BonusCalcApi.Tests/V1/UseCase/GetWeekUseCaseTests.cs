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
    public class GetWeekUseCaseTests
    {
        private GetWeekUseCase _classUnderTest;
        private Mock<IWeekGateway> _mockWeekGateway;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _mockWeekGateway = new Mock<IWeekGateway>();
            _classUnderTest = new GetWeekUseCase(_mockWeekGateway.Object);
        }
        [Test]
        public async Task GetWeek()
        {
            // Arrange
            var expectedWeek = _fixture.Create<Week>();
            _mockWeekGateway.Setup(x => x.GetWeekAsync(expectedWeek.Id))
                .ReturnsAsync(expectedWeek);

            // Act
            var result = await _classUnderTest.ExecuteAsync(expectedWeek.Id);

            // Assert
            result.Should().BeEquivalentTo(expectedWeek);
        }
    }
}
