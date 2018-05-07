namespace CommandLineProcessorLib
{
    using System;
    using System.Linq;

    using CommandLineProcessorCommon;

    using CommandLineProcessorContracts;

    public class InputHandlerProvider : IInputHandlerService
    {
        public Func<ICommand> GetActiveCommandFunc { get; set; }

        public int MinimumSelectionStart => GetPrompt().Length;

        public string PromptRoot { get; set; }

        public bool AllowKeyPress(int key, int selectionStartIndex)
        {
            int minSelStart = MinimumSelectionStart;
            return (selectionStartIndex > minSelStart)
                   || (selectionStartIndex == minSelStart && key != Constants.Keys.Back && key != Constants.Keys.Left);
        }

        public string GetPrompt()
        {
            var result = PromptRoot;
            result = AppendCommandOptions(result);
            return $"{result}: ";
        }

        private string AppendCommandOptions(string prompt)
        {
            var activeCommand = GetActiveCommandFunc();
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
                result = $"{result}: {inputCommand.Name} ({inputCommand.Prompt})";
            }

            return result;
        }
    }
}