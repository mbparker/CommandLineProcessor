namespace CommandLineProcessorContracts
{
    using System;

    public interface IInputHandlerService
    {
        Func<ICommand> GetActiveCommandFunc { get; set; }

        int MinimumSelectionStart { get; }

        string PromptRoot { get; set; }

        bool AllowKeyPress(int key, int selectionStartIndex);

        string GetPrompt();
    }
}