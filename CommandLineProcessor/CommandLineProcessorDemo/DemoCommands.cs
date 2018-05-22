namespace CommandLineProcessorDemo
{
    using System;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands;

    using CommandLineProcessorLib;

    public class DemoCommandsLogic
    {
        private readonly ICommandHistoryAccess commandHistoryAccess;

        public DemoCommandsLogic(ICommandHistoryAccess commandHistoryAccess)
        {
            this.commandHistoryAccess = commandHistoryAccess;
        }

        public void ApplyInputMethod(ICommandContext context, string input)
        {
            commandHistoryAccess.CommandHistoryControl.AppendText($"{input}{Environment.NewLine}");
        }

        public string GetDefault(ICommandContext context)
        {
            return "something";
        }

        public string GetPromptText(ICommandContext context)
        {
            return "Type something and I'll repeat it.";
        }
    }

    public class DemoCommandsDescriptors
    {
        public ICommandDescriptor Descriptor =>
            new CommandDescriptor
                {
                    PrimarySelector = "Test", AliasSelectors = new string[0], Name = "Test Custom Command",
                    HelpText = "This is the test custom command"
                };
    }
}