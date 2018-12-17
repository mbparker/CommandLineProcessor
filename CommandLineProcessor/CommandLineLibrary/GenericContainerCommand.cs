namespace CommandLineLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Contracts.Commands;

    public class GenericContainerCommand : BaseCommand, IContainerCommand, IContainerCommandEdit
    {
        private readonly List<ICommand> children;

        private readonly Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc;

        public GenericContainerCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc)
            : base(primarySelector, aliasSelectors, name, helpText)
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