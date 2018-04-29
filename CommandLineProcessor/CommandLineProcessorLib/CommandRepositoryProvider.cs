namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineProcessorContracts;

    public class CommandRepositoryProvider : ICommandRepositoryService
    {
        private readonly Dictionary<string, ICommand> commandLookup;

        public CommandRepositoryProvider()
        {
            commandLookup = new Dictionary<string, ICommand>();
        }

        public int Count => commandLookup.Count;

        public ICommand this[string selector] => commandLookup[selector.ToUpper()];

        public ICommand this[int index] => commandLookup.Values.ElementAt(index);

        public void Load(IEnumerable<ICommand> commands)
        {
            if (commands == null)
            {
                throw new ArgumentNullException(nameof(commands));
            }

            var commandList = new List<ICommand>(commands);
            if (!commandList.Any())
            {
                throw new ArgumentException("Collection cannot be empty.", nameof(commands));
            }

            BuildCommandLookup(commandList);
        }

        private void BuildCommandLookup(List<ICommand> commandList)
        {
            commandLookup.Clear();
            BuildCommandLookup(commandLookup, commandList.Where(x => x.Parent == null));
        }

        private void BuildCommandLookup(IDictionary<string, ICommand> lookup, IEnumerable<ICommand> commandList)
        {
            try
            {
                foreach (var command in commandList)
                {
                    foreach (var selector in command.Selectors)
                    {
                        var path = command.Path?.ToUpper();
                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            path += "|";
                        }

                        lookup.Add($"{path}{selector.ToUpper()}", command);
                    }

                    BuildCommandLookup(lookup, command.Children);
                }
            }
            catch (ArgumentException e)
            {
                throw new DuplicateCommandSelectorException("Command Selector values must be unique.", e);
            }
        }
    }
}