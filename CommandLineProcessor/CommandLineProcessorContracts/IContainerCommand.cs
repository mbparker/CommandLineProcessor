namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public interface IContainerCommand : ICommand
    {
        IEnumerable<ICommand> Children { get; }

        ICommand GetDefaultCommand(ICommandContext context);

        string GetDefaultCommandSelector(ICommandContext context);
    }
}