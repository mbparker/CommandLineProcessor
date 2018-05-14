namespace CommandLineProcessorDemo
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using CommandLineProcessorContracts;

    using CommandLineProcessorDemo.DemoCommands;

    public partial class FormMain : Form
    {
        private readonly ICommandLineProcessorService commandLineProcessor;

        private readonly IInputHandlerService inputHandler;

        public FormMain(ICommandLineProcessorService commandLineProcessor, IInputHandlerService inputHandler)
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
        }

        private void CommandLineProcessor_HelpRequest(object sender, CommandLineHelpEventArgs e)
        {
            if (e.CommandInfo != null)
            {
                textBox_CommandHistory.AppendText($"Current Command Help:{Environment.NewLine}");
                textBox_CommandHistory.AppendText($"{e.CommandInfo.PrimarySelector} - {e.CommandInfo.HelpText}{Environment.NewLine}");
                if (e.SubCommandInfo != null)
                {
                    textBox_CommandHistory.AppendText($"Options:{Environment.NewLine}");
                    foreach (var subCommand in e.SubCommandInfo)
                    {
                        textBox_CommandHistory.AppendText($"\t{subCommand.PrimarySelector} - {subCommand.HelpText}{Environment.NewLine}");
                    }
                }
            }
            else
            {
                textBox_CommandHistory.AppendText($"Available Commands:{Environment.NewLine}");
                foreach (var command in e.SubCommandInfo)
                {
                    textBox_CommandHistory.AppendText($"{command.PrimarySelector} - {command.HelpText}{Environment.NewLine}");
                }
            }
        }

        private void CommandLineProcessor_ActiveCommandChanged(object sender, CommandLineCommandChangedEventArgs e)
        {
            textBox_Diagnostics.AppendText(
                $"Active Command Change: {e.PriorCommand?.Name ?? "None"} ({e.PriorCommand?.GetType().Name ?? "N/A"}) -> {e.ActiveCommand?.Name ?? "None"} ({e.ActiveCommand?.GetType().Name ?? "N/A"}) Stack Depth: {commandLineProcessor.StackDepth}{Environment.NewLine}");
            UpdateCommandLine();
        }

        private void CommandLineProcessor_CommandRegistrationError(object sender, CommandLineErrorEventArgs e)
        {
            MessageBox.Show(this, e.Exception.Message);
        }

        private void CommandLineProcessor_ProcessingInputElement(object sender, CommandLineProcessInputEventArgs e)
        {
            textBox_Diagnostics.AppendText($"Current Input: {e.InputText}{Environment.NewLine}");
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

        private void FormMain_Load(object sender, EventArgs e)
        {
            commandLineProcessor.RegisterCommands(GetCommands());
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            UpdateCommandLine();
        }

        private IEnumerable<ICommand> GetCommands()
        {
            var echoExecute = new EchoExecuteCommand(
                s => textBox_CommandHistory.AppendText("You Entered: " + s + Environment.NewLine));
            var echoInput = new EchoInputCommand(echoExecute);

            var addExecute = new AddExecuteCommand(
                v => textBox_CommandHistory.AppendText("Result: " + v + Environment.NewLine));
            var addInput2 = new AddInputCommand2(addExecute);
            var addInput1 = new AddInputCommand1(addInput2);

            var multiplyExecute = new MultiplyExecuteCommand(
                v => textBox_CommandHistory.AppendText("Result: " + v + Environment.NewLine));
            var multiplyInput2 = new MultiplyInputCommand2(multiplyExecute);
            var multiplyInput1 = new MultiplyInputCommand1(multiplyInput2);

            var mathCommand = new MathCommand(new ICommand[] { multiplyInput1, addInput1 });

            var exitCommand = new ExitCommand(Application.Exit);

            return new ICommand[] { echoInput, mathCommand, exitCommand };
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
                else
                {
                    if (e.Control && e.KeyCode == Keys.C)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        commandLineProcessor.ProcessInput(commandLineProcessor.Settings.CancelToken);                        
                        UpdateCommandLine();
                    }
                }
            }
            else
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
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