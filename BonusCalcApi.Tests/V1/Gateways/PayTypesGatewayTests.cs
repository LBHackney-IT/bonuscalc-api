using AutoFixture;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BonusCalcApi.Tests.V1.Gateways
{
    public class PayTypesGatewayTests
    {
        private Fixture _fixture;
        private PayElementTypeGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();// FixtureHelpers.Fixture;
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior()); // remove when line above is replaced

            _classUnderTest = new PayElementTypeGateway(InMemoryDb.Instance);
        }

        [Test]
        public async Task CanGetPayElementTypesAsync()
        {
            // Arrange
            var expectedPayElements = _fixture.Build<PayElementType>()
                .Without(pet => pet.PayElements)
                .CreateMany();
            await InMemoryDb.Instance.PayElementTypes.AddRangeAsync(expectedPayElements);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            var result = await _classUnderTest.GetPayElementTypesAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedPayElements);
        }
    }
}
