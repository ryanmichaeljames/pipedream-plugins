using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using PipeDream.Plugins.Interfaces;

namespace PipeDream.Plugins
{
    /// <summary>
    /// Base class for all plug-in classes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Plugin development guide: https://docs.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://docs.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </para>
    /// <para>
    /// For dependency injection support, use the PipeDream.Plugins.Ioc package and inherit from IocPluginBase instead.
    /// </para>
    /// </remarks>
    public abstract class PluginBase : IPlugin
    {
        /// <summary>
        /// Gets the class name of the plugin for logging purposes.
        /// </summary>
        protected string PluginClassName { get; }

        /// <summary>
        /// Gets the unsecure configuration string passed to the plugin constructor.
        /// </summary>
        protected string UnsecureConfig { get; }

        /// <summary>
        /// Gets the secure configuration string passed to the plugin constructor.
        /// </summary>
        protected string SecureConfig { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginBase"/> class.
        /// </summary>
        /// <param name="pluginClassName">The <see cref="Type"/> of the plugin class.</param>
        protected PluginBase(Type pluginClassName) : this(pluginClassName, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginBase"/> class with configuration.
        /// </summary>
        /// <param name="pluginClassName">The <see cref="Type"/> of the plugin class.</param>
        /// <param name="unsecureConfig">Unsecure configuration string from plugin registration.</param>
        /// <param name="secureConfig">Secure configuration string from plugin registration.</param>
        protected PluginBase(Type pluginClassName, string unsecureConfig, string secureConfig)
        {
            PluginClassName = pluginClassName?.ToString() ?? GetType().ToString();
            UnsecureConfig = unsecureConfig;
            SecureConfig = secureConfig;
        }

        /// <summary>
        /// Main entry point for the business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="serviceProvider">The service provider from Dataverse.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Execute")]
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new InvalidPluginExecutionException(nameof(serviceProvider));
            }

            // Construct the local plug-in context
            var localPluginContext = CreateLocalPluginContext(serviceProvider);

            localPluginContext.Trace($"Entered {PluginClassName}.Execute() " +
                $"Correlation Id: {localPluginContext.PluginExecutionContext.CorrelationId}, " +
                $"Initiating User: {localPluginContext.PluginExecutionContext.InitiatingUserId}");

            try
            {
                // Invoke the custom implementation
                ExecuteDataversePlugin(localPluginContext);
            }
            catch (FaultException<OrganizationServiceFault> orgServiceFault)
            {
                localPluginContext.Trace($"Exception: {orgServiceFault}");

                throw new InvalidPluginExecutionException($"OrganizationServiceFault: {orgServiceFault.Message}", orgServiceFault);
            }
            finally
            {
                localPluginContext.Trace($"Exiting {PluginClassName}.Execute()");
            }
        }

        /// <summary>
        /// Creates the local plugin context. Override this method to provide custom logging implementation.
        /// </summary>
        /// <param name="serviceProvider">The service provider from Dataverse.</param>
        /// <returns>A new instance of <see cref="ILocalPluginContext"/>.</returns>
        protected virtual ILocalPluginContext CreateLocalPluginContext(IServiceProvider serviceProvider)
        {
            return new LocalPluginContext(serviceProvider);
        }

        /// <summary>
        /// Override this method to implement custom plug-in business logic.
        /// </summary>
        /// <param name="localPluginContext">Context for the current plug-in.</param>
        protected abstract void ExecuteDataversePlugin(ILocalPluginContext localPluginContext);
    }
}
