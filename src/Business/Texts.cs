namespace TellagoStudios.Hermes.Business
{
    public static class Texts
    {
        public const string ArgumentWasEmpty = "Argument was empty.";
        public const string ArgumentWasEmptyOrWhitespace = "Argument was empty or whitespace.";
        public const string ArgumentWasInvalid = "Argument was invalid.";
        public const string ArgumentWasNull = "Argument was null.";
        public const string CallbackKindUnknown = "The callback's kind '{0}' is unknown.";
        public const string EntityNotFound = "Could not find an entity of type {0} with id ='{1}'.";
        public const string ErrorPushingCallback = "An error ocurred trying to push callback to suscriber."
                    + "\nMessage: {0}. "
                    + "\nSubscription : {1}";

        public const string GroupCircleReference = "Group Id '{0}' appears twice in a single group hierarchy.";

        public const string GroupContainsChildGroups =
            "The group with id = '{0}' contains sub-groups. Delete all sub-groups before deleting. the group.";

        public const string GroupContainsChildTopics =
            "The group with id = '{0}' contains topics. Delete all topics before deleting the group.";

        public const string GroupMustNotBeNull = "Group property must not be null.";
        public const string GroupNameMustBeUnique = "Already exists a topic group with name '{0}'.";
        public const string IdMustNotBeNull = "Id property must not be null.";
        public const string InvalidFilter = "The specified filter is invalid {0}.";
        public const string MessageIdMustNotBeNull = "MessageId property must not be null.";
        public const string NameMustBeNotNull = "Name must not be null nor empty.";
        public const string ReceivedOnMustBeSetted = "ReceivedOn property must be setted.";
        public const string TargetIdMustNotBeNull = "TargetId property must not be null.";
        public const string TargetKindUnknown = "The target's kind '{0}' is unknown.";
        public const string TopicIdMustNotBeNull = "TopicId property must not be null.";
        public const string TopicNameMustBeUnique = "Already exists a topic with name '{0}' in the same group.";
    }
}
