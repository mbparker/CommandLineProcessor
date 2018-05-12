namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public abstract class BaseCommand : ICommand
    {
        public virtual IEnumerable<string> AliasSelectors { get; } = new string[0];

        public virtual string HelpText { get; } = string.Empty;

        public virtual string Name { get; } = string.Empty;

        public virtual ICommand Parent { get; set; }

        public string Path
        {
            get
            {
                var path = $"{Parent?.Path ?? string.Empty}|{Parent?.PrimarySelector ?? string.Empty}";
                if (path == "|")
                {
                    path = string.Empty;
                }

                if (path.StartsWith("|"))
                {
                    path = path.Remove(0, 1);
                }

                return path;
            }
        }

        public abstract string PrimarySelector { get; }

        public bool CommandIs<T>()
            where T : class
        {
            return this as T != null;
        }
    }
}