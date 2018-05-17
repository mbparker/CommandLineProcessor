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

        private string AppendCommandOptions(string prompt)
        {
            var activeCommand = Processor.ActiveCommand;
            var result = prompt;

            var containerCommand = activeCommand as IContainerCommand;
            if (containerCommand != null)
            {
                var subCommands = string.Join(",", containerCommand.Children.Select(x => x.PrimarySelector));
                result = $"{result}: {containerCommand.Name} ({subCommands})";
                return result;
            }

            var inputCommand = activeCommand as IInputCommand;
            if (inputCommand != null)
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
                result = $"{result}: {commandWithName?.Name ?? string.Empty} ({inputCommand.Prompt})";
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
    }
}