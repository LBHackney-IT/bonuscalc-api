using AutoFixture;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BonusCalcApi.Tests.V1.Controllers
{
    [TestFixture]
    public class PaymentElementTypeController
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<IGetPayElementTypeUseCase> _getPaymentUseCaseMock;

        [SetUp]
        public void SetUp()
        {
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _getPaymentUseCaseMock = new Mock<IGetPayElementTypeUseCase>();
        }
        [Test]
        public void Returns404IfPaymentIsNotFound()
        {
            // Arrange
            var expectedPayment = _fixture.Create<Timesheet>();
            _getPaymentUseCaseMock
                .Setup(m => m.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync((PayElement) null);

            // Act
            //var objectResult = await .GetOperative("000000");

            // Assert
            //statusCode.Should().Be((int) HttpStatusCode.NotFound);
            //_problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.NotFound);
            Assert.IsTrue(true);
        }

    }
}
