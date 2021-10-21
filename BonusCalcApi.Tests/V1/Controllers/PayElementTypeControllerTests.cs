using AutoFixture;
using BonusCalcApi.Tests.V1.Controllers.Mocks;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Controllers;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BonusCalcApi.Tests.V1.Controllers
{
    [TestFixture]
    public class PayElementTypeControllerTests : ControllerTests
    {
        private Fixture _fixture;
        private Mock<IGetPayElementTypeUseCase> _getPayElementTypeUseCaseMock;
        private MockProblemDetailsFactory _problemDetailsFactoryMock;

        private PayElementTypeController _classUnderTest;
        //private MockOperativeHelpers _operativeHelpers;
        //private Mock<IGetPayElementTypeUseCase> _getOperativesTimesheetUseCaseMock;

        [SetUp]
        public void SetUp()
        {
            _fixture = FixtureHelpers.Fixture;
            _getPayElementTypeUseCaseMock = new Mock<IGetPayElementTypeUseCase>();
            //_getOperativesTimesheetUseCaseMock = new Mock<IGetOperativeTimesheetUseCase>();
            //_operativeHelpers = new MockOperativeHelpers();
            _problemDetailsFactoryMock = new MockProblemDetailsFactory();

            _classUnderTest = new PayElementTypeController(_getPayElementTypeUseCaseMock.Object);

            // .NET 3.1 doesn't set ProblemDetailsFactory so we need to mock it
            _classUnderTest.ProblemDetailsFactory = _problemDetailsFactoryMock.Object;
        }
        [Test]
        public async Task GetsPayElementTypes()
        {
            // Arrange
            var expectedPaymentElementTypes = _fixture.CreateMany<PayElementType>();
            _getPayElementTypeUseCaseMock.Setup(x => x.ExecuteAsync())
                .ReturnsAsync(expectedPaymentElementTypes);

            // Act
            var objectResult = await _classUnderTest.GetPayElementType();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<IEnumerable<PayElementType>>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedPaymentElementTypes);
        }
    }
}
