namespace CommandLineProcessorLib
{
    using System;

    using CommandLineProcessorContracts;

    public class GenericInputCommand : BaseCommand, IInputCommand
    {
        private readonly Action<ICommandContext, string> applyInputAction;

        public GenericInputCommand(
            ICommandDescriptor descriptor,
            string promptText,
            Action<ICommandContext, string> applyInputAction)
            : base(descriptor)
        {
            Prompt = promptText;
            this.applyInputAction = applyInputAction;
        }

        public ICommand NextCommand { get; set; }

        public string Prompt { get; }

        public void ApplyInput(ICommandContext context, string inputText)
        {
            applyInputAction?.Invoke(context, inputText);
        }
    }
}