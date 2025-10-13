namespace Alicloud.Apm.CrashAnalysis
{
    internal interface IPlatformCrashAnalysis
    {
        void Log(string message);

        void RecordExceptionModel(InternalExceptionModel exceptionModel);
    }
}
