namespace CommandLineLibrary.Demo.Commands
{
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Contracts.Commands;

    public class ConfigCommand
    {
        private const string Disabled = "Disabled";

        private const string Enabled = "Enabled";

        private readonly ICommandLineInterface commandLineInterface;

        private readonly string[] modeOptions = { Disabled, Enabled };

        public ConfigCommand(ICommandLineInterface commandLineInterface)
        {
            this.commandLineInterface = commandLineInterface;
        }

        public void Config_AutomaticHelp_ApplyInputMethod(ICommandContext context, string input)
        {
            commandLineInterface.AutomaticHelp = input.ToUpper().StartsWith("E");
        }

        public string Config_AutomaticHelp_GetDefault(ICommandContext context)
        {
            return commandLineInterface.AutomaticHelp ? Enabled : Disabled;
        }

        public string Config_AutomaticHelp_GetPromptText(ICommandContext context)
        {
            return string.Join(",", modeOptions);
        }

        public ICommand Config_GetDefaultCommand(ICommandContext context, IEnumerable<ICommand> commands)
        {
            return commands.OrderBy(x => x.PrimarySelector).First();
        }

        public void Config_OutputDiagnostics_ApplyInputMethod(ICommandContext context, string input)
        {
            commandLineInterface.OutputDiagnostics = input.ToUpper().StartsWith("E");
        }

        public string Config_OutputDiagnostics_GetDefault(ICommandContext context)
        {
            return commandLineInterface.OutputDiagnostics ? Enabled : Disabled;
        }

        public string Config_OutputDiagnostics_GetPromptText(ICommandContext context)
        {
            return string.Join(",", modeOptions);
        }

        public void Config_OutputErrors_ApplyInputMethod(ICommandContext context, string input)
        {
            commandLineInterface.OutputErrors = input.ToUpper().StartsWith("E");
        }

        public string Config_OutputErrors_GetDefault(ICommandContext context)
        {
            return commandLineInterface.OutputErrors ? Enabled : Disabled;
        }

        public string Config_OutputErrors_GetPromptText(ICommandContext context)
        {
            return string.Join(",", modeOptions);
        }
    }
}