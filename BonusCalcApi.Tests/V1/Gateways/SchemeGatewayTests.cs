using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BonusCalcApi.Tests.V1.Gateways
{
    [TestFixture]
    public class SchemeGatewayTests : DatabaseTests
    {
        private SchemeGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new SchemeGateway(BonusCalcContext);
        }

        [Test]
        public async Task RetrievesSchemesFromDB()
        {
            // Arrange
            var schemes = await AddSchemes();

            // Act
            var result = await _classUnderTest.GetSchemesAsync();

            // Assert
            result.Should().BeEquivalentTo(schemes);
        }

        private async Task<IEnumerable<Scheme>> AddSchemes()
        {
            var schemes = new List<Scheme>()
            {
                new Scheme
                {
                    Id = 1,
                    Type = "SMV",
                    Description = "Planned",
                    ConversionFactor = 1.0M,
                    MaxValue = 60372.0M,
                    PayBands = new List<PayBand>()
                    {
                        new PayBand { Id = 11, Band = 1, Value = 2160.0M },
                        new PayBand { Id = 12, Band = 2, Value = 2700.0M },
                        new PayBand { Id = 13, Band = 3, Value = 3132.0M },
                        new PayBand { Id = 14, Band = 4, Value = 3348.0M },
                        new PayBand { Id = 15, Band = 5, Value = 3564.0M },
                        new PayBand { Id = 16, Band = 6, Value = 3780.0M },
                        new PayBand { Id = 17, Band = 7, Value = 3996.0M },
                        new PayBand { Id = 18, Band = 8, Value = 4320.0M },
                        new PayBand { Id = 19, Band = 9, Value = 4644.0M }
                    }
                },
                new Scheme
                {
                    Id = 2,
                    Type = "SMV",
                    Description = "Reactive",
                    ConversionFactor = 1.0M,
                    MaxValue = 62868.0M,
                    PayBands = new List<PayBand>()
                    {
                        new PayBand { Id = 21, Band = 1, Value = 2160.0M },
                        new PayBand { Id = 22, Band = 2, Value = 2772.0M },
                        new PayBand { Id = 23, Band = 3, Value = 3132.0M },
                        new PayBand { Id = 24, Band = 4, Value = 3366.0M },
                        new PayBand { Id = 25, Band = 5, Value = 3618.0M },
                        new PayBand { Id = 26, Band = 6, Value = 3888.0M },
                        new PayBand { Id = 27, Band = 7, Value = 4182.0M },
                        new PayBand { Id = 28, Band = 8, Value = 4494.0M },
                        new PayBand { Id = 29, Band = 9, Value = 4836.0M }
                    }
                }
            };

            await BonusCalcContext.Schemes.AddRangeAsync(schemes);
            await BonusCalcContext.SaveChangesAsync();

            return schemes;
        }
    }
}
