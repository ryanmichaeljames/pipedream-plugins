using System;
using Microsoft.Xrm.Sdk;

namespace PipeDream.Plugins
{
    /// <summary>
    /// Specialized ITracingService implementation that prefixes all traced messages with a time delta
    /// for Plugin performance diagnostics.
    /// </summary>
    public class LocalTracingService : ITracingService
    {
        private readonly ITracingService _tracingService;
        private DateTime _previousTraceTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalTracingService"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider from the plugin execution.</param>
        public LocalTracingService(IServiceProvider serviceProvider)
        {
            DateTime utcNow = DateTime.UtcNow;

            var context = (IExecutionContext)serviceProvider.GetService(typeof(IExecutionContext));

            DateTime initialTimestamp = context.OperationCreatedOn;

            if (initialTimestamp > utcNow)
            {
                initialTimestamp = utcNow;
            }

            _tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            _previousTraceTime = initialTimestamp;
        }

        /// <summary>
        /// Traces a message with a time delta prefix showing milliseconds since last trace.
        /// </summary>
        /// <param name="message">The message format string.</param>
        /// <param name="args">Optional format arguments.</param>
        public void Trace(string message, params object[] args)
        {
            var utcNow = DateTime.UtcNow;

            // The duration since the last trace
            var deltaMilliseconds = utcNow.Subtract(_previousTraceTime).TotalMilliseconds;

            try
            {
                if (args == null || args.Length == 0)
                {
                    _tracingService.Trace($"[+{deltaMilliseconds:N0}ms] - {message}");
                }
                else
                {
                    _tracingService.Trace($"[+{deltaMilliseconds:N0}ms] - {string.Format(message, args)}");
                }
            }
            catch (FormatException ex)
            {
                throw new InvalidPluginExecutionException($"Failed to write trace message due to error {ex.Message}", ex);
            }

            _previousTraceTime = utcNow;
        }
    }
}
