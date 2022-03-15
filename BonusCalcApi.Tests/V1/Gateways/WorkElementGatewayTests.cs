using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.Gateways
{
    public class WorkElementComparer : IEqualityComparer<WorkElement>
    {
        public bool Equals(WorkElement we1, WorkElement we2)
        {
            return we2.WorkOrder == we1.WorkOrder;
        }

        public int GetHashCode(WorkElement we)
        {
            return we.WorkOrder.GetHashCode();
        }
    }

    [TestFixture]
    public class WorkElementGatewayTests : DatabaseTests
    {
        private WorkElementGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new WorkElementGateway(BonusCalcContext);
        }

        [Test]
        public async Task RetrievesWorkElementsFromDB()
        {
            // Arrange
            await AddPayElements();

            var workElement = new WorkElement()
            {
                WorkOrder = "12345678"
            };

            var otherWorkElement = new WorkElement()
            {
                WorkOrder = "99999999"
            };

            var comparer = new WorkElementComparer();

            // Act
            var result = await _classUnderTest.GetWorkElementsAsync("12345678", 1, 25);

            // Assert
            Assert.That(result, Contains.Item(workElement).Using(comparer));
            Assert.That(result, Does.Not.Contain(otherWorkElement).Using(comparer));
        }

        [Test]
        public async Task RetrievesEmptyResultsFromDB()
        {
            // Act
            var result = await _classUnderTest.GetWorkElementsAsync("00000000", 1, 25);

            // Assert
            Assert.That(result, Is.Empty);
        }

        private async Task AddPayElements()
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
                        Id = "EL",
                        Description = "Electrician"
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
                    WorkOrder = "12345678",
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
                    ClosedAt = new DateTime(2021, 10, 19, 11, 0, 0, DateTimeKind.Utc),
                    Tuesday = 1.0M,
                    Duration = 1.0M,
                    Value = 60.0M,
                    ReadOnly = true
                }
            };

            await BonusCalcContext.PayElementTypes.AddAsync(payElementType);
            await BonusCalcContext.Timesheets.AddAsync(timesheet);
            await BonusCalcContext.PayElements.AddRangeAsync(payElements);
            await BonusCalcContext.SaveChangesAsync();
        }
    }
}
