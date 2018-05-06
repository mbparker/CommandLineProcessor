namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public interface IInputCommand : ICommand
    {
        IDictionary<string, object> Inputs { get; }

        ICommand NextCommand { get; }

        string Prompt { get; }

        void ApplyInput(string inputText);
    }
}