namespace PensionMgmt.Session
{
    public enum UserRole { SystemAdmin, PensionAdmin, PensionManager, PensionHolder }

    /// <summary>Stores the currently logged-in user for the lifetime of the session.</summary>
    public static class CurrentUser
    {
        public static string   Username { get; set; }
        public static string   FullName { get; set; }
        public static UserRole Role     { get; set; }

        public static void Clear()
        {
            Username = null;
            FullName = null;
        }
    }
}
