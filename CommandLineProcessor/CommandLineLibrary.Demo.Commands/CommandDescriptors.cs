namespace CommandLineLibrary.Demo.Commands
{
    using CommandLineLibrary.Contracts.Commands;

    public class CommandDescriptors
    {
        public ICommandDescriptor Echo_Descriptor =>
            new CommandDescriptor
                {
                    PrimarySelector = "Echo",
                    AliasSelectors = new[] { "E" },
                    Name = "Echo Text",
                    HelpText = "Writes out the specified text to the history window."
                };

        public ICommandDescriptor Exit_Descriptor =>
            new CommandDescriptor
                {
                    PrimarySelector = "Exit",
                    AliasSelectors = new[] { "EX" },
                    Name = "Exit Application",
                    HelpText = "Terminates the program, with confirmation."
                };

        public ICommandDescriptor Math_Add_Descriptor =>
            new CommandDescriptor
                {
                    PrimarySelector = "Add",
                    AliasSelectors = new[] { "A" },
                    Name = "Addition",
                    HelpText = "Adds two numbers and displays the result."
                };

        public ICommandDescriptor Math_Descriptor =>
            new CommandDescriptor
                {
                    PrimarySelector = "Math",
                    Name = "Mathematical Operations",
                    HelpText = "Supports performing various mathematical operations."
                };

        public ICommandDescriptor Math_Mult_Descriptor =>
            new CommandDescriptor
                {
                    PrimarySelector = "Mult",
                    AliasSelectors = new[] { "M" },
                    Name = "Multiplication",
                    HelpText = "Multiplies two numbers and displays the result."
                };

        public ICommandDescriptor Config_Descriptor =>
            new CommandDescriptor
                {
                    PrimarySelector = "Config",
                    Name = "Configuration Settings",
                    HelpText = "Supports checking and updating configuration settings."
                };

        public ICommandDescriptor Config_AutomaticHelp_Descriptor =>
            new CommandDescriptor
                {
                    PrimarySelector = "Autohelp",
                    AliasSelectors = new[] { "A" },
                    Name = "Automatic Help",
                    HelpText = "Controls whether the automatic help text system is active."
                };

        public ICommandDescriptor Config_OutputDiagnostics_Descriptor =>
            new CommandDescriptor
                {
                    PrimarySelector = "Diagout",
                    AliasSelectors = new[] { "D" },
                    Name = "Output Diagnostics",
                    HelpText = "Controls whether diagnostic messages are written to the console."
                };

        public ICommandDescriptor Config_OutputErrors_Descriptor =>
            new CommandDescriptor
                {
                    PrimarySelector = "Errorsout",
                    AliasSelectors = new[] { "E" },
                    Name = "Output Errors",
                    HelpText = "Controls whether error messages are written to the console."
                };
    }
}