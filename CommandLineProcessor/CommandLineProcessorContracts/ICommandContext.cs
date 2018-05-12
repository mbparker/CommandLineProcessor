namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public interface ICommandContext
    {
        IDictionary<string, object> DataStore { get; }
    }
}