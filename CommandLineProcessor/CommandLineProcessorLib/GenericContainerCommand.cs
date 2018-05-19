namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands;

    public class GenericContainerCommand : BaseCommand, IContainerCommand, IContainerCommandEdit
    {
        private readonly List<ICommand> children;

        private readonly Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc;

        public GenericContainerCommand(
            ICommandDescriptor descriptor,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc)
            : base(descriptor)
        {
            this.getDefaultCommandFunc = getDefaultCommandFunc;
            children = new List<ICommand>();
        }

        public IEnumerable<ICommand> Children => children;

        public void AddChild(ICommand command)
        {
            children.Add(command);
        }

        public ICommand GetDefaultCommand(ICommandContext context)
        {
            return getDefaultCommandFunc?.Invoke(context, Children) ?? Children.FirstOrDefault();
        }

        public string GetDefaultCommandSelector(ICommandContext context)
        {
            return GetDefaultCommand(context)?.PrimarySelector;
        }
    }
}