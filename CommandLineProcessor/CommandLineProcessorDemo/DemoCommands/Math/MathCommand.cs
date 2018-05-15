namespace CommandLineProcessorDemo.DemoCommands.Math
{
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class MathCommand : BaseCommand, IContainerCommand
    {
        public MathCommand(IEnumerable<ICommand> children)
        {
            Children = children;
            foreach (var child in Children)
            {
                child.Parent = this;
            }
        }

        public override IEnumerable<string> AliasSelectors => new[] { "M" };

        public IEnumerable<ICommand> Children { get; }

        public override string HelpText => "Supports performing various mathematical operations.";

        public override string Name => "Mathematical Operations";

        public override string PrimarySelector => "Math";
    }
}