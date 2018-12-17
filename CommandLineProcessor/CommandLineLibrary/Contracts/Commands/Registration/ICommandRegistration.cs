namespace CommandLineLibrary.Contracts.Commands.Registration
{
    using System.Collections.Generic;

    using CommandLineLibrary.Contracts.Commands;

    public interface ICommandRegistration
    {
        void Clear();

        IEnumerable<ICommand> RegisteredCommands { get; }            
    }
}