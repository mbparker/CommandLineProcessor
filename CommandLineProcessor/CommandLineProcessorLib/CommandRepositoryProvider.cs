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

        public ICommand this[string selector]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(selector))
                {
                    throw new ArgumentException("Value is required.", nameof(selector));
                }

                if (commandLookup.TryGetValue(selector.Trim().ToUpper(), out var result))
                {
                    return result;
                }

                throw new CommandNotFoundException("Command not found.", selector);
            }
        }

        public ICommand this[int index]
        {
            get
            {
                try
                {
                    return commandLookup.Values.ElementAt(index);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw new ArgumentOutOfRangeException(
                        $"Specified index is out of bounds.{Environment.NewLine}Index value: {index}",
                        e);
                }
            }
        }

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