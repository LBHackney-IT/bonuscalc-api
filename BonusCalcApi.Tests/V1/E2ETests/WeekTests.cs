using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.E2ETests
{
    public class WeekResponseComparer : IEqualityComparer<WeekResponse>
    {
        public bool Equals(WeekResponse w1, WeekResponse w2)
        {
            return w2.Id == w1.Id
                && w2.StartAt == w1.StartAt
                && w2.ClosedAt == w1.ClosedAt
                && w2.BonusPeriod?.Id == w1.BonusPeriod?.Id;
        }

        public int GetHashCode(WeekResponse w)
        {
            return w.Id.GetHashCode();
        }
    }

    public class OperativeSummaryResponseComparer : IEqualityComparer<OperativeSummaryResponse>
    {
        public bool Equals(OperativeSummaryResponse o1, OperativeSummaryResponse o2)
        {
            return o2.Id == o1.Id && o2.Name == o1.Name;
        }

        public int GetHashCode(OperativeSummaryResponse o)
        {
            return o.Id.GetHashCode();
        }
    }

    public class OutOfHoursSummaryResponseComparer : IEqualityComparer<OutOfHoursSummaryResponse>
    {
        public bool Equals(OutOfHoursSummaryResponse o1, OutOfHoursSummaryResponse o2)
        {
            return o2.Id == o1.Id && o2.Name == o1.Name;
        }

        public int GetHashCode(OutOfHoursSummaryResponse o)
        {
            return o.Id.GetHashCode();
        }
    }

    public class OvertimeSummaryResponseComparer : IEqualityComparer<OvertimeSummaryResponse>
    {
        public bool Equals(OvertimeSummaryResponse o1, OvertimeSummaryResponse o2)
        {
            return o2.Id == o1.Id && o2.Name == o1.Name;
        }

        public int GetHashCode(OvertimeSummaryResponse o)
        {
            return o.Id.GetHashCode();
        }
    }

    public class WeekTests : IntegrationTests<Startup>
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task CanGetWeek()
        {
            // Arrange
            var week = await SeedWeek();
            var operative = await SeedOperative();
            var comparer = new WeekResponseComparer();
            var operativeComparer = new OperativeSummaryResponseComparer();
            var operativeSummary = new OperativeSummaryResponse()
            {
                Id = operative.Id,
                Name = operative.Name
            };

            // Act
            var (code, response) = await Get<WeekResponse>($"/api/v1/weeks/2021-10-18");

            // Assert
            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.EqualTo(week.ToResponse()).Using(comparer));
            Assert.That(response.OperativeSummaries, Contains.Item(operativeSummary).Using(operativeComparer));
        }

        [Test]
        public async Task CanUpdateWeek()
        {
            // Arrange
            var week = await SeedWeek();
            var update = new WeekUpdate()
            {
                ClosedAt = new DateTime(2021, 10, 29, 16, 0, 0, DateTimeKind.Utc),
                ClosedBy = "a.manager@hackney.gov.uk"
            };

            // Act
            var (code, response) = await Post<WeekResponse>($"/api/v1/weeks/2021-10-18", update);

            // Assert
            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.ClosedAt, Is.EqualTo(update.ClosedAt));
            Assert.That(response.ClosedBy, Is.EqualTo(update.ClosedBy));
        }

        [Test]
        public async Task CanUpdateReportsSentAt()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var week = await SeedWeek();

            // Act
            var (postCode, _) = await Post<WeekResponse>($"/api/v1/weeks/2021-10-18/reports", null);
            var (getCode, response) = await Get<WeekResponse>($"/api/v1/weeks/2021-10-18");

            // Assert
            postCode.Should().Be(HttpStatusCode.OK);
            getCode.Should().Be(HttpStatusCode.OK);
            response.ReportsSentAt.Should().BeOnOrAfter(now);
        }

        [Test]
        public async Task CanGetOutOfHoursSummaries()
        {
            // Arrange
            await SeedOutOfHoursPayElements();

            var outOfHoursSummary = new OutOfHoursSummaryResponse()
            {
                Id = "123456",
                Name = "An Operative"
            };

            var otherOutOfHoursSummary = new OutOfHoursSummaryResponse()
            {
                Id = "118118",
                Name = "Other Operative"
            };

            var laterOutOfHoursSummary = new OutOfHoursSummaryResponse()
            {
                Id = "123123",
                Name = "Later Operative"
            };

            var comparer = new OutOfHoursSummaryResponseComparer();

            // Act
            var (code, response) = await Get<List<OutOfHoursSummaryResponse>>($"/api/v1/weeks/2021-10-18/out-of-hours");

            // Assert
            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Contains.Item(outOfHoursSummary).Using(comparer));
            Assert.That(response, Contains.Item(otherOutOfHoursSummary).Using(comparer));
            Assert.That(response, Does.Not.Contain(laterOutOfHoursSummary).Using(comparer));
            Assert.That(response, Is.Ordered.Ascending.By("Id"));
        }

        [Test]
        public async Task CanGetOvertimeSummaries()
        {
            // Arrange
            await SeedOvertimePayElements();

            var overtimeSummary = new OvertimeSummaryResponse()
            {
                Id = "123456",
                Name = "An Operative"
            };

            var otherOvertimeSummary = new OvertimeSummaryResponse()
            {
                Id = "118118",
                Name = "Other Operative"
            };

            var laterOvertimeSummary = new OvertimeSummaryResponse()
            {
                Id = "123123",
                Name = "Later Operative"
            };

            var comparer = new OvertimeSummaryResponseComparer();

            // Act
            var (code, response) = await Get<List<OvertimeSummaryResponse>>($"/api/v1/weeks/2021-10-18/overtime");

            // Assert
            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Contains.Item(overtimeSummary).Using(comparer));
            Assert.That(response, Contains.Item(otherOvertimeSummary).Using(comparer));
            Assert.That(response, Does.Not.Contain(laterOvertimeSummary).Using(comparer));
            Assert.That(response, Is.Ordered.Ascending.By("Id"));
        }

        private async Task<Week> SeedWeek()
        {
            var week = new Week
            {
                Id = "2021-10-18",
                BonusPeriod = new BonusPeriod
                {
                    Id = "2021-08-02",
                    StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                    Year = 2020,
                    Number = 3,
                    ClosedAt = null
                },
                StartAt = new DateTime(2021, 10, 17, 23, 0, 0, DateTimeKind.Utc),
                Number = 12,
                ClosedAt = null,
                ReportsSentAt = null
            };

            await Context.Weeks.AddAsync(week);
            await Context.SaveChangesAsync();

            return week;
        }

        private async Task<Operative> SeedOperative()
        {
            var operative = new Operative
            {
                Id = "123456",
                Name = "An Operative",
                EmailAddress = "an.operative@hackney.gov.uk",
                Trade = new Trade
                {
                    Id = "CP",
                    Description = "Carpenter"
                },
                Scheme = new Scheme
                {
                    Id = 1,
                    Type = "SMV",
                    Description = "Reactive",
                    ConversionFactor = 1.0M,
                    MaxValue = 62868.0M
                },
                Section = "H3007",
                SalaryBand = 5,
                Utilisation = 1.0M,
                FixedBand = false,
                IsArchived = false
            };

            var timesheet = new Timesheet
            {
                Id = "123456/2021-10-18",
                OperativeId = "123456",
                WeekId = "2021-10-18",
                Utilisation = 1.0M
            };

            await Context.Operatives.AddAsync(operative);
            await Context.Timesheets.AddAsync(timesheet);
            await Context.SaveChangesAsync();

            return operative;
        }

        private async Task SeedOutOfHoursPayElements()
        {
            var bonusPeriod = new BonusPeriod
            {
                Id = "2021-08-02",
                StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                Year = 2020,
                Number = 3,
                ClosedAt = null
            };

            var week = new Week
            {
                Id = "2021-10-18",
                BonusPeriod = bonusPeriod,
                StartAt = new DateTime(2021, 10, 17, 23, 0, 0, DateTimeKind.Utc),
                Number = 12,
                ClosedAt = null
            };

            var payElementType = new PayElementType
            {
                Id = 501,
                Description = "OOH Job",
                PayAtBand = false,
                Paid = false,
                Adjustment = false,
                Productive = false,
                NonProductive = false,
                OutOfHours = true,
                Overtime = false,
                Selectable = false,
                SmvPerHour = null,
                CostCode = null
            };

            var scheme = new Scheme
            {
                Id = 1,
                Type = "SMV",
                Description = "Reactive",
                ConversionFactor = 1.0M
            };

            var timesheets = new List<Timesheet>()
            {
                new Timesheet
                {
                    Id = "123456/2021-10-18",
                    Week = week,
                    Operative = new Operative
                    {
                        Id = "123456",
                        Name = "An Operative",
                        EmailAddress = "an.operative@hackney.gov.uk",
                        Trade = new Trade
                        {
                            Id = "CP",
                            Description = "Carpenter"
                        },
                        Scheme = scheme,
                        Section = "H3007",
                        SalaryBand = 5,
                        Utilisation = 1.0M,
                        FixedBand = false,
                        IsArchived = false
                    },
                    PayElements = new List<PayElement>()
                    {
                        new PayElement
                        {
                            PayElementType = payElementType,
                            Value = 20.0M,
                            ReadOnly = true
                        }
                    }
                },
                new Timesheet
                {
                    Id = "118118/2021-10-18",
                    Week = week,
                    Operative = new Operative
                    {
                        Id = "118118",
                        Name = "Other Operative",
                        EmailAddress = "other.operative@hackney.gov.uk",
                        Trade = new Trade
                        {
                            Id = "MT",
                            Description = "Multi-Trade"
                        },
                        Scheme = scheme,
                        Section = "H3002",
                        SalaryBand = 5,
                        Utilisation = 1.0M,
                        FixedBand = false,
                        IsArchived = false
                    },
                    PayElements = new List<PayElement>()
                    {
                        new PayElement
                        {
                            PayElementType = payElementType,
                            Value = 20.0M,
                            ReadOnly = true
                        }
                    }
                },
                new Timesheet
                {
                    Id = "123123/2021-10-25",
                    Week = new Week
                    {
                        Id = "2021-10-25",
                        BonusPeriod = bonusPeriod,
                        StartAt = new DateTime(2021, 10, 24, 23, 0, 0, DateTimeKind.Utc),
                        Number = 13,
                        ClosedAt = null
                    },
                    Operative = new Operative
                    {
                        Id = "123123",
                        Name = "Later Operative",
                        EmailAddress = "later.operative@hackney.gov.uk",
                        Trade = new Trade
                        {
                            Id = "PL",
                            Description = "Plumber"
                        },
                        Scheme = scheme,
                        Section = "H3009",
                        SalaryBand = 5,
                        Utilisation = 1.0M,
                        FixedBand = false,
                        IsArchived = false
                    },
                    PayElements = new List<PayElement>()
                    {
                        new PayElement
                        {
                            PayElementType = payElementType,
                            Value = 20.0M,
                            ReadOnly = true
                        }
                    }
                }
            };

            await Context.BonusPeriods.AddAsync(bonusPeriod);
            await Context.Weeks.AddAsync(week);
            await Context.PayElementTypes.AddAsync(payElementType);
            await Context.Schemes.AddAsync(scheme);
            await Context.Timesheets.AddRangeAsync(timesheets);
            await Context.SaveChangesAsync();
        }

        private async Task SeedOvertimePayElements()
        {
            var bonusPeriod = new BonusPeriod
            {
                Id = "2021-08-02",
                StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                Year = 2020,
                Number = 3,
                ClosedAt = null
            };

            var week = new Week
            {
                Id = "2021-10-18",
                BonusPeriod = bonusPeriod,
                StartAt = new DateTime(2021, 10, 17, 23, 0, 0, DateTimeKind.Utc),
                Number = 12,
                ClosedAt = null
            };

            var payElementType = new PayElementType
            {
                Id = 401,
                Description = "Overtime Job",
                PayAtBand = false,
                Paid = false,
                Adjustment = false,
                Productive = false,
                NonProductive = false,
                OutOfHours = false,
                Overtime = true,
                Selectable = false,
                SmvPerHour = null,
                CostCode = null
            };

            var scheme = new Scheme
            {
                Id = 1,
                Type = "SMV",
                Description = "Reactive",
                ConversionFactor = 1.0M
            };

            var timesheets = new List<Timesheet>()
            {
                new Timesheet
                {
                    Id = "123456/2021-10-18",
                    Week = week,
                    Operative = new Operative
                    {
                        Id = "123456",
                        Name = "An Operative",
                        EmailAddress = "an.operative@hackney.gov.uk",
                        Trade = new Trade
                        {
                            Id = "CP",
                            Description = "Carpenter"
                        },
                        Scheme = scheme,
                        Section = "H3007",
                        SalaryBand = 5,
                        Utilisation = 1.0M,
                        FixedBand = false,
                        IsArchived = false
                    },
                    PayElements = new List<PayElement>()
                    {
                        new PayElement
                        {
                            PayElementType = payElementType,
                            Value = 22.86M,
                            ReadOnly = true
                        }
                    }
                },
                new Timesheet
                {
                    Id = "118118/2021-10-18",
                    Week = week,
                    Operative = new Operative
                    {
                        Id = "118118",
                        Name = "Other Operative",
                        EmailAddress = "other.operative@hackney.gov.uk",
                        Trade = new Trade
                        {
                            Id = "MT",
                            Description = "Multi-Trade"
                        },
                        Scheme = scheme,
                        Section = "H3002",
                        SalaryBand = 5,
                        Utilisation = 1.0M,
                        FixedBand = false,
                        IsArchived = false
                    },
                    PayElements = new List<PayElement>()
                    {
                        new PayElement
                        {
                            PayElementType = payElementType,
                            Value = 22.86M,
                            ReadOnly = true
                        }
                    }
                },
                new Timesheet
                {
                    Id = "123123/2021-10-25",
                    Week = new Week
                    {
                        Id = "2021-10-25",
                        BonusPeriod = bonusPeriod,
                        StartAt = new DateTime(2021, 10, 24, 23, 0, 0, DateTimeKind.Utc),
                        Number = 13,
                        ClosedAt = null
                    },
                    Operative = new Operative
                    {
                        Id = "123123",
                        Name = "Later Operative",
                        EmailAddress = "later.operative@hackney.gov.uk",
                        Trade = new Trade
                        {
                            Id = "PL",
                            Description = "Plumber"
                        },
                        Scheme = scheme,
                        Section = "H3009",
                        SalaryBand = 5,
                        Utilisation = 1.0M,
                        FixedBand = false,
                        IsArchived = false
                    },
                    PayElements = new List<PayElement>()
                    {
                        new PayElement
                        {
                            PayElementType = payElementType,
                            Value = 22.86M,
                            ReadOnly = true
                        }
                    }
                }
            };

            await Context.BonusPeriods.AddAsync(bonusPeriod);
            await Context.Weeks.AddAsync(week);
            await Context.PayElementTypes.AddAsync(payElementType);
            await Context.Schemes.AddAsync(scheme);
            await Context.Timesheets.AddRangeAsync(timesheets);
            await Context.SaveChangesAsync();
        }
    }
}
