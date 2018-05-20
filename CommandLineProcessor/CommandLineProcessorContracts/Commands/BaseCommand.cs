namespace CommandLineProcessorContracts.Commands
{
    using System.Collections.Generic;

    using CommandLineProcessorCommon;

    public class BaseCommand : ICommand
    {
        public BaseCommand(string primarySelector, IEnumerable<string> aliasSelectors, string name, string helpText)
        {
            PrimarySelector = primarySelector;
            AliasSelectors = aliasSelectors;
            Name = name;
            HelpText = helpText;
        }

        public IEnumerable<string> AliasSelectors { get; protected set; }

        public string HelpText { get; protected set; }

        public string Name { get; protected set; }

        public ICommand Parent { get; set; }

        public string Path
        {
            get
            {
                var path = $"{Parent?.Path ?? string.Empty}|{Parent?.PrimarySelector ?? string.Empty}";
                if (path == Constants.InternalTokens.SelectorSeperator)
                {
                    path = string.Empty;
                }

                if (path.StartsWith(Constants.InternalTokens.SelectorSeperator))
                {
                    path = path.Remove(0, 1);
                }

                return path;
            }
        }

        public string PrimarySelector { get; protected set; }

        public bool CommandIs<T>()
            where T : class
        {
            return this as T != null;
        }
    }
}