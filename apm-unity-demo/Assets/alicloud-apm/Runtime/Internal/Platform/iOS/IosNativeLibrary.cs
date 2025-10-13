namespace Alicloud.Apm
{
#if UNITY_IOS && !UNITY_EDITOR
    internal static class IosNativeLibrary
    {
        internal const string LibraryName = "__Internal";
    }
#endif
}
