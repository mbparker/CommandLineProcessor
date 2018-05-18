namespace CommandLineProcessorContracts
{
    using CommandLineProcessorCommon;
    using System.Collections.Generic;

    public class BaseCommand : ICommand
    {
        public BaseCommand(ICommandDescriptor descriptor)
        {
            Name = descriptor.Name;
            HelpText = descriptor.HelpText;
            PrimarySelector = descriptor.PrimarySelector;
            AliasSelectors = descriptor.AliasSelectors;
        }

        public BaseCommand()
        {
            AliasSelectors = new string[0];
            HelpText = string.Empty;
            Name = string.Empty;
            PrimarySelector = string.Empty;
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