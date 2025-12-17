using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using Moq;

namespace PipeDream.Plugins.Tests.Fakes
{
    /// <summary>
    /// Fake service provider for testing plugins.
    /// </summary>
    public class FakeServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public Mock<IPluginExecutionContext> PluginExecutionContextMock { get; }
        public Mock<ITracingService> TracingServiceMock { get; }
        public Mock<IOrganizationServiceFactory> OrganizationServiceFactoryMock { get; }
        public Mock<IOrganizationService> OrganizationServiceMock { get; }
        public Mock<IServiceEndpointNotificationService> NotificationServiceMock { get; }
        public Mock<ILogger> LoggerMock { get; }
        public Mock<IExecutionContext> ExecutionContextMock { get; }

        public FakeServiceProvider()
        {
            PluginExecutionContextMock = new Mock<IPluginExecutionContext>();
            TracingServiceMock = new Mock<ITracingService>();
            OrganizationServiceFactoryMock = new Mock<IOrganizationServiceFactory>();
            OrganizationServiceMock = new Mock<IOrganizationService>();
            NotificationServiceMock = new Mock<IServiceEndpointNotificationService>();
            LoggerMock = new Mock<ILogger>();
            ExecutionContextMock = new Mock<IExecutionContext>();

            // Setup default values
            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(new ParameterCollection());
            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(new ParameterCollection());
            PluginExecutionContextMock.Setup(x => x.PreEntityImages).Returns(new EntityImageCollection());
            PluginExecutionContextMock.Setup(x => x.PostEntityImages).Returns(new EntityImageCollection());
            PluginExecutionContextMock.Setup(x => x.SharedVariables).Returns(new ParameterCollection());
            PluginExecutionContextMock.Setup(x => x.CorrelationId).Returns(Guid.NewGuid());
            PluginExecutionContextMock.Setup(x => x.InitiatingUserId).Returns(Guid.NewGuid());
            PluginExecutionContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("Create");
            PluginExecutionContextMock.Setup(x => x.PrimaryEntityName).Returns("account");
            PluginExecutionContextMock.Setup(x => x.PrimaryEntityId).Returns(Guid.NewGuid());

            ExecutionContextMock.Setup(x => x.OperationCreatedOn).Returns(DateTime.UtcNow);

            OrganizationServiceFactoryMock
                .Setup(x => x.CreateOrganizationService(It.IsAny<Guid?>()))
                .Returns(OrganizationServiceMock.Object);

            // Register services
            _services[typeof(IPluginExecutionContext)] = PluginExecutionContextMock.Object;
            _services[typeof(IPluginExecutionContext2)] = new Mock<IPluginExecutionContext2>().Object;
            _services[typeof(IPluginExecutionContext3)] = new Mock<IPluginExecutionContext3>().Object;
            _services[typeof(IPluginExecutionContext4)] = new Mock<IPluginExecutionContext4>().Object;
            _services[typeof(IPluginExecutionContext5)] = new Mock<IPluginExecutionContext5>().Object;
            _services[typeof(IPluginExecutionContext6)] = new Mock<IPluginExecutionContext6>().Object;
            _services[typeof(IPluginExecutionContext7)] = new Mock<IPluginExecutionContext7>().Object;
            _services[typeof(ITracingService)] = TracingServiceMock.Object;
            _services[typeof(IOrganizationServiceFactory)] = OrganizationServiceFactoryMock.Object;
            _services[typeof(IServiceEndpointNotificationService)] = NotificationServiceMock.Object;
            _services[typeof(ILogger)] = LoggerMock.Object;
            _services[typeof(IExecutionContext)] = ExecutionContextMock.Object;
        }

        public object GetService(Type serviceType)
        {
            if (_services.TryGetValue(serviceType, out var service))
            {
                return service;
            }
            return null;
        }

        /// <summary>
        /// Sets a custom service instance for the specified type.
        /// </summary>
        public void SetService<T>(T service)
        {
            _services[typeof(T)] = service;
        }

        /// <summary>
        /// Sets the Target entity in InputParameters.
        /// </summary>
        public void SetTarget(Entity entity)
        {
            var inputParams = new ParameterCollection { { "Target", entity } };
            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(inputParams);
        }

        /// <summary>
        /// Sets the Target entity reference in InputParameters (for Delete operations).
        /// </summary>
        public void SetTargetReference(EntityReference entityRef)
        {
            var inputParams = new ParameterCollection { { "Target", entityRef } };
            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(inputParams);
        }

        /// <summary>
        /// Sets the pre-image entity.
        /// </summary>
        public void SetPreImage(Entity entity, string imageName = "PreImage")
        {
            var images = new EntityImageCollection { { imageName, entity } };
            PluginExecutionContextMock.Setup(x => x.PreEntityImages).Returns(images);
        }

        /// <summary>
        /// Sets the post-image entity.
        /// </summary>
        public void SetPostImage(Entity entity, string imageName = "PostImage")
        {
            var images = new EntityImageCollection { { imageName, entity } };
            PluginExecutionContextMock.Setup(x => x.PostEntityImages).Returns(images);
        }

        /// <summary>
        /// Sets the message name.
        /// </summary>
        public void SetMessageName(string messageName)
        {
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns(messageName);
        }

        /// <summary>
        /// Sets the stage.
        /// </summary>
        public void SetStage(int stage)
        {
            PluginExecutionContextMock.Setup(x => x.Stage).Returns(stage);
        }
    }
}
