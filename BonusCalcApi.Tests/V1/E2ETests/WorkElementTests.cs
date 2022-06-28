using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Infrastructure;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.E2ETests
{
    public class WorkElementResponseComparer : IEqualityComparer<WorkElementResponse>
    {
        public bool Equals(WorkElementResponse we1, WorkElementResponse we2)
        {
            return we2.WorkOrder == we1.WorkOrder;
        }

        public int GetHashCode(WorkElementResponse we)
        {
            return we.WorkOrder.GetHashCode();
        }
    }

    public class WorkElementTests : IntegrationTests<Startup>
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task CanGetWorkElements()
        {
            // Arrange
            await SeedPayElements();

            var workElement = new WorkElementResponse()
            {
                WorkOrder = "12345678"
            };

            var otherWorkElement = new WorkElementResponse()
            {
                WorkOrder = "99999999"
            };

            var laterWorkElement = new WorkElementResponse()
            {
                WorkOrder = "23456789"
            };

            var comparer = new WorkElementResponseComparer();

            // Act
            var (code, response) = await Get<List<WorkElementResponse>>($"/api/v1/work/elements?query=Somewhere");

            // Assert
            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Contains.Item(workElement).Using(comparer));
            Assert.That(response, Contains.Item(laterWorkElement).Using(comparer));
            Assert.That(response, Does.Not.Contain(otherWorkElement).Using(comparer));
            Assert.That(response, Is.Ordered.Descending.By("ClosedAt"));
        }

        private async Task SeedPayElements()
        {
            var payElementType = new PayElementType
            {
                Id = 301,
                Description = "Reactive Repairs",
                PayAtBand = false,
                Paid = true,
                Adjustment = false,
                Productive = true,
                NonProductive = false,
                OutOfHours = false,
                Overtime = false,
                Selectable = true,
                SmvPerHour = null,
                CostCode = null
            };

            var timesheet = new Timesheet
            {
                Id = "123456/2021-10-18",
                Week = new Week
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
                    ClosedAt = null
                },
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
                }
            };

            var payElements = new List<PayElement>()
            {
                new PayElement()
                {
                    Timesheet = timesheet,
                    PayElementType = payElementType,
                    WorkOrder = "23456789",
                    Address = "Somewhere Road",
                    ClosedAt = new DateTime(2021, 10, 20, 11, 0, 0, DateTimeKind.Utc),
                    Monday = 1.0M,
                    Duration = 1.0M,
                    Value = 60.0M,
                    ReadOnly = true
                },
                new PayElement()
                {
                    Timesheet = timesheet,
                    PayElementType = payElementType,
                    WorkOrder = "12345678",
                    Address = "Somewhere Road",
                    ClosedAt = new DateTime(2021, 10, 18, 11, 0, 0, DateTimeKind.Utc),
                    Monday = 1.0M,
                    Duration = 1.0M,
                    Value = 60.0M,
                    ReadOnly = true
                },
                new PayElement()
                {
                    Timesheet = timesheet,
                    PayElementType = payElementType,
                    WorkOrder = "99999999",
                    Address = "Other Place",
                    ClosedAt = new DateTime(2021, 10, 19, 11, 0, 0, DateTimeKind.Utc),
                    Tuesday = 1.0M,
                    Duration = 1.0M,
                    Value = 60.0M,
                    ReadOnly = true
                }
            };

            await Context.PayElementTypes.AddAsync(payElementType);
            await Context.Timesheets.AddAsync(timesheet);
            await Context.PayElements.AddRangeAsync(payElements);
            await Context.SaveChangesAsync();
        }
    }
}
