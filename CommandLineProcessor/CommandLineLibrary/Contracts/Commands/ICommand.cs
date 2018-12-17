namespace CommandLineLibrary.Contracts.Commands
{
    public interface ICommand : ICommandDescriptor
    {
        ICommand Parent { get; set; }

        string Path { get; }

        bool CommandIs<T>()
            where T : class;
    }
}