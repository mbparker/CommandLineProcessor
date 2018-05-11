namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public abstract class BaseCommand : ICommand
    {
        public abstract IEnumerable<string> AliasSelectors { get; }

        public abstract string HelpText { get; }

        public abstract string Name { get; }

        public abstract ICommand Parent { get; set; }

        public abstract string Path { get; }

        public abstract string PrimarySelector { get; }

        public bool CommandIs<T>()
            where T : class
        {
            return this as T != null;
        }
    }
}