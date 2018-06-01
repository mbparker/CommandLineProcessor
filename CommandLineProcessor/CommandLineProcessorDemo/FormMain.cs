namespace CommandLineProcessorDemo
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands;
    using CommandLineProcessorContracts.Commands.Registration;
    using CommandLineProcessorContracts.Events;

    using CommandLineProcessorDemo.DemoCommands;

    using CommandLineProcessorWinForms;

    public partial class FormMain : Form
    {
        private readonly ICommandInputControlAccess commandControlAccess;

        private readonly ICommandHistoryControlAccess commandHistoryAccess;

        private readonly ICommandLineProcessorService commandLineProcessor;

        private readonly IRootCommandRegistration commandRegistration;

        private readonly ICommandLineWinFormsHelper helper;

        public FormMain(
            ICommandLineProcessorService commandLineProcessor,
            IRootCommandRegistration commandRegistration,
            ICommandInputControlAccess commandControlAccess,
            ICommandHistoryControlAccess commandHistoryAccess,
            ICommandLineWinFormsHelper helper)
        {
            InitializeComponent();
            this.commandLineProcessor = commandLineProcessor;
            this.commandLineProcessor.CommandRegistrationError += CommandLineProcessor_CommandRegistrationError;
            this.commandRegistration = commandRegistration;
            this.commandControlAccess = commandControlAccess;
            this.commandHistoryAccess = commandHistoryAccess;
            this.helper = helper;
        }

        private void CommandLineProcessor_CommandRegistrationError(object sender, CommandLineErrorEventArgs e)
        {
            MessageBox.Show(this, e.Exception.Message);
        }

        private IEnumerable<ICommand> CreateCommands()
        {
            return CommandRegistration.Register(commandRegistration);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            commandControlAccess.InputControl = textBox_CommandLine;
            commandHistoryAccess.HistoryControl = textBox_CommandHistory;
            helper.AutomaticHelp = true;
            helper.OutputDiagnostics = false;
            helper.OutputErrors = true;
            commandLineProcessor.RegisterCommands(CreateCommands());
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            helper.UpdateCommandLine();
        }

        private void textBox_CommandLine_KeyDown(object sender, KeyEventArgs e)
        {
            helper.HandleKeyDown(e);
        }
    }
}