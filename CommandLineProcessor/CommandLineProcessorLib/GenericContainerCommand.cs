namespace CommandLineProcessorLib
{
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class GenericContainerCommand : BaseCommand, IContainerCommand, IContainerCommandEdit
    {
        private readonly List<ICommand> children;

        public GenericContainerCommand(ICommandDescriptor descriptor)
            : base(descriptor)
        {
            children = new List<ICommand>();
        }

        public IEnumerable<ICommand> Children => children;

        public void AddChild(ICommand command)
        {
            children.Add(command);
        }
    }
}