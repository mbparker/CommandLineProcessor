namespace CommandLineProcessorDemo.DemoCommands
{
    using CommandLineProcessorContracts;

    public class ExitCommand
    {
        private readonly IApplication application;

        public ExitCommand(IApplication application)
        {
            this.application = application;
        }

        public void Exit_ApplyInputMethod(ICommandContext context, string input)
        {
            if (input.ToUpper().StartsWith("Y"))
            {
                application.Exit();
            }
        }

        public string Exit_GetDefault(ICommandContext context)
        {
            return "N";
        }

        public string Exit_GetPromptText(ICommandContext context)
        {
            return "Are you sure you want to exit? (Y/N)";
        }
    }
}