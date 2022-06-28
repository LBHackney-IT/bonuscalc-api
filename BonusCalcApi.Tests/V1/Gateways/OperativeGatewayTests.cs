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
                MaxValue = 62868.0M
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
            await BonusCalcContext.Operatives.AddAsync(operative);
            await BonusCalcContext.SaveChangesAsync();

            return operative;
        }
    }
}
