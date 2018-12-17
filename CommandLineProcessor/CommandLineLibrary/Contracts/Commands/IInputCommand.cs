namespace CommandLineLibrary.Contracts.Commands
{
    public interface IInputCommand : ICommand
    {
        ICommand NextCommand { get; set; }

        void ApplyInput(ICommandContext context, string inputText);

        string GetDefaultValue(ICommandContext context);

        string GetPromptText(ICommandContext context);
    }
}