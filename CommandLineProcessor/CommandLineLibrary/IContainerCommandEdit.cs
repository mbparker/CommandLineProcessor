namespace CommandLineLibrary
{
    using CommandLineLibrary.Contracts.Commands;

    internal interface IContainerCommandEdit
    {
        void AddChild(ICommand command);
    }
}