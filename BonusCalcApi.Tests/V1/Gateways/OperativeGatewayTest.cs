using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.Gateways
{
    public class OperativeGatewayTests
    {
        private OperativeGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new OperativeGateway(InMemoryDb.Instance);
        }

        [TearDown]
        public void TearDown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task RetrievesOperativeFromDB()
        {
            // Arrange
            var trade = await AddTrade();
            var scheme = await AddScheme();
            var operative = await AddOperative(trade, scheme);
            var expectedOperative = new Operative
            {
                Id = operative.Id,
                Name = "An Operative",
                TradeId = trade.Id,
                Trade = trade,
                SchemeId = scheme.Id,
                Scheme = scheme,
                Section = "H3007",
                SalaryBand = 5,
                FixedBand = false,
                IsArchived = false
            };

            // Act
            var result = await _classUnderTest.GetOperativeAsync(operative.Id);

            // Assert
            result.Should().BeEquivalentTo(expectedOperative);
        }

        [Test]
        public async Task RetrievesNonExistentOperativeFromDB()
        {
            // Act
            var result = await _classUnderTest.GetOperativeAsync("1234");

            // Assert
            result.Should().BeNull();
        }

        private static async Task<Trade> AddTrade()
        {
            var trade = new Trade
            {
                Id = "EL",
                Description = "Electrician"
            };

            await InMemoryDb.Instance.Trades.AddAsync(trade);
            await InMemoryDb.Instance.SaveChangesAsync();
            return trade;
        }

        private static async Task<Scheme> AddScheme()
        {
            var scheme = new Scheme
            {
                Id = 1,
                Type = "SMV",
                Description = "Reactive",
                ConversionFactor = 1.0M
            };

            await InMemoryDb.Instance.Schemes.AddAsync(scheme);
            await InMemoryDb.Instance.SaveChangesAsync();
            return scheme;
        }

        private static async Task<Operative> AddOperative(Trade trade, Scheme scheme)
        {
            var operative = new Operative
            {
                Id = "1234",
                Name = "An Operative",
                TradeId = trade.Id,
                SchemeId = scheme.Id,
                Section = "H3007",
                SalaryBand = 5,
                FixedBand = false,
                IsArchived = false
            };

            await InMemoryDb.Instance.Operatives.AddAsync(operative);
            await InMemoryDb.Instance.SaveChangesAsync();
            return operative;
        }
    }
}
