namespace CommandLineLibrary.Contracts
{
    using System.Collections.Generic;

    using CommandLineLibrary.Contracts.Commands;

    public interface ICommandRepositoryService : IEnumerable<ICommand>
    {
        int Count { get; }

        ICommand this[string selector] { get; }

        ICommand this[int index] { get; }

        void Clear();

        void Load(IEnumerable<ICommand> commands);
    }
}