namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands;

    public class GenericInputCommand : BaseCommand, IInputCommand
    {
        private readonly Action<ICommandContext, string> applyInputAction;

        private readonly Func<ICommandContext, string> getDefaultFunc;

        public GenericInputCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            string promptText,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
            : base(primarySelector, aliasSelectors, name, helpText)
        {
            Prompt = promptText;
            this.applyInputAction = applyInputAction;
            this.getDefaultFunc = getDefaultFunc;
        }

        public ICommand NextCommand { get; set; }

        public string Prompt { get; }

        public void ApplyInput(ICommandContext context, string inputText)
        {
            applyInputAction?.Invoke(context, inputText);
        }

        public string GetDefaultValue(ICommandContext context)
        {
            return getDefaultFunc?.Invoke(context);
        }
    }
}