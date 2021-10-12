using System.Collections.Generic;
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
    public class ListOperativeNonProductiveTimeTests
    {
        private ListOperativeNonProductiveTime _classUnderTest;
        private Mock<INonProductiveTimeGateway> _mockTimeGateway;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _mockTimeGateway = new Mock<INonProductiveTimeGateway>();
            _classUnderTest = new ListOperativeNonProductiveTime(
                _mockTimeGateway.Object
                );
        }

        [Test]
        public async Task ReturnListOfNpt()
        {
            // Arrange
            string expectedPrn = "123456";
            var timeList = _fixture.CreateMany<NonProductiveTime>();
            _mockTimeGateway.Setup(x => x.GetNonProductiveTimeAsync(It.IsAny<string>()))
                .ReturnsAsync(timeList);

            // Act
            var result = await _classUnderTest.Execute(expectedPrn);

            // Assert
            result.Should().BeEquivalentTo(timeList);
        }
    }
}
