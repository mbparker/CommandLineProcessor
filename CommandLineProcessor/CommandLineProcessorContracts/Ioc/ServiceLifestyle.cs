namespace CommandLineProcessorContracts.Ioc
{
    public enum ServiceLifestyle
    {
        Transient,

        PerThread,

        PerWebRequest,

        Singleton
    }
}