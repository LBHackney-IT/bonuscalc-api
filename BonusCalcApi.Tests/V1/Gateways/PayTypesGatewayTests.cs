using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BonusCalcApi.Tests.V1.Gateways
{
    public class PayTypesGatewayTests
    {
        private PayElementTypeGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _classUnderTest = new PayElementTypeGateway(InMemoryDb.Instance);
        }

        [Test]
        public void CanGetPayElementTypes()
        {
            // Arrange
            var expectedPayElements = new List<PayElementType>();
        }
    }
}
