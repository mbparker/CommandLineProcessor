namespace CommandLineProcessorDemo.DemoCommands
{
    using System.Windows.Forms;

    using CommandLineProcessorContracts;

    public class ExitCommand
    {
        public void Exit_ApplyInputMethod(ICommandContext context, string input)
        {
            if (input.ToUpper().StartsWith("Y"))
            {
                Application.Exit();
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