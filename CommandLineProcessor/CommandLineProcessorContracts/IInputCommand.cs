namespace CommandLineProcessorContracts
{
    public interface IInputCommand : ICommand
    {
        ICommand NextCommand { get; set; }

        string Prompt { get; }

        void ApplyInput(ICommandContext context, string inputText);

        string GetDefaultValue(ICommandContext context);
    }
}