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

        public FormMain(ICommandLineProcessorService commandLineProcessor)
        {
            InitializeComponent();
            this.commandLineProcessor = commandLineProcessor;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            commandLineProcessor.RegisterCommands(GetCommands());
        }

        private IEnumerable<ICommand> GetCommands()
        {
            var exeCommand = new EchoExecuteCommand((s) => memoEdit_CommandHistory.Text += s + Environment.NewLine);
            var inputCommand = new EchoInputCommand(exeCommand);
            exeCommand.Parent = inputCommand;
            return new[] { inputCommand };
        }

        private void textEdit_CommandLine_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                commandLineProcessor.ProcessInput(textEdit_CommandLine.Text);
                textEdit_CommandLine.Text = string.Empty;
            }
        }
    }
}