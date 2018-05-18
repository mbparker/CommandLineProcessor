namespace CommandLineProcessorCommon.Ioc
{
    public enum ServiceLifestyle
    {
        Transient,

        PerThread,

        PerWebRequest,

        Singleton
    }
}