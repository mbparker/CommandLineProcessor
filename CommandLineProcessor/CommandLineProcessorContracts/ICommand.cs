namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public interface ICommand
    {
        IEnumerable<string> AliasSelectors { get; }

        string HelpText { get; }

        string Name { get; }

        ICommand Parent { get; set; }

        string Path { get; }

        string PrimarySelector { get; }
    }
}