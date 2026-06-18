namespace SmartGrid.WebApi.Hubs
{
    public static class DeviceHubGroups
    {
        public static string Property(string propertyId) => $"property:{propertyId}";
    }
}
