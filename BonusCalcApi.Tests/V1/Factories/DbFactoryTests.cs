using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.Factories
{
    public class DbFactoryTests
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
        }

        [Test]
        public void CanMapPayElementUpdate()
        {
            // Arrange
            var payElementUpdate = _fixture.Create<PayElementUpdate>();

            // Act
            var result = payElementUpdate.ToDb();

            // Assert
            result.Id.Should().Be(payElementUpdate.Id);
            result.Address.Should().Be(payElementUpdate.Address);
            result.Comment.Should().Be(payElementUpdate.Comment);
            result.Duration.Should().Be(payElementUpdate.Duration);
            result.Value.Should().Be(payElementUpdate.Value);
            result.WeekDay.Should().Be(payElementUpdate.WeekDay);
            result.WorkOrder.Should().Be(payElementUpdate.WorkOrder);
            result.PayElementTypeId.Should().Be(payElementUpdate.PayElementTypeId);
        }
    }
}
