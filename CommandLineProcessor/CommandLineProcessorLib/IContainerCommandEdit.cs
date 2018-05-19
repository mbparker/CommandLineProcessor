namespace CommandLineProcessorLib
{
    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands;

    internal interface IContainerCommandEdit
    {
        void AddChild(ICommand command);
    }
}