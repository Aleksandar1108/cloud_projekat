namespace SmartGrid.WebApi.Authorization
{
    /// <summary>
    /// Role names as serialized into the JWT "role" claim (UserRole enum ToString()).
    /// </summary>
    public static class Roles
    {
        public const string Consumer = "User";
        public const string BillingAdmin = "Admin";
        public const string SysAdmin = "SysAdmin";

        public const string AnyAdmin = BillingAdmin + "," + SysAdmin;
    }
}
