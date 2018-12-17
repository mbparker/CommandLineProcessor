namespace CommandLineLibrary.Demo.Commands
{
    using CommandLineLibrary.Contracts;

    public class EchoCommand
    {
        private readonly ICommandHistoryWriter commandHistoryAccess;

        public EchoCommand(ICommandHistoryWriter commandHistoryAccess)
        {
            this.commandHistoryAccess = commandHistoryAccess;
        }

        public void Echo_ApplyInputMethod(ICommandContext context, string input)
        {
            context.DataStore.Set("TEXT", input);
        }

        public void Echo_Execute(ICommandContext context)
        {
            var text = context.DataStore.Get<string>("TEXT");
            commandHistoryAccess.WriteLine($"You entered: {text}");
        }

        public string Echo_GetDefault(ICommandContext context)
        {
            return "Hello World";
        }

        public string Echo_GetPromptText(ICommandContext context)
        {
            return "Enter Text";
        }
    }
}