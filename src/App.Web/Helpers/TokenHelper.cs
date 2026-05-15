namespace App.Web.Helpers
{
    public static class TokenHelper
    {
        private const string TokenKey = "JwtToken";
        private const string RoleKey = "UserRole";
        private const string UserIdKey = "UserId";
        private const string UserNameKey = "UserName";

        public static string? GetToken(IHttpContextAccessor ctx) =>
            ctx.HttpContext?.Session.GetString(TokenKey);

        public static void SetToken(IHttpContextAccessor ctx, string token) =>
            ctx.HttpContext?.Session.SetString(TokenKey, token);

        public static void ClearSession(IHttpContextAccessor ctx) =>
            ctx.HttpContext?.Session.Clear();

        public static void SetRole(IHttpContextAccessor ctx, string role) =>
            ctx.HttpContext?.Session.SetString(RoleKey, role);

        public static string? GetRole(IHttpContextAccessor ctx) =>
            ctx.HttpContext?.Session.GetString(RoleKey);

        public static bool IsAdmin(IHttpContextAccessor ctx) =>
            GetRole(ctx) == "Admin";

        public static void SetUserName(IHttpContextAccessor ctx, string name) =>
            ctx.HttpContext?.Session.SetString(UserNameKey, name);

        public static string? GetUserName(IHttpContextAccessor ctx) =>
            ctx.HttpContext?.Session.GetString(UserNameKey);
    }
}
