namespace Alicloud.Apm
{
    internal static class PackageUtility
    {
        public static string ReadPackageVersion()
        {
            var version = ApmVersionInfo.Version;
            return string.IsNullOrWhiteSpace(version) ? "unknown" : version;
        }
    }
}
