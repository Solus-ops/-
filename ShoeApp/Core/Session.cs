using ShoeApp;

namespace ShoeApp.Core
{
    public static class Session
    {
        public static Users CurrentUser { get; set; }

        public static bool IsAuthenticated => CurrentUser != null;

        public static bool IsInRole(string roleName)
        {
            return CurrentUser != null
                   && CurrentUser.Roles != null
                   && CurrentUser.Roles.Name == roleName;
        }
    }
}
