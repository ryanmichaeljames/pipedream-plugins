namespace PipeDream.Plugins.Constants
{
    /// <summary>
    /// Common input parameter names.
    /// </summary>
    public static class InputParameter
    {
        /// <summary>
        /// Target parameter name for entity operations.
        /// </summary>
        public const string Target = "Target";

        /// <summary>
        /// State parameter name for SetState message.
        /// </summary>
        public const string State = "State";

        /// <summary>
        /// Status parameter name for SetState message.
        /// </summary>
        public const string Status = "Status";

        /// <summary>
        /// EntityMoniker parameter name (Delete, Retrieve).
        /// </summary>
        public const string EntityMoniker = "EntityMoniker";

        /// <summary>
        /// Relationship parameter name for Associate/Disassociate.
        /// </summary>
        public const string Relationship = "Relationship";

        /// <summary>
        /// RelatedEntities parameter name for Associate/Disassociate.
        /// </summary>
        public const string RelatedEntities = "RelatedEntities";

        /// <summary>
        /// Query parameter name for RetrieveMultiple.
        /// </summary>
        public const string Query = "Query";

        /// <summary>
        /// Assignee parameter name for Assign message.
        /// </summary>
        public const string Assignee = "Assignee";
    }
}
