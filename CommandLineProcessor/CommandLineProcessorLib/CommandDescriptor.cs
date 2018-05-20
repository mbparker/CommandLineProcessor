namespace CommandLineProcessorLib
{
    using System.Collections.Generic;

    using CommandLineProcessorContracts.Commands;

    public class CommandDescriptor : ICommandDescriptor
    {
        public CommandDescriptor(string primarySelector, string name, string helpText, params string[] aliasSelectors)
        {
            PrimarySelector = primarySelector;
            Name = name;
            HelpText = helpText;
            AliasSelectors = aliasSelectors;
        }

        public IEnumerable<string> AliasSelectors { get; }

        public string HelpText { get; }

        public string Name { get; }

        public string PrimarySelector { get; }
    }
}