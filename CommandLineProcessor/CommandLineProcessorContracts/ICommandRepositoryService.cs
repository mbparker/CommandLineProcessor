namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public interface ICommandRepositoryService : IEnumerable<ICommand>
    {
        int Count { get; }

        ICommand this[string selector] { get; }

        ICommand this[int index] { get; }

        void Clear();

        void Load(IEnumerable<ICommand> commands);
    }
}