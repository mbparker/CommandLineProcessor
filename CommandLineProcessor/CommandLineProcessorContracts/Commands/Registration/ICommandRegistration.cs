namespace CommandLineProcessorContracts.Commands.Registration
{
    using System.Collections.Generic;

    using CommandLineProcessorContracts.Commands;

    public interface ICommandRegistration
    {
        void Clear();

        IEnumerable<ICommand> RegisteredCommands { get; }            
    }
}