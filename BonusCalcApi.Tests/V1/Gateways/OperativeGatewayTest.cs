using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.Gateways
{
    [TestFixture]
    public class OperativeGatewayTests : DatabaseTests
    {
        private OperativeGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new OperativeGateway(BonusCalcContext);
        }

        [Test]
        public async Task RetrievesOperativeFromDB()
        {
            // Arrange
            var operative = await AddOperative();

            // Act
            var result = await _classUnderTest.GetOperativeAsync(operative.Id);

            // Assert
            result.Should().BeEquivalentTo(operative);
        }

        [Test]
        public async Task RetrievesNonExistentOperativeFromDB()
        {
            // Act
            var result = await _classUnderTest.GetOperativeAsync("000000");

            // Assert
            result.Should().BeNull();
        }

        private async Task<Operative> AddOperative()
        {
            var trade = new Trade
            {
                Id = "EL",
                Description = "Electrician"
            };

            var scheme = new Scheme
            {
                Id = 1,
                Type = "SMV",
                Description = "Reactive",
                ConversionFactor = 1.0M
            };

            var operative = new Operative
            {
                Id = "123456",
                Name = "An Operative",
                EmailAddress = "an.operative@hackney.gov.uk",
                Trade = trade,
                Scheme = scheme,
                Section = "H3007",
                SalaryBand = 5,
                FixedBand = false,
                IsArchived = false
            };

            await BonusCalcContext.Trades.AddAsync(trade);
            await BonusCalcContext.Schemes.AddAsync(scheme);
            await BonusCalcContext.Operatives.AddAsync(operative);
            await BonusCalcContext.SaveChangesAsync();

            return operative;
        }
    }
}
