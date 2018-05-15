namespace CommandLineProcessorContracts
{
    using CommandLineProcessorEntity;

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