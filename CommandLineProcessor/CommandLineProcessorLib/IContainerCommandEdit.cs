namespace CommandLineProcessorLib
{
    using CommandLineProcessorContracts;

    internal interface IContainerCommandEdit
    {
        void AddChild(ICommand command);
    }
}