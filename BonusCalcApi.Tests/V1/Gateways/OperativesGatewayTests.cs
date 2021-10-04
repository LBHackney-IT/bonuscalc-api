using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.UseCase;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.Gateways
{
    [TestFixture]
    public class OperativesGatewayTests
    {
        private OperativesGateway _sut;
        private Mock<IApiGateway> _apiGatewayMock;
        private Mock<ILogger<OperativesGateway>> _loggerMock;
        private Mock<IOptions<OperativesGatewayOptions>> _gatewayOptionsMock;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _apiGatewayMock = new Mock<IApiGateway>();
            _loggerMock = new Mock<ILogger<OperativesGateway>>();
            _gatewayOptionsMock = new Mock<IOptions<OperativesGatewayOptions>>();


            _gatewayOptionsMock.Setup(gwo => gwo.Value)
                .Returns(new OperativesGatewayOptions
                {
                    RepairsHubBaseAddr = "http://null/",
                    RepairsHubApiKey = "-"
                });

            _sut = new OperativesGateway(_apiGatewayMock.Object, _loggerMock.Object, _gatewayOptionsMock.Object);
        }

        [Test]
        public async Task ShouldLogWarningAndReturnNullIfNoOperativeFound()
        {

            //Arrange
            _apiGatewayMock.Setup(api => api.ExecuteRequest<OperativeResponse>(Moq.It.IsAny<string>(), Moq.It.IsAny<Uri>()))
                .Returns(Task.FromResult(new ApiResponse<OperativeResponse>(false, System.Net.HttpStatusCode.NotFound, null)));


            //Act
            var result = await _sut.Execute("3561577").ConfigureAwait(false);

            //Assert
            result.Should().BeNull();

        }

        [Test]
        public void ShouldThrowExceptionIfOtherNonSuccessReturn()
        {
            //Arrange
            _apiGatewayMock.Setup(api => api.ExecuteRequest<OperativeResponse>(Moq.It.IsAny<string>(), Moq.It.IsAny<Uri>()))
                .Returns(Task.FromResult(new ApiResponse<OperativeResponse>(false, System.Net.HttpStatusCode.InternalServerError, null)));

            //Act & Assert
            Assert.Throws<ApiException>(() => _sut.Execute("435436456").GetAwaiter().GetResult());

        }

        [Test]
        public async Task ShouldReturnCorrectNameIfGatewaySuccessfullyReturnsObject()
        {
            //Arrange
            var operative = _fixture.Create<OperativeResponse>();

            _apiGatewayMock.Setup(api => api.ExecuteRequest<OperativeResponse>(Moq.It.IsAny<string>(), Moq.It.IsAny<Uri>()))
                .Returns(Task.FromResult(new ApiResponse<OperativeResponse>(true, System.Net.HttpStatusCode.OK, operative)));


            //Act
            var result = await _sut.Execute("23324534").ConfigureAwait(false);

            //Assert
            result.Name.Should().Be(operative.Name);
        }

        [Test]
        public async Task ShouldReturnCorrectTypeIfGatewaySuccessfullyReturnsObject()
        {
            //Arrange
            var operative = _fixture.Create<OperativeResponse>();

            _apiGatewayMock.Setup(api => api.ExecuteRequest<OperativeResponse>(Moq.It.IsAny<string>(), Moq.It.IsAny<Uri>()))
                .Returns(Task.FromResult(new ApiResponse<OperativeResponse>(true, System.Net.HttpStatusCode.OK, operative)));


            //Act
            var result = await _sut.Execute("23324534").ConfigureAwait(false);

            //Assert
            result.Should().BeOfType<OperativeResponse>();
        }

    }
}
