namespace CommandLineProcessorDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands;
    using CommandLineProcessorContracts.Commands.Registration;
    using CommandLineProcessorContracts.Events;

    public partial class FormMain : Form
    {
        private readonly ICommandLineProcessorService commandLineProcessor;

        private readonly IRootCommandRegistration commandRegistration;

        private readonly IInputHandlerService inputHandler;

        public FormMain(
            ICommandLineProcessorService commandLineProcessor,
            IInputHandlerService inputHandler,
            IRootCommandRegistration commandRegistration)
        {
            InitializeComponent();
            this.commandLineProcessor = commandLineProcessor;
            this.commandLineProcessor.ActiveCommandChanged += CommandLineProcessor_ActiveCommandChanged;
            this.commandLineProcessor.StatusChanged += CommandLineProcessor_StatusChanged;
            this.commandLineProcessor.ProcessingRawInput += CommandLineProcessor_ProcessingRawInput;
            this.commandLineProcessor.ProcessingInputElement += CommandLineProcessor_ProcessingInputElement;
            this.commandLineProcessor.HelpRequest += CommandLineProcessor_HelpRequest;
            this.commandLineProcessor.ProcessInputError += CommandLineProcessor_ProcessInputError;
            this.commandLineProcessor.CommandRegistrationError += CommandLineProcessor_CommandRegistrationError;
            this.inputHandler = inputHandler;
            this.inputHandler.Processor = commandLineProcessor;
            this.commandRegistration = commandRegistration;
        }

        private void CommandLineProcessor_ActiveCommandChanged(object sender, CommandLineCommandChangedEventArgs e)
        {
            textBox_Diagnostics.AppendText(
                $"Active Command Change: {GetCommandNameForStateText(e.PriorCommand)} -> {GetCommandNameForStateText(e.ActiveCommand)} Stack Depth: {commandLineProcessor.StackDepth}{Environment.NewLine}");
            UpdateCommandLine();
        }

        private void CommandLineProcessor_CommandRegistrationError(object sender, CommandLineErrorEventArgs e)
        {
            MessageBox.Show(this, e.Exception.Message);
        }

        private void CommandLineProcessor_HelpRequest(object sender, CommandLineHelpEventArgs e)
        {
            if (e.CommandInfo != null)
            {
                DisplayActiveCommandHelp(e.CommandInfo, e.SubCommandInfo);
            }
            else
            {
                DisplayGlobalHelp(e.SubCommandInfo);
            }
        }

        private void CommandLineProcessor_ProcessingInputElement(object sender, CommandLineProcessInputEventArgs e)
        {
            var text = inputHandler.GetPrompt() + e.InputText;
            textBox_CommandHistory.AppendText($"{text}{Environment.NewLine}");
            UpdateCommandLine();
        }

        private void CommandLineProcessor_ProcessingRawInput(object sender, CommandLineProcessInputEventArgs e)
        {
            textBox_Diagnostics.AppendText($"Current Raw Input: {e.InputText}{Environment.NewLine}");
            UpdateCommandLine();
        }

        private void CommandLineProcessor_ProcessInputError(object sender, CommandLineErrorEventArgs e)
        {
            textBox_CommandHistory.AppendText($"Error: {e.Exception.Message}{Environment.NewLine}");
        }

        private void CommandLineProcessor_StatusChanged(object sender, CommandLineStatusChangedEventArgs e)
        {
            textBox_Diagnostics.AppendText($"Status Change: {e.PriorStatus} -> {e.Status}{Environment.NewLine}");
            UpdateCommandLine();
        }

        private IEnumerable<ICommand> CreateCommands()
        {
            commandRegistration.RegisterInputCommand(
                "Exit",
                new[] { "EX" },
                "Exit",
                "Terminates the program, with confirmation.",
                "Are you sure you want to exit? (Y/N)",
                (context, input) =>
                    {
                        if (input.ToUpper().StartsWith("Y"))
                        {
                            Application.Exit();
                        }
                    },
                context => "N");

            commandRegistration
                .RegisterInputCommand(
                    "Echo",
                    new[] { "E" },
                    "Echo",
                    "Writes out the specified text to the history window.",
                    "Enter Text",
                    (context, input) => { context.DataStore.Set("TEXT", input); },
                    context => "Hello World").SetChildToExecutableCommand(
                    context =>
                        {
                            textBox_CommandHistory.AppendText(
                                "You Entered: " + context.DataStore.Get<string>("TEXT") + Environment.NewLine);
                        });

            var mathCommandRegistration = commandRegistration.RegisterContainerCommand(
                "Math",
                "Mathematical Operations",
                "Supports performing various mathematical operations.",
                (context, children) => children.Single(x => x.PrimarySelector == "Mult"));
            mathCommandRegistration
                .AddInputCommand(
                    "Add",
                    new[] { "A" },
                    "Addition",
                    "Adds two numbers and displays the result.",
                    "Enter first number",
                    (context, input) => { context.DataStore.Set("N1", double.Parse(input)); }).SetChildToInputCommand(
                    "Enter second number",
                    (context, input) => { context.DataStore.Set("N2", double.Parse(input)); })
                .SetChildToExecutableCommand(
                    context =>
                        {
                            var result = context.DataStore.Get<double>("N1") + context.DataStore.Get<double>("N2");
                            textBox_CommandHistory.AppendText($"Result: {result}{Environment.NewLine}");
                        });
            mathCommandRegistration
                .AddInputCommand(
                    "Mult",
                    new[] { "M" },
                    "Multiplication",
                    "Multiplies two numbers and displays the result.",
                    "Enter first number",
                    (context, input) => { context.DataStore.Set("N1", double.Parse(input)); }).SetChildToInputCommand(
                    "Enter second number",
                    (context, input) => { context.DataStore.Set("N2", double.Parse(input)); })
                .SetChildToExecutableCommand(
                    context =>
                        {
                            var result = context.DataStore.Get<double>("N1") * context.DataStore.Get<double>("N2");
                            textBox_CommandHistory.AppendText($"Result: {result}{Environment.NewLine}");
                        });

            return commandRegistration.RegisteredCommands;
        }

        private void DisplayActiveCommandHelp(
            ICommandDescriptor activeCommandDescriptor,
            IEnumerable<ICommandDescriptor> subCommandDescriptors)
        {
            textBox_CommandHistory.AppendText($"Current Command Help:{Environment.NewLine}");
            var aliases = GetAliasesForHelp(activeCommandDescriptor);
            textBox_CommandHistory.AppendText(GenerateHelpTextForCommand(activeCommandDescriptor, aliases));
            if (subCommandDescriptors != null)
            {
                textBox_CommandHistory.AppendText($"Options:{Environment.NewLine}");
                foreach (var subCommand in subCommandDescriptors)
                {
                    aliases = GetAliasesForHelp(subCommand);
                    textBox_CommandHistory.AppendText($"\t{GenerateHelpTextForCommand(subCommand, aliases)}");
                }
            }
        }

        private void DisplayGlobalHelp(IEnumerable<ICommandDescriptor> commands)
        {
            textBox_CommandHistory.AppendText($"Available Commands:{Environment.NewLine}");
            foreach (var command in commands)
            {
                var aliases = GetAliasesForHelp(command);
                textBox_CommandHistory.AppendText(GenerateHelpTextForCommand(command, aliases));
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            commandLineProcessor.RegisterCommands(CreateCommands());
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            UpdateCommandLine();
        }

        private string GenerateHelpTextForCommand(ICommandDescriptor command, string aliases)
        {
            return $"{command.PrimarySelector}{aliases} - {command.Name}: {command.HelpText}{Environment.NewLine}";
        }

        private string GetAliasesForHelp(ICommandDescriptor command)
        {
            return command.AliasSelectors.Any() ? $" ({string.Join(",", command.AliasSelectors)})" : string.Empty;
        }

        private string GetCommandNameForStateText(ICommand command)
        {
            return $"{command?.Name ?? "None"} ({command?.GetType().Name ?? "N/A"})";
        }

        private void textBox_CommandLine_KeyDown(object sender, KeyEventArgs e)
        {
            if (inputHandler.AllowKeyPress(e.KeyValue, textBox_CommandLine.SelectionStart))
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    var input = textBox_CommandLine.Text.Substring(inputHandler.MinimumSelectionStart).Trim();
                    commandLineProcessor.ProcessInput(input);
                    UpdateCommandLine();
                }
                else if ((e.Control && e.KeyCode == Keys.C) || e.KeyCode == Keys.Escape)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    commandLineProcessor.ProcessInput(commandLineProcessor.Settings.CancelToken);
                    UpdateCommandLine();
                }
                else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    switch (e.KeyCode)
                    {
                        case Keys.Up:
                            var previousCommand = commandLineProcessor.HistoryService.Previous();
                            if (previousCommand != null)
                            {
                                textBox_CommandLine.Text = inputHandler.GetPrompt() + previousCommand.PrimarySelector;
                                textBox_CommandLine.SelectionStart = textBox_CommandLine.Text.Length;
                            }
                            else
                            {
                                textBox_CommandHistory.AppendText(
                                    $"*At beginning of command history*{Environment.NewLine}");
                            }

                            break;
                        case Keys.Down:
                            var nextCommand = commandLineProcessor.HistoryService.Next();
                            if (nextCommand != null)
                            {
                                textBox_CommandLine.Text = inputHandler.GetPrompt() + nextCommand.PrimarySelector;
                                textBox_CommandLine.SelectionStart = textBox_CommandLine.Text.Length;
                            }
                            else
                            {
                                textBox_CommandHistory.AppendText($"*At end of command history*{Environment.NewLine}");
                            }

                            break;
                    }
                }
            }
            else
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                textBox_CommandLine.SelectionStart = textBox_CommandLine.Text.Length;
            }
        }

        private void UpdateCommandLine()
        {
            textBox_CommandLine.Focus();
            textBox_CommandLine.Text = inputHandler.GetPrompt();
            textBox_CommandLine.SelectionStart = textBox_CommandLine.Text.Length;
        }
    }
}