namespace CommandLineProcessorLib
{
    using System.Collections.Generic;

    using CommandLineProcessorContracts.Commands;

    public class CommandDescriptor : ICommandDescriptor
    {
        public IEnumerable<string> AliasSelectors { get; set; }

        public string HelpText { get; set; }

        public string Name { get; set; }

        public string PrimarySelector { get; set; }
    }
}