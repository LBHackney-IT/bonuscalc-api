using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace BonusCalcApi.Tests.V1.E2ETests
{
    public class PayElementTypeTests : IntegrationTests<Startup>
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
        }

        [Test]
        public async Task CanGetPayElementTypes()
        {
            // Arrange
            var payElementType = await SeedPayElementTypes();

            // Act
            var (code, response) = await Get<IEnumerable<PayElementType>>($"/api/v1/pay/type");

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            response.Should().BeEquivalentTo(payElementType);
        }
        private async Task<IEnumerable<PayElementType>> SeedPayElementTypes()
        {
            var types = _fixture.Build<PayElementType>()
                .Without(pet => pet.PayElements)
                .CreateMany();
            await Context.PayElementTypes.AddRangeAsync(types);
            await Context.SaveChangesAsync();
            return types;
        }
    }
}
