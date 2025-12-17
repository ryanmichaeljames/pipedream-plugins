using System;
using Microsoft.Xrm.Sdk;
using Moq;
using PipeDream.Plugins.Tests.Fakes;
using Xunit;

namespace PipeDream.Plugins.Tests
{
    public class LocalTracingServiceTests
    {
        [Fact]
        public void Trace_PrefixesMessageWithTimeDelta()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var tracingService = new LocalTracingService(serviceProvider);

            // Act
            tracingService.Trace("Test message");

            // Assert
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("[+") && s.Contains("ms]") && s.Contains("Test message")), It.IsAny<object[]>()),
                Times.Once);
        }

        [Fact]
        public void Trace_WithFormatArgs_FormatsMessage()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var tracingService = new LocalTracingService(serviceProvider);

            // Act
            tracingService.Trace("Value: {0}, Count: {1}", 42, 100);

            // Assert
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("Value: 42") && s.Contains("Count: 100")), It.IsAny<object[]>()),
                Times.Once);
        }

        [Fact]
        public void Trace_WithEmptyArgs_TracesMessageOnly()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var tracingService = new LocalTracingService(serviceProvider);

            // Act
            tracingService.Trace("Simple message");

            // Assert
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("Simple message")), It.IsAny<object[]>()),
                Times.Once);
        }

        [Fact]
        public void Trace_WithNullArgs_TracesMessageOnly()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var tracingService = new LocalTracingService(serviceProvider);

            // Act
            tracingService.Trace("Message with null args", null);

            // Assert
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("Message with null args")), It.IsAny<object[]>()),
                Times.Once);
        }

        [Fact]
        public void Trace_InvalidFormat_ThrowsInvalidPluginExecutionException()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            serviceProvider.TracingServiceMock
                .Setup(x => x.Trace(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, args) =>
                {
                    // Simulate FormatException being thrown during format
                    if (msg.Contains("{0}") && (args == null || args.Length == 0))
                    {
                        throw new FormatException("Input string was not in correct format");
                    }
                });

            var tracingService = new LocalTracingService(serviceProvider);

            // Act & Assert
            Assert.Throws<InvalidPluginExecutionException>(() => 
                tracingService.Trace("Missing arg {0}"));
        }

        [Fact]
        public void Trace_MultipleCalls_ShowsIncreasingTimeDelta()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var tracingService = new LocalTracingService(serviceProvider);
            var tracedMessages = new System.Collections.Generic.List<string>();

            serviceProvider.TracingServiceMock
                .Setup(x => x.Trace(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, args) => tracedMessages.Add(msg));

            // Act
            tracingService.Trace("First");
            System.Threading.Thread.Sleep(10); // Small delay
            tracingService.Trace("Second");

            // Assert
            Assert.Equal(2, tracedMessages.Count);
            Assert.All(tracedMessages, msg => Assert.Contains("[+", msg));
        }

        [Fact]
        public void Constructor_WithFutureOperationCreatedOn_UsesUtcNow()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            // Set OperationCreatedOn to future time
            serviceProvider.ExecutionContextMock
                .Setup(x => x.OperationCreatedOn)
                .Returns(DateTime.UtcNow.AddHours(1));

            // Act - should not throw
            var tracingService = new LocalTracingService(serviceProvider);
            tracingService.Trace("Test");

            // Assert
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.IsAny<string>(), It.IsAny<object[]>()),
                Times.Once);
        }
    }
}
