using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BonusCalcApi.Tests.V1.Gateways
{
    [TestFixture]
    public class PayElementTypeGatewayTests : DatabaseTests
    {
        private PayElementTypeGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new PayElementTypeGateway(BonusCalcContext);
        }

        [Test]
        public async Task RetrievesPayElementTypesFromDB()
        {
            // Arrange
            var payElementTypes = await AddPayElementTypes();

            // Act
            var result = await _classUnderTest.GetPayElementTypesAsync();

            // Assert
            result.Should().BeEquivalentTo(payElementTypes);
        }

        private async Task<IEnumerable<PayElementType>> AddPayElementTypes()
        {
            var payElementTypes = new List<PayElementType>()
            {
                new PayElementType
                {
                    Id = 101,
                    Description = "Dayworks",
                    PayAtBand = false,
                    Paid = true,
                    Adjustment = false,
                    Productive = false,
                    NonProductive = true,
                    OutOfHours = false,
                    Overtime = false,
                    Selectable = true,
                    SmvPerHour = null
                },
                new PayElementType
                {
                    Id = 132,
                    Description = "Apprentice",
                    PayAtBand = false,
                    Paid = true,
                    Adjustment = false,
                    Productive = false,
                    NonProductive = true,
                    OutOfHours = false,
                    Overtime = false,
                    Selectable = true,
                    SmvPerHour = 60
                },
                new PayElementType
                {
                    Id = 201,
                    Description = "Manual Adjustment",
                    PayAtBand = false,
                    Paid = false,
                    Adjustment = true,
                    Productive = true,
                    NonProductive = false,
                    OutOfHours = false,
                    Overtime = false,
                    Selectable = true,
                    SmvPerHour = null
                },
                new PayElementType
                {
                    Id = 301,
                    Description = "Reactive Repairs",
                    PayAtBand = true,
                    Paid = true,
                    Adjustment = false,
                    Productive = true,
                    NonProductive = false,
                    OutOfHours = false,
                    Overtime = false,
                    SmvPerHour = null
                }
            };

            await BonusCalcContext.PayElementTypes.AddRangeAsync(payElementTypes);
            await BonusCalcContext.SaveChangesAsync();

            return payElementTypes;
        }
    }
}
