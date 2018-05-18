namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public interface IInputCommand : ICommand
    {
        ICommand NextCommand { get; set; }

        string Prompt { get; }

        void ApplyInput(ICommandContext context, string inputText);
    }
}