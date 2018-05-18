namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public interface ICommandRegistration
    {
        IEnumerable<ICommand> RegisteredCommands { get; }            
    }
}