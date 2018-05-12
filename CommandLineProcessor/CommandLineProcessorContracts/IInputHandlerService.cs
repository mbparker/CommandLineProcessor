namespace CommandLineProcessorContracts
{
    public interface IInputHandlerService
    {
        int MinimumSelectionStart { get; }

        ICommandLineProcessorService Processor { get; set; }

        bool AllowKeyPress(int key, int selectionStartIndex);

        string GetPrompt();
    }
}