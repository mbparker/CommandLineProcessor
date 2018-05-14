namespace CommandLineProcessorContracts
{
    using System;

    public interface ICommandHistoryService
    {
        event EventHandler<CommandExecutedEventArgs> CommandExecuting;

        ICommand First();

        ICommand Last();

        ICommand Next();

        void NotifyCommandExecuting(ICommand command);

        ICommand Previous();
    }
}