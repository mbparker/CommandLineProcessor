namespace CommandLineProcessorEntity
{
    public class CommandLineHelpInfo
    {
        public CommandLineHelpInfo(string name, string selector, string helpText)
        {
            Name = name;
            Selector = selector;
            HelpText = helpText;
        }

        public string HelpText { get; }

        public string Name { get; }

        public string Selector { get; }
    }
}