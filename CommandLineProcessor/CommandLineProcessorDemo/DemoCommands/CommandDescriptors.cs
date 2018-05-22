namespace CommandLineProcessorDemo.DemoCommands
{
    using CommandLineProcessorContracts.Commands;

    using CommandLineProcessorLib;

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
    }
}