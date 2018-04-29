namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public interface ICommandRepositoryService
    {
        int Count { get; }

        ICommand this[string selector] { get; }

        ICommand this[int index] { get; }

        void Load(IEnumerable<ICommand> commands);
    }
}