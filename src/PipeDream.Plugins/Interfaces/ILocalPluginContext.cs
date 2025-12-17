using System;
using System.Runtime.CompilerServices;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;

namespace PipeDream.Plugins.Interfaces
{
    /// <summary>
    /// This interface provides an abstraction on top of IServiceProvider for commonly used 
    /// PowerPlatform Dataverse Plugin development constructs.
    /// </summary>
    public interface ILocalPluginContext
    {
        #region Services

        /// <summary>
        /// The PowerPlatform Dataverse organization service for the user who triggered the action.
        /// </summary>
        IOrganizationService InitiatingUserService { get; }

        /// <summary>
        /// The PowerPlatform Dataverse organization service for the account that was registered 
        /// to run this plugin. This could be the same user as InitiatingUserService.
        /// </summary>
        IOrganizationService PluginUserService { get; }

        /// <summary>
        /// IPluginExecutionContext contains information that describes the run-time environment 
        /// in which the plug-in executes, information related to the execution pipeline, 
        /// and entity business information.
        /// </summary>
        IPluginExecutionContext PluginExecutionContext { get; }

        /// <summary>
        /// IPluginExecutionContext2 extends IPluginExecutionContext with additional properties
        /// for portal/Power Pages scenarios and Azure AD integration, including IsPortalsClientCall,
        /// PortalsContactId, and Azure AD object IDs.
        /// </summary>
        IPluginExecutionContext2 PluginExecutionContext2 { get; }

        /// <summary>
        /// IPluginExecutionContext3 extends IPluginExecutionContext2 with AuthenticatedUserId
        /// for impersonation scenarios.
        /// </summary>
        IPluginExecutionContext3 PluginExecutionContext3 { get; }

        /// <summary>
        /// IPluginExecutionContext4 extends IPluginExecutionContext3 with PreEntityImagesCollection
        /// and PostEntityImagesCollection for bulk/batch operations.
        /// </summary>
        IPluginExecutionContext4 PluginExecutionContext4 { get; }

        /// <summary>
        /// IPluginExecutionContext5 extends IPluginExecutionContext4 with InitiatingUserAgent
        /// for client detection (browser, mobile, API).
        /// </summary>
        IPluginExecutionContext5 PluginExecutionContext5 { get; }

        /// <summary>
        /// IPluginExecutionContext6 extends IPluginExecutionContext5 with EnvironmentId and TenantId
        /// for multi-tenant/multi-environment scenarios.
        /// </summary>
        IPluginExecutionContext6 PluginExecutionContext6 { get; }

        /// <summary>
        /// IPluginExecutionContext7 extends IPluginExecutionContext6 with IsApplicationUser
        /// to differentiate service accounts from human users.
        /// </summary>
        IPluginExecutionContext7 PluginExecutionContext7 { get; }

        /// <summary>
        /// Synchronous registered plug-ins can post the execution context to the Microsoft Azure Service Bus.
        /// It is through this notification service that synchronous plug-ins can send brokered messages 
        /// to the Microsoft Azure Service Bus.
        /// </summary>
        IServiceEndpointNotificationService NotificationService { get; }

        /// <summary>
        /// Provides logging run-time trace information for plug-ins.
        /// </summary>
        ITracingService TracingService { get; }

        /// <summary>
        /// General Service Provider for things not accounted for in the base class.
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// OrganizationService Factory for creating connections for users other than current user and system.
        /// </summary>
        IOrganizationServiceFactory OrgSvcFactory { get; }

        /// <summary>
        /// ILogger for this plugin (Application Insights integration).
        /// </summary>
        ILogger Logger { get; }

        #endregion

        #region Helper Properties

        /// <summary>
        /// Gets the Target entity from InputParameters. Returns null if not present or not an Entity.
        /// </summary>
        Entity Target { get; }

        /// <summary>
        /// Gets the Target entity reference from InputParameters (for Delete operations). 
        /// Returns null if not present or not an EntityReference.
        /// </summary>
        EntityReference TargetReference { get; }

        /// <summary>
        /// Gets the first PreImage entity if registered. Returns null if no pre-images are registered.
        /// </summary>
        Entity PreImage { get; }

        /// <summary>
        /// Gets the first PostImage entity if registered. Returns null if no post-images are registered.
        /// </summary>
        Entity PostImage { get; }

        /// <summary>
        /// Gets the message name (e.g., "Create", "Update", "Delete").
        /// </summary>
        string MessageName { get; }

        /// <summary>
        /// Gets the primary entity logical name (e.g., "account", "contact").
        /// </summary>
        string PrimaryEntityName { get; }

        /// <summary>
        /// Gets the primary entity ID (the GUID of the record being processed).
        /// </summary>
        Guid PrimaryEntityId { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Writes a trace message to the trace log.
        /// </summary>
        /// <param name="message">Message to trace.</param>
        /// <param name="method">Calling method name (automatically populated).</param>
        void Trace(string message, [CallerMemberName] string method = null);

        /// <summary>
        /// Logs an informational message to both Plugin Trace Log and Application Insights.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="method">Calling method name (automatically populated).</param>
        void LogInfo(string message, [CallerMemberName] string method = null);

        /// <summary>
        /// Logs a warning message to both Plugin Trace Log and Application Insights.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="method">Calling method name (automatically populated).</param>
        void LogWarning(string message, [CallerMemberName] string method = null);

        /// <summary>
        /// Logs an error message to both Plugin Trace Log and Application Insights.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="method">Calling method name (automatically populated).</param>
        void LogError(string message, [CallerMemberName] string method = null);

        /// <summary>
        /// Logs an exception with message to both Plugin Trace Log and Application Insights.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">Message to log.</param>
        /// <param name="method">Calling method name (automatically populated).</param>
        void LogError(Exception ex, string message, [CallerMemberName] string method = null);

        #endregion
    }
}
