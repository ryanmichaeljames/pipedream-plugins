using System;
using System.ServiceModel;
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

        [Fact]
        public void Execute_WhenOrganizationServiceFaultThrown_WrapsInInvalidPluginExecutionException()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var plugin = new TestPluginThatThrowsOrgServiceFault();

            // Act & Assert
            var exception = Assert.Throws<InvalidPluginExecutionException>(() => plugin.Execute(serviceProvider));
            Assert.Contains("OrganizationServiceFault", exception.Message);
        }

        [Fact]
        public void Execute_WhenExceptionThrown_StillTracesExit()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var plugin = new TestPluginThatThrowsOrgServiceFault();

            // Act
            try
            {
                plugin.Execute(serviceProvider);
            }
            catch
            {
                // Expected exception
            }

            // Assert - should still trace exit
            serviceProvider.TracingServiceMock.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("Exiting")), It.IsAny<object[]>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public void Execute_WithCustomLocalPluginContext_UsesCustomContext()
        {
            // Arrange
            var serviceProvider = new FakeServiceProvider();
            var plugin = new TestPluginWithCustomContext();

            // Act
            plugin.Execute(serviceProvider);

            // Assert
            Assert.True(plugin.UsedCustomContext);
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

            protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
            {
                // No-op for config testing
            }

            public string GetUnsecureConfig() => UnsecureConfig;
            public string GetSecureConfig() => SecureConfig;
        }

        /// <summary>
        /// Test plugin that throws OrganizationServiceFault.
        /// </summary>
        private class TestPluginThatThrowsOrgServiceFault : PluginBase
        {
            public TestPluginThatThrowsOrgServiceFault() : base(typeof(TestPluginThatThrowsOrgServiceFault))
            {
            }

            protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
            {
                var fault = new OrganizationServiceFault { Message = "Test fault" };
                throw new FaultException<OrganizationServiceFault>(fault, "Test fault message");
            }
        }

        /// <summary>
        /// Test plugin with custom local plugin context.
        /// </summary>
        private class TestPluginWithCustomContext : PluginBase
        {
            public bool UsedCustomContext { get; private set; }

            public TestPluginWithCustomContext() : base(typeof(TestPluginWithCustomContext))
            {
            }

            protected override ILocalPluginContext CreateLocalPluginContext(IServiceProvider serviceProvider)
            {
                return new CustomLocalPluginContext(serviceProvider, this);
            }

            protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
            {
                if (localPluginContext is CustomLocalPluginContext)
                {
                    UsedCustomContext = true;
                }
            }

            private class CustomLocalPluginContext : LocalPluginContext
            {
                private readonly TestPluginWithCustomContext _plugin;

                public CustomLocalPluginContext(IServiceProvider serviceProvider, TestPluginWithCustomContext plugin)
                    : base(serviceProvider)
                {
                    _plugin = plugin;
                }
            }
        }
    }
}
