using Microsoft.Xrm.Sdk;
using PipeDream.Plugins.Constants;

namespace PipeDream.Plugins.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IPluginExecutionContext"/>.
    /// </summary>
    public static class PluginExecutionContextExtensions
    {
        #region Stage Checks

        /// <summary>
        /// Determines if the current stage is PreValidation (10).
        /// </summary>
        public static bool IsPreValidation(this IPluginExecutionContext context)
            => context.Stage == Stage.PreValidation;

        /// <summary>
        /// Determines if the current stage is PreOperation (20).
        /// </summary>
        public static bool IsPreOperation(this IPluginExecutionContext context)
            => context.Stage == Stage.PreOperation;

        /// <summary>
        /// Determines if the current stage is MainOperation (30).
        /// </summary>
        public static bool IsMainOperation(this IPluginExecutionContext context)
            => context.Stage == Stage.MainOperation;

        /// <summary>
        /// Determines if the current stage is PostOperation (40).
        /// </summary>
        public static bool IsPostOperation(this IPluginExecutionContext context)
            => context.Stage == Stage.PostOperation;

        #endregion

        #region Message Checks

        /// <summary>
        /// Determines if the current message is Create.
        /// </summary>
        public static bool IsCreate(this IPluginExecutionContext context)
            => context.MessageName == Message.Create;

        /// <summary>
        /// Determines if the current message is Update.
        /// </summary>
        public static bool IsUpdate(this IPluginExecutionContext context)
            => context.MessageName == Message.Update;

        /// <summary>
        /// Determines if the current message is Delete.
        /// </summary>
        public static bool IsDelete(this IPluginExecutionContext context)
            => context.MessageName == Message.Delete;

        /// <summary>
        /// Determines if the current message is Retrieve.
        /// </summary>
        public static bool IsRetrieve(this IPluginExecutionContext context)
            => context.MessageName == Message.Retrieve;

        /// <summary>
        /// Determines if the current message is RetrieveMultiple.
        /// </summary>
        public static bool IsRetrieveMultiple(this IPluginExecutionContext context)
            => context.MessageName == Message.RetrieveMultiple;

        /// <summary>
        /// Determines if the current message is Associate.
        /// </summary>
        public static bool IsAssociate(this IPluginExecutionContext context)
            => context.MessageName == Message.Associate;

        /// <summary>
        /// Determines if the current message is Disassociate.
        /// </summary>
        public static bool IsDisassociate(this IPluginExecutionContext context)
            => context.MessageName == Message.Disassociate;

        /// <summary>
        /// Determines if the current message is SetState.
        /// </summary>
        public static bool IsSetState(this IPluginExecutionContext context)
            => context.MessageName == Message.SetState;

        /// <summary>
        /// Determines if the current message is SetStateDynamicEntity.
        /// </summary>
        public static bool IsSetStateDynamicEntity(this IPluginExecutionContext context)
            => context.MessageName == Message.SetStateDynamicEntity;

        /// <summary>
        /// Determines if the current message matches the specified message name.
        /// </summary>
        public static bool IsMessage(this IPluginExecutionContext context, string messageName)
            => context.MessageName == messageName;

        #endregion

        #region Mode Checks

        /// <summary>
        /// Determines if the plugin is executing synchronously (Mode = 0).
        /// </summary>
        public static bool IsSynchronous(this IPluginExecutionContext context)
            => context.Mode == Mode.Synchronous;

        /// <summary>
        /// Determines if the plugin is executing asynchronously (Mode = 1).
        /// </summary>
        public static bool IsAsynchronous(this IPluginExecutionContext context)
            => context.Mode == Mode.Asynchronous;

        #endregion

        #region Depth Checks

        /// <summary>
        /// Determines if the plugin depth exceeds the specified maximum depth.
        /// Use to prevent infinite loops caused by plugin recursion.
        /// </summary>
        /// <param name="context">The plugin execution context.</param>
        /// <param name="maxDepth">The maximum allowed depth (default is 1).</param>
        public static bool ExceedsDepth(this IPluginExecutionContext context, int maxDepth = Depth.Default)
            => context.Depth > maxDepth;

        /// <summary>
        /// Determines if this is the initial invocation (Depth = 1).
        /// </summary>
        public static bool IsInitialInvocation(this IPluginExecutionContext context)
            => context.Depth == Depth.Initial;

        #endregion

        #region Shared Variables

        /// <summary>
        /// Gets a shared variable from the context, returning the specified default if not found or wrong type.
        /// </summary>
        public static T GetSharedVariable<T>(this IPluginExecutionContext context, string key, T defaultValue = default)
        {
            if (context.SharedVariables.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// Sets a shared variable in the context.
        /// </summary>
        public static void SetSharedVariable<T>(this IPluginExecutionContext context, string key, T value)
        {
            context.SharedVariables[key] = value;
        }

        /// <summary>
        /// Determines if a shared variable exists in the context.
        /// </summary>
        public static bool HasSharedVariable(this IPluginExecutionContext context, string key)
            => context.SharedVariables.ContainsKey(key);

        /// <summary>
        /// Removes a shared variable from the context.
        /// </summary>
        public static bool RemoveSharedVariable(this IPluginExecutionContext context, string key)
            => context.SharedVariables.Remove(key);

        #endregion

        #region Parent Context

        /// <summary>
        /// Gets the root parent context (traverses up the parent chain).
        /// </summary>
        public static IPluginExecutionContext GetRootContext(this IPluginExecutionContext context)
        {
            var current = context;
            while (current.ParentContext != null)
            {
                current = current.ParentContext;
            }
            return current;
        }

        /// <summary>
        /// Determines if the context has a parent context.
        /// </summary>
        public static bool HasParentContext(this IPluginExecutionContext context)
            => context.ParentContext != null;

        #endregion
    }
}
