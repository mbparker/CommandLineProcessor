﻿namespace CommandLineLibrary
{
    using System.Linq;
    using System.Text;

    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Contracts.Commands;

    public class InputHandlerProvider : IInputHandlerService
    {
        public int MinimumSelectionStart => GetPrompt().Length;

        public ICommandLineProcessorService Processor { get; set; }

        public bool AllowKeyPress(int key, int selectionStartIndex)
        {
            int minSelStart = MinimumSelectionStart;
            return (selectionStartIndex > minSelStart)
                   || (selectionStartIndex == minSelStart && key != Constants.Keys.Back && key != Constants.Keys.Left);
        }

        public string GetPrompt()
        {
            var result = Processor.Settings.CommandPromptRoot;
            result = AppendCommandOptions(result);
            result = AppendDefaultValue(result);
            var levelIndicator = GetCommandLevelIndicator();
            return $"{levelIndicator}{result}: ";
        }

        private string AppendCommandOptions(string prompt)
        {
            var activeCommand = Processor.ActiveCommand;
            var result = prompt;

            if (activeCommand is IContainerCommand containerCommand)
            {
                return GetCommandOptionsForContainer(containerCommand, result);
            }

            if (activeCommand is IInputCommand inputCommand)
            {
                result = GetCommandOptionsForInput(inputCommand, result);
            }

            return result;
        }

        private string AppendDefaultValue(string promptText)
        {
            var activeCommand = Processor.ActiveCommand;
            var result = promptText;

            if (activeCommand is IContainerCommand containerCommand)
            {
                return GetDefaultValueForContainer(containerCommand, result);
            }

            if (activeCommand is IInputCommand inputCommand)
            {
                result = GetDefaultValueForInput(inputCommand, result);
            }

            return result;
        }

        private string GetCommandLevelIndicator()
        {
            var builder = new StringBuilder();
            for (int i = 0; i < Processor.StackDepth; i++)
            {
                builder.Append(Processor.Settings.CommandLevelIndicator);
            }

            if (Processor.StackDepth > 0)
            {
                builder.Append(" ");
            }

            return builder.ToString();
        }

        private string GetCommandOptionsForContainer(IContainerCommand containerCommand, string promptText)
        {
            var subCommands = string.Join(",", containerCommand.Children.Select(x => x.PrimarySelector));
            return $"{promptText}: {containerCommand.Name} ({subCommands})";
        }

        private string GetCommandOptionsForInput(IInputCommand inputCommand, string promptText)
        {
            ICommand commandWithName = inputCommand;
            while (commandWithName != null && string.IsNullOrWhiteSpace(commandWithName.Name))
            {
                if (!string.IsNullOrWhiteSpace(commandWithName.Name))
                {
                    break;
                }

                commandWithName = commandWithName.Parent;
            }

            return
                $"{promptText}: {commandWithName?.Name ?? string.Empty} ({inputCommand.GetPromptText(Processor.ActiveContext)})";
        }

        private string GetDefaultValueForContainer(IContainerCommand containerCommand, string promptText)
        {
            var defaultValue = containerCommand.GetDefaultCommandSelector(Processor.ActiveContext);
            if (!string.IsNullOrWhiteSpace(defaultValue))
            {
                return $"{promptText} [{defaultValue}]";
            }

            return promptText;
        }

        private string GetDefaultValueForInput(IInputCommand inputCommand, string promptText)
        {
            var defaultValue = inputCommand.GetDefaultValue(Processor.ActiveContext);
            if (!string.IsNullOrWhiteSpace(defaultValue))
            {
                return $"{promptText} [{defaultValue}]";
            }

            return promptText;
        }
    }
}