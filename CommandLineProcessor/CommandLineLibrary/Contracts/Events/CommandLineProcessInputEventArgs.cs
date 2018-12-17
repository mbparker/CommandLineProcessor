namespace CommandLineLibrary.Contracts.Events
{
    using System;

    public class CommandLineProcessInputEventArgs : EventArgs
    {
        public CommandLineProcessInputEventArgs(string inputText)
        {
            InputText = inputText;
        }

        public string InputText { get; }
    }
}