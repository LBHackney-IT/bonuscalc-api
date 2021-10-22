using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.UseCase
{
    public class UpdateTimesheetUseCaseTests
    {
        private UpdateTimesheetUseCase _classUnderTest;
        private Fixture _fixture;
        private Mock<ITimesheetGateway> _timesheetGatewayMock;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;

            _timesheetGatewayMock = new Mock<ITimesheetGateway>();

            _classUnderTest = new UpdateTimesheetUseCase(_timesheetGatewayMock.Object, InMemoryDb.DbSaver);
        }

        [TearDown]
        public void Teardown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task InsertsNewPayElements()
        {
            // Arrange
            var existingTimesheet = CreateExistingTimesheet();

            var newPayElement = _fixture.Build<PayElementUpdate>()
                .Without(x => x.Id)
                .Create();
            var request = CreateRequest(newPayElement);

            // Act
            await _classUnderTest.Execute(request, existingTimesheet.OperativeId, existingTimesheet.WeekId);

            // Assert
            var payElement = existingTimesheet.PayElements.Single();
            payElement.Should().BeEquivalentTo(newPayElement.ToDb(), options =>
                options.Excluding(pe => pe.Id)
            );
            InMemoryDb.DbSaver.VerifySaveCalled();
        }

        [Test]
        public async Task RemovesAllPayElements()
        {
            // Arrange
            var existingPayElement = _fixture.Create<PayElement>();
            existingPayElement.ReadOnly = false;
            var existingTimesheet = CreateExistingTimesheet();
            existingTimesheet.PayElements = new List<PayElement>()
            {
                existingPayElement
            };

            var request = new TimesheetUpdateRequest();

            // Act
            await _classUnderTest.Execute(request, existingTimesheet.OperativeId, existingTimesheet.WeekId);

            // Assert
            existingTimesheet.PayElements.Should().BeEmpty();
            InMemoryDb.DbSaver.VerifySaveCalled();
        }

        [Test]
        public async Task UpdatesExistingPayElements()
        {
            // Arrange
            var existingPayElement = _fixture.Create<PayElement>();
            var existingTimesheet = CreateExistingTimesheet();
            existingTimesheet.PayElements = new List<PayElement>()
            {
                existingPayElement
            };

            var updatedPayElement = _fixture.Build<PayElementUpdate>()
                .With(pe => pe.Id, existingPayElement.Id)
                .Create();
            var request = CreateRequest(updatedPayElement);

            // Act
            await _classUnderTest.Execute(request, existingTimesheet.OperativeId, existingTimesheet.WeekId);

            // Assert
            var payElement = existingTimesheet.PayElements.Single();
            payElement.Should().BeEquivalentTo(updatedPayElement.ToDb(), options => options
                .Excluding(pe => pe.Id)
                .Excluding(pe => pe.Timesheet)
                .Excluding(pe => pe.TimesheetId)
                .Excluding(pe => pe.PayElementType)
                .Excluding(pe => pe.ReadOnly)
            );
            InMemoryDb.DbSaver.VerifySaveCalled();
        }

        [Test]
        public async Task RemovesExistingPayElements()
        {
            // Arrange
            var existingPayElement = _fixture.Create<PayElementUpdate>();
            var existingTimesheet = CreateExistingTimesheet();
            existingTimesheet.PayElements = new List<PayElement>()
            {
                existingPayElement.ToDb()
            };

            var updatedPayElement = _fixture.Create<PayElementUpdate>();
            var request = CreateRequest(updatedPayElement);

            // Act
            await _classUnderTest.Execute(request, existingTimesheet.OperativeId, existingTimesheet.WeekId);

            // Assert
            var payElement = existingTimesheet.PayElements.Single();
            payElement.Should().BeEquivalentTo(updatedPayElement.ToDb());
            InMemoryDb.DbSaver.VerifySaveCalled();
        }

        [Test]
        public async Task DoesNotRemovePayElementsIfReadOnly()
        {
            // Arrange
            var existingPayElement = _fixture.Create<PayElement>();
            existingPayElement.ReadOnly = true;
            var existingTimesheet = CreateExistingTimesheet();
            existingTimesheet.PayElements = new List<PayElement>()
            {
                existingPayElement
            };

            var request = new TimesheetUpdateRequest();

            // Act
            await _classUnderTest.Execute(request, existingTimesheet.OperativeId, existingTimesheet.WeekId);

            // Assert
            var payElement = existingTimesheet.PayElements.Single();
            payElement.Should().BeEquivalentTo(existingPayElement);
            InMemoryDb.DbSaver.VerifySaveCalled();
        }

        [Test]
        public async Task ReadOnlyIsFalseForNewPayElements()
        {
            // Arrange
            var existingTimesheet = CreateExistingTimesheet();

            var updatedPayElement = _fixture.Create<PayElementUpdate>();
            var request = CreateRequest(updatedPayElement);

            // Act
            await _classUnderTest.Execute(request, existingTimesheet.OperativeId, existingTimesheet.WeekId);

            // Assert
            var payElement = existingTimesheet.PayElements.Single();
            payElement.ReadOnly.Should().BeFalse();
            InMemoryDb.DbSaver.VerifySaveCalled();
        }

        [Test]
        public async Task ThrowsResourceNotFoundWhenTimesheetDoesntExist()
        {
            // Arrange
            _timesheetGatewayMock.Setup(x => x.GetOperativesTimesheetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(null as Timesheet);

            // Act
            Func<Task> act = async () => await _classUnderTest.Execute(new TimesheetUpdateRequest(), "1", "1");

            // Assert
            await act.Should().ThrowAsync<ResourceNotFoundException>();
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        private TimesheetUpdateRequest CreateRequest(PayElementUpdate newPayElement)
        {

            var request = _fixture.Build<TimesheetUpdateRequest>()
                .With(x => x.PayElements, new List<PayElementUpdate>()
                {
                    newPayElement
                })
                .Create();
            return request;
        }

        private Timesheet CreateExistingTimesheet()
        {
            var existingTimesheet = _fixture.Build<Timesheet>()
                .Without(t => t.PayElements)
                .Create();
            _timesheetGatewayMock.Setup(x => x.GetOperativesTimesheetAsync(existingTimesheet.WeekId, existingTimesheet.OperativeId))
                .ReturnsAsync(existingTimesheet);
            return existingTimesheet;
        }
    }
}
