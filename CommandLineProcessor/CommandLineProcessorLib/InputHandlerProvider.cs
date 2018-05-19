namespace CommandLineProcessorLib
{
    using System.Linq;
    using System.Text;

    using CommandLineProcessorCommon;

    using CommandLineProcessorContracts;

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
            var levelIndicator = GetCommandLevelIndicator();
            return $"{levelIndicator}{result}: ";
        }

        private static string GetCommandOptionsForInput(IInputCommand inputCommand, string promptText)
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

            return $"{promptText}: {commandWithName?.Name ?? string.Empty} ({inputCommand.Prompt})";
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
    }
}