namespace CommandLineProcessorContracts
{
    using System;

    public class CommandLineCommandChangedEventArgs : EventArgs
    {
        public CommandLineCommandChangedEventArgs(ICommand priorCommand, ICommand activeCommand)
        {
            PriorCommand = priorCommand;
            ActiveCommand = activeCommand;
        }

        public ICommand ActiveCommand { get; }

        public ICommand PriorCommand { get; }
    }
}