using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using Moq;
using PipeDream.Plugins.Tests.Fakes;
using Xunit;

namespace PipeDream.Plugins.Tests
{
    public class LocalPluginContextTests
    {
        [Fact]
        public void Constructor_WithNullServiceProvider_ThrowsInvalidPluginExecutionException()
        {
            // Act & Assert
            Assert.Throws<InvalidPluginExecutionException>(() => new LocalPluginContext(null));
        }

        [Fact]
        public void Constructor_InitializesAllServices()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.NotNull(context.PluginExecutionContext);
            Assert.NotNull(context.TracingService);
            Assert.NotNull(context.OrgSvcFactory);
            Assert.NotNull(context.InitiatingUserService);
            Assert.NotNull(context.PluginUserService);
            Assert.NotNull(context.ServiceProvider);
        }

        [Fact]
        public void Constructor_InitializesNotificationService()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.NotNull(context.NotificationService);
        }

        [Fact]
        public void Constructor_InitializesLogger()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.NotNull(context.Logger);
        }

        [Fact]
        public void Constructor_WithCustomLogger_UsesCustomLogger()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var customLogger = new Mock<ILogger>().Object;

            // Act
            var context = new LocalPluginContext(serviceProvider, customLogger);

            // Assert
            Assert.Same(customLogger, context.Logger);
        }

        [Fact]
        public void Constructor_InitializesPluginExecutionContext2Through7()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.NotNull(context.PluginExecutionContext2);
            Assert.NotNull(context.PluginExecutionContext3);
            Assert.NotNull(context.PluginExecutionContext4);
            Assert.NotNull(context.PluginExecutionContext5);
            Assert.NotNull(context.PluginExecutionContext6);
            Assert.NotNull(context.PluginExecutionContext7);
        }

        [Fact]
        public void Target_ReturnsEntityFromInputParameters()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var targetEntity = new Entity("account") { Id = Guid.NewGuid() };
            serviceProvider.SetTarget(targetEntity);

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.NotNull(context.Target);
            Assert.Equal(targetEntity.Id, context.Target.Id);
            Assert.Equal("account", context.Target.LogicalName);
        }

        [Fact]
        public void Target_ReturnsNullWhenNotPresent()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.Null(context.Target);
        }

        [Fact]
        public void Target_ReturnsNullWhenTargetIsEntityReference()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var targetRef = new EntityReference("account", Guid.NewGuid());
            serviceProvider.SetTargetReference(targetRef);

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.Null(context.Target);
        }

        [Fact]
        public void TargetReference_ReturnsEntityReferenceFromInputParameters()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var targetRef = new EntityReference("account", Guid.NewGuid());
            serviceProvider.SetTargetReference(targetRef);

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.NotNull(context.TargetReference);
            Assert.Equal(targetRef.Id, context.TargetReference.Id);
            Assert.Equal("account", context.TargetReference.LogicalName);
        }

        [Fact]
        public void TargetReference_ReturnsNullWhenTargetIsEntity()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var targetEntity = new Entity("account") { Id = Guid.NewGuid() };
            serviceProvider.SetTarget(targetEntity);

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.Null(context.TargetReference);
        }

        [Fact]
        public void TargetReference_ReturnsNullWhenNotPresent()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.Null(context.TargetReference);
        }

        [Fact]
        public void PreImage_ReturnsFirstPreImage()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var preImage = new Entity("account") { Id = Guid.NewGuid() };
            preImage["name"] = "Test Account";
            serviceProvider.SetPreImage(preImage);

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.NotNull(context.PreImage);
            Assert.Equal("Test Account", context.PreImage.GetAttributeValue<string>("name"));
        }

        [Fact]
        public void PreImage_ReturnsNullWhenNoImages()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.Null(context.PreImage);
        }

        [Fact]
        public void PostImage_ReturnsFirstPostImage()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var postImage = new Entity("account") { Id = Guid.NewGuid() };
            postImage["name"] = "Updated Account";
            serviceProvider.SetPostImage(postImage);

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.NotNull(context.PostImage);
            Assert.Equal("Updated Account", context.PostImage.GetAttributeValue<string>("name"));
        }

        [Fact]
        public void PostImage_ReturnsNullWhenNoImages()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.Null(context.PostImage);
        }

        [Fact]
        public void MessageName_ReturnsValueFromContext()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            serviceProvider.SetMessageName("Update");

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.Equal("Update", context.MessageName);
        }

        [Fact]
        public void PrimaryEntityName_ReturnsValueFromContext()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            serviceProvider.PluginExecutionContextMock.Setup(x => x.PrimaryEntityName).Returns("contact");

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.Equal("contact", context.PrimaryEntityName);
        }

        [Fact]
        public void PrimaryEntityId_ReturnsValueFromContext()
        {
            // Arrange
            var expectedId = Guid.NewGuid();
            var serviceProvider = new FakeServiceProvider();
            serviceProvider.PluginExecutionContextMock.Setup(x => x.PrimaryEntityId).Returns(expectedId);

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.Equal(expectedId, context.PrimaryEntityId);
        }

        [Fact]
        public void Trace_WritesToTracingService()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var context = new LocalPluginContext(serviceProvider);

            // Act
            context.Trace("Test message");

            // Assert
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("Test message")), It.IsAny<object[]>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public void Trace_WithEmptyMessage_DoesNotThrow()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var context = new LocalPluginContext(serviceProvider);

            // Act & Assert (should not throw)
            context.Trace("");
            context.Trace(null);
        }

        [Fact]
        public void Trace_WithWhitespaceMessage_DoesNotTrace()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var context = new LocalPluginContext(serviceProvider);

            // Act
            context.Trace("   ");

            // Assert - Trace should not be called for whitespace-only message
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.IsAny<string>(), It.IsAny<object[]>()),
                Times.Never);
        }

        [Fact]
        public void LogInfo_WritesToTracingServiceAndLogger()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var context = new LocalPluginContext(serviceProvider);

            // Act
            context.LogInfo("Info message");

            // Assert
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("Info message")), It.IsAny<object[]>()),
                Times.AtLeastOnce);
            serviceProvider.LoggerMock.Verify(
                x => x.LogInformation(It.Is<string>(s => s.Contains("Info message"))),
                Times.Once);
        }

        [Fact]
        public void LogInfo_WithEmptyMessage_DoesNotLog()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var context = new LocalPluginContext(serviceProvider);

            // Act
            context.LogInfo("");
            context.LogInfo(null);

            // Assert
            serviceProvider.LoggerMock.Verify(
                x => x.LogInformation(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public void LogWarning_WritesToTracingServiceAndLogger()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var context = new LocalPluginContext(serviceProvider);

            // Act
            context.LogWarning("Warning message");

            // Assert
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("WARNING") && s.Contains("Warning message")), It.IsAny<object[]>()),
                Times.AtLeastOnce);
            serviceProvider.LoggerMock.Verify(
                x => x.LogWarning(It.Is<string>(s => s.Contains("Warning message"))),
                Times.Once);
        }

        [Fact]
        public void LogWarning_WithEmptyMessage_DoesNotLog()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var context = new LocalPluginContext(serviceProvider);

            // Act
            context.LogWarning("");
            context.LogWarning(null);

            // Assert
            serviceProvider.LoggerMock.Verify(
                x => x.LogWarning(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public void LogError_WritesToTracingServiceAndLogger()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var context = new LocalPluginContext(serviceProvider);

            // Act
            context.LogError("Error message");

            // Assert
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("ERROR") && s.Contains("Error message")), It.IsAny<object[]>()),
                Times.AtLeastOnce);
            serviceProvider.LoggerMock.Verify(
                x => x.LogError(It.Is<string>(s => s.Contains("Error message"))),
                Times.Once);
        }

        [Fact]
        public void LogError_WithEmptyMessage_DoesNotLog()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var context = new LocalPluginContext(serviceProvider);

            // Act
            context.LogError("");
            context.LogError(null);

            // Assert
            serviceProvider.LoggerMock.Verify(
                x => x.LogError(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public void LogError_WithException_IncludesExceptionDetails()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var context = new LocalPluginContext(serviceProvider);
            var exception = new InvalidOperationException("Test exception");

            // Act
            context.LogError(exception, "Error with exception");

            // Assert
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("Error with exception") && s.Contains("InvalidOperationException")), It.IsAny<object[]>()),
                Times.AtLeastOnce);
            serviceProvider.LoggerMock.Verify(
                x => x.LogError(exception, It.Is<string>(s => s.Contains("Error with exception"))),
                Times.Once);
        }

        [Fact]
        public void LogError_WithNullExceptionAndNullMessage_DoesNotLog()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var context = new LocalPluginContext(serviceProvider);

            // Act
            context.LogError(null, null);

            // Assert
            serviceProvider.LoggerMock.Verify(
                x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public void ServiceProvider_ReturnsSameInstancePassedToConstructor()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.Same(serviceProvider, context.ServiceProvider);
        }

        [Fact]
        public void OrgSvcFactory_ReturnsOrganizationServiceFactory()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();

            // Act
            var context = new LocalPluginContext(serviceProvider);

            // Assert
            Assert.NotNull(context.OrgSvcFactory);
            Assert.Same(serviceProvider.OrganizationServiceFactoryMock.Object, context.OrgSvcFactory);
        }
    }
}
