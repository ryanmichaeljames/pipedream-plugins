namespace PipeDream.Plugins.Constants
{
    /// <summary>
    /// Pipeline stage values.
    /// </summary>
    public static class Stage
    {
        /// <summary>
        /// Pre-validation stage (10) - Before main system operation validation.
        /// </summary>
        public const int PreValidation = 10;

        /// <summary>
        /// Pre-operation stage (20) - Before main system operation, inside transaction.
        /// </summary>
        public const int PreOperation = 20;

        /// <summary>
        /// Main operation stage (30) - Reserved for custom API main operation.
        /// </summary>
        public const int MainOperation = 30;

        /// <summary>
        /// Post-operation stage (40) - After main system operation, inside transaction.
        /// </summary>
        public const int PostOperation = 40;
    }
}
