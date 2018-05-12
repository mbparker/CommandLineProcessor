namespace CommandLineProcessorLib
{
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class CommandContext : ICommandContext
    {
        public CommandContext()
        {
            DataStore = new Dictionary<string, object>();
        }

        public IDictionary<string, object> DataStore { get; }
    }
}