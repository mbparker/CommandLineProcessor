namespace CommandLineProcessorContracts.Commands
{
    using System.Collections.Generic;

    public interface ICommandDescriptor
    {
        IEnumerable<string> AliasSelectors { get; }

        string HelpText { get; }

        string Name { get; }

        string PrimarySelector { get; }
    }
}