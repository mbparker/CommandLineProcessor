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

        private readonly Func<ICommandContext, string> getPromptTextFunc;

        public GenericInputCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
            : base(primarySelector, aliasSelectors, name, helpText)
        {
            this.applyInputAction = applyInputAction;
            this.getDefaultFunc = getDefaultFunc;
            this.getPromptTextFunc = getPromptTextFunc;
        }

        public ICommand NextCommand { get; set; }

        public void ApplyInput(ICommandContext context, string inputText)
        {
            applyInputAction?.Invoke(context, inputText);
        }

        public string GetDefaultValue(ICommandContext context)
        {
            return getDefaultFunc?.Invoke(context);
        }

        public string GetPromptText(ICommandContext context)
        {
            return getPromptTextFunc?.Invoke(context);
        }
    }
}