using System;
using Microsoft.Xrm.Sdk;
using Moq;
using PipeDream.Plugins.Interfaces;
using PipeDream.Plugins.Tests.Fakes;
using Xunit;

namespace PipeDream.Plugins.Tests
{
    public class PluginBaseTests
    {
        [Fact]
        public void Execute_WithNullServiceProvider_ThrowsInvalidPluginExecutionException()
        {
            // Arrange
            var plugin = new TestPlugin();

            // Act & Assert
            Assert.Throws<InvalidPluginExecutionException>(() => plugin.Execute(null));
        }

        [Fact]
        public void Execute_WithValidServiceProvider_CallsExecuteDataversePlugin()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var plugin = new TestPlugin();

            // Act
            plugin.Execute(serviceProvider);

            // Assert
            Assert.True(plugin.ExecuteDataversePluginCalled);
        }

        [Fact]
        public void Execute_TracesEntryAndExit()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var plugin = new TestPlugin();

            // Act
            plugin.Execute(serviceProvider);

            // Assert
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("Entered")), It.IsAny<object[]>()),
                Times.AtLeastOnce);
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("Exiting")), It.IsAny<object[]>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public void Execute_TracesCorrelationIdAndUser()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var serviceProvider = new FakeServiceProvider();
            serviceProvider.PluginExecutionContextMock.Setup(x => x.CorrelationId).Returns(correlationId);
            serviceProvider.PluginExecutionContextMock.Setup(x => x.InitiatingUserId).Returns(userId);
            var plugin = new TestPlugin();

            // Act
            plugin.Execute(serviceProvider);

            // Assert
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains(correlationId.ToString())), It.IsAny<object[]>()),
                Times.AtLeastOnce);
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains(userId.ToString())), It.IsAny<object[]>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public void Constructor_WithConfiguration_StoresConfigValues()
        {
            // Arrange
            var unsecure = "unsecure-config";
            var secure = "secure-config";

            // Act
            var plugin = new TestPluginWithConfig(unsecure, secure);

            // Assert
            Assert.Equal(unsecure, plugin.GetUnsecureConfig());
            Assert.Equal(secure, plugin.GetSecureConfig());
        }

        [Fact]
        public void Constructor_SetsPluginClassName()
        {
            // Arrange & Act
            var plugin = new TestPlugin();

            // Assert
            Assert.Contains("TestPlugin", plugin.GetPluginClassName());
        }

        /// <summary>
        /// Test plugin implementation for testing PluginBase.
        /// </summary>
        private class TestPlugin : PluginBase
        {
            public bool ExecuteDataversePluginCalled { get; private set; }

            public TestPlugin() : base(typeof(TestPlugin))
            {
            }

            protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
            {
                ExecuteDataversePluginCalled = true;
            }

            public string GetPluginClassName() => PluginClassName;
        }

        /// <summary>
        /// Test plugin with configuration support.
        /// </summary>
        private class TestPluginWithConfig : PluginBase
        {
            public TestPluginWithConfig(string unsecure, string secure) 
                : base(typeof(TestPluginWithConfig), unsecure, secure)
            {
            }

            public string GetUnsecureConfig() => UnsecureConfig;
            public string GetSecureConfig() => SecureConfig;
        }
    }
}
