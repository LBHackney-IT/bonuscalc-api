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
        public async Task RetrievesOperativeFromDb()
        {
            // Arrange
            var operative = await AddOperative();

            // Act
            var result = await _classUnderTest.GetOperativeAsync(operative.Id);

            // Assert
            result.Should().BeEquivalentTo(operative);
        }

        [Test]
        public async Task RetrievesOperativesFromDb()
        {
            // Arrange
            var operatives = await AddOperatives();

            // Act
            var results = await _classUnderTest.GetOperativesAsync("123456", null, null);

            // Assert
            results.Should().BeEquivalentTo(operatives);
        }

        [Test]
        public async Task RetrievesNonExistentOperativeFromDb()
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
                ConversionFactor = 1.0M,
                MaxValue = 62868.0M,
                PayBands = new List<PayBand>
                {
                    new PayBand { Id = 11, Band = 1, Value = 2160 },
                    new PayBand { Id = 12, Band = 2, Value = 2772 },
                    new PayBand { Id = 13, Band = 3, Value = 3132 },
                    new PayBand { Id = 14, Band = 4, Value = 3366 },
                    new PayBand { Id = 15, Band = 5, Value = 3618 },
                    new PayBand { Id = 16, Band = 6, Value = 3888 },
                    new PayBand { Id = 17, Band = 7, Value = 4182 },
                    new PayBand { Id = 18, Band = 8, Value = 4494 },
                    new PayBand { Id = 19, Band = 9, Value = 4836 }
                }
            };

            var manager = new Person
            {
                Id = "800001",
                Name = "A Manager",
                EmailAddress = "a.manager@hackney.gov.uk"
            };

            var supervisor = new Person
            {
                Id = "810001",
                Name = "A Supervisor",
                EmailAddress = "a.supervisor@hackney.gov.uk"
            };

            var operative = new Operative
            {
                Id = "123456",
                Name = "An Operative",
                EmailAddress = "an.operative@hackney.gov.uk",
                Manager = manager,
                Supervisor = supervisor,
                Trade = trade,
                Scheme = scheme,
                Section = "H3007",
                SalaryBand = 5,
                Utilisation = 1.0M,
                FixedBand = false,
                IsArchived = false
            };

            await BonusCalcContext.People.AddAsync(manager);
            await BonusCalcContext.People.AddAsync(supervisor);
            await BonusCalcContext.Trades.AddAsync(trade);
            await BonusCalcContext.Schemes.AddAsync(scheme);
            await BonusCalcContext.SaveChangesAsync();

            await BonusCalcContext.Operatives.AddAsync(operative);
            await BonusCalcContext.SaveChangesAsync();

            BonusCalcContext.ChangeTracker.Clear();

            return operative;
        }

        private async Task<List<Operative>> AddOperatives()
        {
            return new List<Operative>() { await AddOperative() };
        }
    }
}
