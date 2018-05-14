namespace CommandLineProcessorLib
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineProcessorCommon;

    using CommandLineProcessorContracts;

    using CommandLineProcessorEntity.Exceptions;

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

                throw new CommandNotFoundException(
                    $"Command '{selector.Split(new[] { Constants.InternalTokens.SelectorSeperator }, StringSplitOptions.RemoveEmptyEntries).Last()}' not found.",
                    selector);
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

        public void Clear()
        {
            commandLookup.Clear();
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
            string selectorText = string.Empty;
            try
            {
                foreach (var command in commandList)
                {
                    if (!string.IsNullOrWhiteSpace(command.PrimarySelector))
                    {
                        var path = command.Path?.ToUpper();
                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            path += Constants.InternalTokens.SelectorSeperator;
                        }

                        selectorText = $"{path}{command.PrimarySelector.ToUpper()}";
                        lookup.Add(selectorText, command);

                        foreach (var selector in command.AliasSelectors)
                        {
                            selectorText = $"{path}{selector.ToUpper()}";
                            lookup.Add(selectorText, command);
                        }

                        var children = (command as IContainerCommand)?.Children;
                        if (children != null)
                        {
                            BuildCommandLookup(lookup, children);
                        }
                    }
                }
            }
            catch (ArgumentException e)
            {
                throw new DuplicateCommandSelectorException($"Cannot add '{selectorText}'. Command Selector values must be unique.", e);
            }
        }

        IEnumerator<ICommand> IEnumerable<ICommand>.GetEnumerator()
        {
            return commandLookup.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return commandLookup.Values.GetEnumerator();
        }
    }
}