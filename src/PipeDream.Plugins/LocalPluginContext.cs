using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using PipeDream.Plugins.Interfaces;

namespace PipeDream.Plugins
{
    /// <summary>
    /// Plug-in context object that provides access to all commonly used services and helper properties.
    /// </summary>
    public class LocalPluginContext : ILocalPluginContext
    {
        #region Services

        /// <inheritdoc/>
        public IOrganizationService InitiatingUserService { get; }

        /// <inheritdoc/>
        public IOrganizationService PluginUserService { get; }

        /// <inheritdoc/>
        public IPluginExecutionContext PluginExecutionContext { get; }

        /// <inheritdoc/>
        public IPluginExecutionContext2 PluginExecutionContext2 { get; }

        /// <inheritdoc/>
        public IPluginExecutionContext3 PluginExecutionContext3 { get; }

        /// <inheritdoc/>
        public IPluginExecutionContext4 PluginExecutionContext4 { get; }

        /// <inheritdoc/>
        public IPluginExecutionContext5 PluginExecutionContext5 { get; }

        /// <inheritdoc/>
        public IPluginExecutionContext6 PluginExecutionContext6 { get; }

        /// <inheritdoc/>
        public IPluginExecutionContext7 PluginExecutionContext7 { get; }

        /// <inheritdoc/>
        public IServiceEndpointNotificationService NotificationService { get; }

        /// <inheritdoc/>
        public ITracingService TracingService { get; }

        /// <inheritdoc/>
        public IServiceProvider ServiceProvider { get; }

        /// <inheritdoc/>
        public IOrganizationServiceFactory OrgSvcFactory { get; }

        /// <inheritdoc/>
        public ILogger Logger { get; }

        #endregion

        #region Helper Properties

        /// <inheritdoc/>
        public Entity Target
        {
            get
            {
                if (PluginExecutionContext.InputParameters.Contains("Target") &&
                    PluginExecutionContext.InputParameters["Target"] is Entity entity)
                {
                    return entity;
                }
                return null;
            }
        }

        /// <inheritdoc/>
        public EntityReference TargetReference
        {
            get
            {
                if (PluginExecutionContext.InputParameters.Contains("Target") &&
                    PluginExecutionContext.InputParameters["Target"] is EntityReference entityRef)
                {
                    return entityRef;
                }
                return null;
            }
        }

        /// <inheritdoc/>
        public Entity PreImage
        {
            get
            {
                if (PluginExecutionContext.PreEntityImages != null &&
                    PluginExecutionContext.PreEntityImages.Count > 0)
                {
                    return PluginExecutionContext.PreEntityImages.Values.FirstOrDefault();
                }
                return null;
            }
        }

        /// <inheritdoc/>
        public Entity PostImage
        {
            get
            {
                if (PluginExecutionContext.PostEntityImages != null &&
                    PluginExecutionContext.PostEntityImages.Count > 0)
                {
                    return PluginExecutionContext.PostEntityImages.Values.FirstOrDefault();
                }
                return null;
            }
        }

        /// <inheritdoc/>
        public string MessageName => PluginExecutionContext.MessageName;

        /// <inheritdoc/>
        public string PrimaryEntityName => PluginExecutionContext.PrimaryEntityName;

        /// <inheritdoc/>
        public Guid PrimaryEntityId => PluginExecutionContext.PrimaryEntityId;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalPluginContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider from the plugin execution.</param>
        /// <param name="customLogger">Optional custom logger implementation to override default ILogger from service provider.</param>
        /// <exception cref="InvalidPluginExecutionException">Thrown when serviceProvider is null.</exception>
        public LocalPluginContext(IServiceProvider serviceProvider, ILogger customLogger = null)
        {
            if (serviceProvider == null)
            {
                throw new InvalidPluginExecutionException(nameof(serviceProvider));
            }

            ServiceProvider = serviceProvider;

            Logger = customLogger ?? serviceProvider.Get<ILogger>();

            // Base context - InputParameters, OutputParameters, Images, Stage, Depth, etc.
            PluginExecutionContext = serviceProvider.Get<IPluginExecutionContext>();

            // Adds: IsPortalsClientCall, PortalsContactId, Azure AD object IDs
            PluginExecutionContext2 = serviceProvider.Get<IPluginExecutionContext2>();

            // Adds: AuthenticatedUserId (for impersonation scenarios)
            PluginExecutionContext3 = serviceProvider.Get<IPluginExecutionContext3>();

            // Adds: PreEntityImagesCollection, PostEntityImagesCollection (for bulk operations)
            PluginExecutionContext4 = serviceProvider.Get<IPluginExecutionContext4>();

            // Adds: InitiatingUserAgent (browser/client detection)
            PluginExecutionContext5 = serviceProvider.Get<IPluginExecutionContext5>();

            // Adds: EnvironmentId, TenantId (multi-tenant scenarios)
            PluginExecutionContext6 = serviceProvider.Get<IPluginExecutionContext6>();

            // Adds: IsApplicationUser (service principal vs human user)
            PluginExecutionContext7 = serviceProvider.Get<IPluginExecutionContext7>();

            TracingService = new LocalTracingService(serviceProvider);

            NotificationService = serviceProvider.Get<IServiceEndpointNotificationService>();

            OrgSvcFactory = serviceProvider.Get<IOrganizationServiceFactory>();

            // User that the plugin is registered to run as (could be same as initiating user)
            PluginUserService = serviceProvider.GetOrganizationService(PluginExecutionContext.UserId);

            // User whose action triggered the plugin
            InitiatingUserService = serviceProvider.GetOrganizationService(PluginExecutionContext.InitiatingUserId);
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public void Trace(string message, [CallerMemberName] string method = null)
        {
            if (string.IsNullOrWhiteSpace(message) || TracingService == null)
            {
                return;
            }

            if (method != null)
            {
                TracingService.Trace($"[{method}] - {message}");
            }
            else
            {
                TracingService.Trace(message);
            }
        }

        /// <inheritdoc/>
        public void LogInfo(string message, [CallerMemberName] string method = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var formattedMessage = method != null ? $"[{method}] - {message}" : message;

            // Plugin Trace Log
            TracingService?.Trace(formattedMessage);

            // Application Insights
            Logger?.LogInformation(formattedMessage);
        }

        /// <inheritdoc/>
        public void LogWarning(string message, [CallerMemberName] string method = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var formattedMessage = method != null ? $"[{method}] - {message}" : message;

            // Plugin Trace Log
            TracingService?.Trace($"WARNING: {formattedMessage}");

            // Application Insights
            Logger?.LogWarning(formattedMessage);
        }

        /// <inheritdoc/>
        public void LogError(string message, [CallerMemberName] string method = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var formattedMessage = method != null ? $"[{method}] - {message}" : message;

            // Plugin Trace Log
            TracingService?.Trace($"ERROR: {formattedMessage}");

            // Application Insights
            Logger?.LogError(formattedMessage);
        }

        /// <inheritdoc/>
        public void LogError(Exception ex, string message, [CallerMemberName] string method = null)
        {
            if (ex == null && string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var formattedMessage = method != null ? $"[{method}] - {message}" : message;

            // Plugin Trace Log - include full exception details
            TracingService?.Trace($"ERROR: {formattedMessage}\n{ex}");

            // Application Insights - structured exception logging
            Logger?.LogError(ex, formattedMessage);
        }

        #endregion
    }
}
