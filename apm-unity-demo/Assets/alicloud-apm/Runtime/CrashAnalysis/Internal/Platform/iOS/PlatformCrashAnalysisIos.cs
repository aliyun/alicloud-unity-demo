using System.Runtime.InteropServices;

namespace Alicloud.Apm.CrashAnalysis
{
#if UNITY_IOS && !UNITY_EDITOR
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeStackFrame
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string symbol;

        [MarshalAs(UnmanagedType.LPStr)]
        public string file;

        public int line;

        [MarshalAs(UnmanagedType.LPStr)]
        public string library;

        public ulong address;
    }

    internal sealed class PlatformCrashAnalysisIos : IPlatformCrashAnalysis
    {
        public PlatformCrashAnalysisIos()
        {
            try
            {
                eapm_ca_set_internal_key_value(
                    Alicloud.Apm.MetadataBuilder.METADATA_KEY,
                    Alicloud.Apm.MetadataBuilder.GenerateMetadataJSON()
                );
            }
            catch
            {
                CrashAnalysisLogger.Error(
                    "Failed to set Unity metadata during iOS platform initialization"
                );
            }
        }

        [DllImport(
            Alicloud.Apm.IosNativeLibrary.LibraryName,
            CallingConvention = CallingConvention.Cdecl
        )]
        private static extern void eapm_ca_record_exception(
            [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] string reason,
            [MarshalAs(UnmanagedType.LPArray)] NativeStackFrame[] stackFrames,
            int frameCount,
            int language,
            [MarshalAs(UnmanagedType.I1)] bool isCustom,
            [MarshalAs(UnmanagedType.I1)] bool isUrgent,
            [MarshalAs(UnmanagedType.I1)] bool quitApp
        );

        [DllImport(
            Alicloud.Apm.IosNativeLibrary.LibraryName,
            CallingConvention = CallingConvention.Cdecl
        )]
        private static extern void eapm_ca_log([MarshalAs(UnmanagedType.LPStr)] string message);

        [DllImport(
            Alicloud.Apm.IosNativeLibrary.LibraryName,
            CallingConvention = CallingConvention.Cdecl
        )]
        private static extern void eapm_ca_set_internal_key_value(
            [MarshalAs(UnmanagedType.LPStr)] string key,
            [MarshalAs(UnmanagedType.LPStr)] string value
        );

        public void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            try
            {
                eapm_ca_log(message);
            }
            catch
            {
                CrashAnalysisLogger.Error($"Failed to log message to native: {message}");
            }
        }

        public void RecordExceptionModel(InternalExceptionModel exceptionModel)
        {
            if (exceptionModel == null)
            {
                return;
            }

            try
            {
                var nativeFrames = StackFrameParser.ConvertToNativeFrames(
                    exceptionModel.StackTrace
                );
                eapm_ca_record_exception(
                    exceptionModel.Name,
                    exceptionModel.Reason,
                    nativeFrames,
                    nativeFrames.Length,
                    (int)exceptionModel.Language,
                    exceptionModel.Custom,
                    exceptionModel.Urgent,
                    exceptionModel.QuitApp
                );
            }
            catch
            {
                CrashAnalysisLogger.Error(
                    $"Failed to record exception to native: {exceptionModel.Name} - {exceptionModel.Reason}"
                );
            }
        }
    }
#endif
}
