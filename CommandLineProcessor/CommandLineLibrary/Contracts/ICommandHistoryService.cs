namespace CommandLineLibrary.Contracts
{
    using CommandLineLibrary.Contracts.Commands;

    using CommandLineLibrary.Models;

    public interface ICommandHistoryService
    {
        CommandHistorySettings Settings { get; set; }

        ICommand First();

        ICommand Last();

        ICommand Next();

        void NotifyCommandExecuting(ICommand command);

        ICommand Previous();
    }
}