namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineProcessorContracts;

    public class CommandLineProcessorProvider : ICommandLineProcessorService
    {
        private Dictionary<string, ICommand> commandLookup;

        public string CurrentInput { get; private set; }

        public void ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Value is required.", nameof(input));
            }

            CurrentInput = input;
        }

        public void RegisterCommands(IEnumerable<ICommand> commands)
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

            commandLookup = BuildCommandLookup(commandList);
        }

        private Dictionary<string, ICommand> BuildCommandLookup(List<ICommand> commandList)
        {
            var result = new Dictionary<string, ICommand>();
            try
            {
                foreach (var command in commandList.Where(x => x.Parent == null))
                {
                    foreach (var selector in command.Selectors)
                    {
                        result.Add(selector.ToUpper(), command);
                    }
                }

                return result;
            }
            catch (ArgumentException e)
            {
                throw new DuplicateCommandSelectorException("Command Selector values must be unique.", e);
            }
        }
    }
}