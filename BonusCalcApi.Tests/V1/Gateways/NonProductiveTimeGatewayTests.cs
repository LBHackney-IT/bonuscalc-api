using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.Gateways
{
    public class NonProductiveTimeGatewayTests
    {
        private NonProductiveTimeGateway _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _classUnderTest = new NonProductiveTimeGateway(InMemoryDb.Instance);
        }

        [TearDown]
        public void Teardown() => InMemoryDb.Teardown();

        [Test]
        public async Task RetrievesTime()
        {
            // Arrange
            var operative = new Operative
            {
                Id = "123456",
                NonProductiveTimeList = _fixture.CreateMany<NonProductiveTime>().ToList()
            };
            await InMemoryDb.Instance.Operatives.AddAsync(operative);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            var result = await _classUnderTest.GetNonProductiveTimeAsync(operative.Id);

            // Assert
            result.Should().BeEquivalentTo(operative.NonProductiveTimeList);
        }
    }
}
