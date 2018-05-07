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
            this.inputHandler = inputHandler;
            this.inputHandler.GetActiveCommandFunc = () => commandLineProcessor.ActiveCommand;
            this.inputHandler.PromptRoot = "Command";
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
            var exeCommand = new EchoExecuteCommand(s => textBox_CommandHistory.AppendText(s + Environment.NewLine));
            var inputCommand = new EchoInputCommand(exeCommand);
            exeCommand.Parent = inputCommand;
            return new[] { inputCommand };
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
                    TryProcessInput(input);
                    UpdateCommandLine();
                }
            }
            else
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void TryProcessInput(string input)
        {
            try
            {
                commandLineProcessor.ProcessInput(input);
            }
            catch (Exception e)
            {
                textBox_CommandHistory.AppendText(e.Message + Environment.NewLine);
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