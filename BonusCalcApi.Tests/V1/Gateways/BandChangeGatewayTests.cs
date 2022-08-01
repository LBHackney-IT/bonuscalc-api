using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BonusCalcApi.Tests.V1.Gateways
{
    [TestFixture]
    public class BandChangeGatewayTests : DatabaseTests
    {
        private BandChangeGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new BandChangeGateway(BonusCalcContext);
        }

        [Test]
        public async Task RetrievesBandChangesFromDb()
        {
            // Arrange
            var bandChanges = await SeedBandChanges();

            // Act
            var result = await _classUnderTest.GetBandChangesAsync("2021-08-02");

            // Assert
            result.Should().BeEquivalentTo(bandChanges);
        }

        [Test]
        public async Task DoesNotReturnApprovalsAsAuthorisationsFromDb()
        {
            // Arrange
            var bandChange = await SeedBandChange();

            bandChange.Supervisor = new BandChangeApprover
            {
                Name = "A Supervisor",
                EmailAddress = "a.supervisor@hackney.gov.uk",
                Decision = BandChangeDecision.Approved
            };

            await BonusCalcContext.SaveChangesAsync();

            // Act
            var result = await _classUnderTest.GetBandChangeAuthorisationsAsync("2021-08-02");

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public async Task DoesNotReturnDownwardRejectionsAsAuthorisationsFromDb()
        {
            // Arrange
            var bandChange = await SeedBandChange();

            bandChange.Supervisor = new BandChangeApprover
            {
                Name = "A Supervisor",
                EmailAddress = "a.supervisor@hackney.gov.uk",
                Decision = BandChangeDecision.Rejected,
                Reason = "Some reason",
                SalaryBand = 5
            };

            await BonusCalcContext.SaveChangesAsync();

            // Act
            var result = await _classUnderTest.GetBandChangeAuthorisationsAsync("2021-08-02");

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public async Task ReturnsUpwardsRejectionsAsAuthorisationsFromDb()
        {
            // Arrange
            var bandChange = await SeedBandChange();
            var bandChanges = new List<BandChange> { bandChange };

            bandChange.Supervisor = new BandChangeApprover
            {
                Name = "A Supervisor",
                EmailAddress = "a.supervisor@hackney.gov.uk",
                Decision = BandChangeDecision.Rejected,
                Reason = "Some reason",                
                SalaryBand = 7
            };

            await BonusCalcContext.SaveChangesAsync();

            // Act
            var result = await _classUnderTest.GetBandChangeAuthorisationsAsync("2021-08-02");

            // Assert
            result.Should().BeEquivalentTo(bandChanges);
        }

        [Test]
        public async Task RetrievesBandChangeFromDb()
        {
            // Arrange
            var bandChange = await SeedBandChange();

            // Act
            var result = await _classUnderTest.GetBandChangeAsync("2021-08-02", "123456");

            // Assert
            result.Should().BeEquivalentTo(bandChange);
        }

        private async Task<IEnumerable<BandChange>> SeedBandChanges()
        {
            var bandChange = await SeedBandChange();
            return new List<BandChange> { bandChange };
        }

        private async Task<BandChange> SeedBandChange()
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

            var bonusPeriod = new BonusPeriod
            {
                Id = "2021-08-02",
                StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                Year = 2021,
                Number = 3,
                ClosedAt = null
            };

            var bandChange = new BandChange
            {
                Id = "123456/2021-08-02",
                BonusPeriodId = "2021-08-02",
                OperativeId = "123456",
                Trade = "Electrician (EL)",
                Scheme = "Reactive",
                BandValue = 50544.0M,
                MaxValue = 62868.0M,
                SickDuration = 0.0M,
                TotalValue = 51140.9748M,
                Utilisation = 1.0M,
                FixedBand = false,
                SalaryBand = 7,
                ProjectedBand = 6,
                Supervisor = new BandChangeApprover
                {
                    Name = "A Supervisor",
                    EmailAddress = "a.supervisor@hackney.gov.uk"
                },
                Manager = new BandChangeApprover
                {
                    Name = "A Manager",
                    EmailAddress = "a.manager@hackney.gov.uk"
                }
            };

            await BonusCalcContext.BonusPeriods.AddAsync(bonusPeriod);
            await BonusCalcContext.Operatives.AddAsync(operative);
            await BonusCalcContext.BandChanges.AddAsync(bandChange);
            await BonusCalcContext.SaveChangesAsync();

            return bandChange;
        }
    }
}
