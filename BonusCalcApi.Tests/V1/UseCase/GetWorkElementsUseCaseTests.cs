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
    public class GetWorkElementsUseCaseTests
    {
        private Mock<IWorkElementGateway> _mockWorkElementGateway;
        private GetWorkElementsUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockWorkElementGateway = new Mock<IWorkElementGateway>();
            _classUnderTest = new GetWorkElementsUseCase(_mockWorkElementGateway.Object);
        }
        [Test]
        public async Task GetWorkElements()
        {
            // Arrange
            var expectedWorkElements = _fixture.CreateMany<WorkElement>();
            _mockWorkElementGateway
                .Setup(x => x.GetWorkElementsAsync("12345678", 1, 25))
                .ReturnsAsync(expectedWorkElements);

            // Act
            var result = await _classUnderTest.ExecuteAsync("12345678", 1, 25);

            // Assert
            result.Should().BeEquivalentTo(expectedWorkElements);
        }
    }
}
