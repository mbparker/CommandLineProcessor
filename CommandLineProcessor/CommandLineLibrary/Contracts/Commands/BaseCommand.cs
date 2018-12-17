namespace CommandLineLibrary.Contracts.Commands
{
    using System.Collections.Generic;

    public class BaseCommand : ICommand
    {
        private IEnumerable<string> aliasSelectors;

        public BaseCommand(string primarySelector, IEnumerable<string> aliasSelectors, string name, string helpText)
        {
            PrimarySelector = primarySelector;
            AliasSelectors = aliasSelectors;
            Name = name;
            HelpText = helpText;
        }

        public IEnumerable<string> AliasSelectors
        {
            get => aliasSelectors;
            protected set
            {
                if (value == null)
                {
                    value = new string[0];
                }

                aliasSelectors = value;
            }
        }

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