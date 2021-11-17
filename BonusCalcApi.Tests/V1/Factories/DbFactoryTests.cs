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
            result.Monday.Should().Be(payElementUpdate.Monday);
            result.Tuesday.Should().Be(payElementUpdate.Tuesday);
            result.Wednesday.Should().Be(payElementUpdate.Wednesday);
            result.Thursday.Should().Be(payElementUpdate.Thursday);
            result.Friday.Should().Be(payElementUpdate.Friday);
            result.Saturday.Should().Be(payElementUpdate.Saturday);
            result.Sunday.Should().Be(payElementUpdate.Sunday);
            result.Duration.Should().Be(payElementUpdate.Duration);
            result.Value.Should().Be(payElementUpdate.Value);
            result.WorkOrder.Should().Be(payElementUpdate.WorkOrder);
            result.ClosedAt.Should().Be(payElementUpdate.ClosedAt);
            result.PayElementTypeId.Should().Be(payElementUpdate.PayElementTypeId);
        }
    }
}
