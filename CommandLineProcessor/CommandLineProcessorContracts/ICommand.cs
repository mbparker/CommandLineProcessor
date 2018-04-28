namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public interface ICommand
    {
        IEnumerable<ICommand> Children { get; }

        string HelpText { get; }

        string Name { get; }

        ICommand Parent { get; }

        IEnumerable<string> Selectors { get; }

        void Execute(params object[] args);
    }
}