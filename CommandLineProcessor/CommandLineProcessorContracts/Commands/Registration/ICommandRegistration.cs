namespace CommandLineProcessorContracts.Commands.Registration
{
    using System.Collections.Generic;

    using CommandLineProcessorContracts.Commands;

    public interface ICommandRegistration
    {
        IEnumerable<ICommand> RegisteredCommands { get; }            
    }
}