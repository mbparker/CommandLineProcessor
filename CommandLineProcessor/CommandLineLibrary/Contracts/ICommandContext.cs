namespace CommandLineLibrary.Contracts
{
    using System.Collections.Generic;

    public interface ICommandContext
    {
        ICommandDataStore DataStore { get; }

        T GetService<T>();
    }
}