namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public interface ICommand
    {
        IEnumerable<ICommand> Children { get; }

        string HelpText { get; }

        string Name { get; }

        ICommand Parent { get; }

        string Path { get; }

        IEnumerable<string> Selectors { get; }
    }
}