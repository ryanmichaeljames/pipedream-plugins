namespace PipeDream.Plugins.Constants
{
    /// <summary>
    /// Message names for common Dataverse operations.
    /// </summary>
    public static class Message
    {
        /// <summary>Create message - fired when a record is created.</summary>
        public const string Create = "Create";

        /// <summary>Update message - fired when a record is updated.</summary>
        public const string Update = "Update";

        /// <summary>Delete message - fired when a record is deleted.</summary>
        public const string Delete = "Delete";

        /// <summary>Retrieve message - fired when a single record is retrieved.</summary>
        public const string Retrieve = "Retrieve";

        /// <summary>RetrieveMultiple message - fired when multiple records are retrieved.</summary>
        public const string RetrieveMultiple = "RetrieveMultiple";

        /// <summary>Associate message - fired when records are associated via N:N relationship.</summary>
        public const string Associate = "Associate";

        /// <summary>Disassociate message - fired when records are disassociated from N:N relationship.</summary>
        public const string Disassociate = "Disassociate";

        /// <summary>SetState message - fired when record state/status is changed.</summary>
        public const string SetState = "SetState";

        /// <summary>SetStateDynamicEntity message - legacy state change message.</summary>
        public const string SetStateDynamicEntity = "SetStateDynamicEntity";

        /// <summary>Assign message - fired when record ownership is changed.</summary>
        public const string Assign = "Assign";

        /// <summary>GrantAccess message - fired when access is granted to a record.</summary>
        public const string GrantAccess = "GrantAccess";

        /// <summary>ModifyAccess message - fired when access to a record is modified.</summary>
        public const string ModifyAccess = "ModifyAccess";

        /// <summary>RevokeAccess message - fired when access to a record is revoked.</summary>
        public const string RevokeAccess = "RevokeAccess";
    }
}
