namespace CommandLineProcessorWinForms
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands;
    using CommandLineProcessorContracts.Events;

    public class CommandLineWinFormsHelper : ICommandLineWinFormsHelper
    {
        private readonly ICommandHistoryWriter historyWriter;

        private readonly ICommandInputControlAccess inputControlAccess;

        private readonly IInputHandlerService inputHandler;

        private readonly ICommandLineProcessorService processor;

        private bool automaticHelp;

        private bool outputDiagnostics;

        private bool outputErrors;

        public CommandLineWinFormsHelper(
            ICommandInputControlAccess inputControlAccess,
            IInputHandlerService inputHandler,
            ICommandHistoryWriter historyWriter,
            ICommandLineProcessorService processor)
        {
            this.inputControlAccess = inputControlAccess;
            this.inputHandler = inputHandler;
            this.historyWriter = historyWriter;
            this.processor = processor;
            this.inputHandler.Processor = this.processor;
        }

        public bool AutomaticHelp
        {
            get => automaticHelp;
            set
            {
                if (value != automaticHelp)
                {
                    if (value)
                    {
                        processor.HelpRequest += CommandLineProcessor_HelpRequest;
                    }
                    else
                    {
                        processor.HelpRequest -= CommandLineProcessor_HelpRequest;
                    }

                    automaticHelp = value;
                }
            }
        }

        public bool OutputDiagnostics
        {
            get => outputDiagnostics;
            set
            {
                if (value != outputDiagnostics)
                {
                    if (value)
                    {
                        processor.ActiveCommandChanged += CommandLineProcessor_ActiveCommandChanged;
                        processor.StatusChanged += CommandLineProcessor_StatusChanged;
                        processor.ProcessingRawInput += CommandLineProcessor_ProcessingRawInput;
                        processor.ProcessingInputElement += CommandLineProcessor_ProcessingInputElement;
                    }
                    else
                    {
                        processor.ActiveCommandChanged -= CommandLineProcessor_ActiveCommandChanged;
                        processor.StatusChanged -= CommandLineProcessor_StatusChanged;
                        processor.ProcessingRawInput -= CommandLineProcessor_ProcessingRawInput;
                        processor.ProcessingInputElement -= CommandLineProcessor_ProcessingInputElement;
                    }

                    outputDiagnostics = value;
                }
            }
        }

        public bool OutputErrors
        {
            get => outputErrors;
            set
            {
                if (value != outputErrors)
                {
                    if (value)
                    {
                        processor.ProcessInputError += CommandLineProcessor_ProcessInputError;
                        processor.CommandRegistrationError += CommandLineProcessor_CommandRegistrationError;
                    }
                    else
                    {
                        processor.ProcessInputError -= CommandLineProcessor_ProcessInputError;
                        processor.CommandRegistrationError -= CommandLineProcessor_CommandRegistrationError;
                    }

                    outputErrors = value;
                }
            }
        }

        public void HandleKeyDown(KeyEventArgs eventArgs)
        {
            if (inputHandler.AllowKeyPress(eventArgs.KeyValue, inputControlAccess.SelectionStart))
            {
                if (eventArgs.KeyCode == Keys.Enter)
                {
                    eventArgs.Handled = true;
                    eventArgs.SuppressKeyPress = true;
                    var input = inputControlAccess.Text.Substring(inputHandler.MinimumSelectionStart).Trim();
                    processor.ProcessInput(input);
                    UpdateCommandLine();
                }
                else if ((eventArgs.Control && eventArgs.KeyCode == Keys.C) || eventArgs.KeyCode == Keys.Escape)
                {
                    eventArgs.Handled = true;
                    eventArgs.SuppressKeyPress = true;
                    processor.ProcessInput(processor.Settings.CancelToken);
                    UpdateCommandLine();
                }
                else if (eventArgs.KeyCode == Keys.Up || eventArgs.KeyCode == Keys.Down)
                {
                    eventArgs.Handled = true;
                    eventArgs.SuppressKeyPress = true;
                    switch (eventArgs.KeyCode)
                    {
                        case Keys.Up:
                            var previousCommand = processor.HistoryService.Previous();
                            if (previousCommand != null)
                            {
                                inputControlAccess.Text = inputHandler.GetPrompt() + previousCommand.PrimarySelector;
                                inputControlAccess.SelectionStart = inputControlAccess.Text.Length;
                            }
                            else
                            {
                                historyWriter.WriteLine(processor.HistoryService.Settings.BeginningOfHistoryText);
                            }

                            break;
                        case Keys.Down:
                            var nextCommand = processor.HistoryService.Next();
                            if (nextCommand != null)
                            {
                                inputControlAccess.Text = inputHandler.GetPrompt() + nextCommand.PrimarySelector;
                                inputControlAccess.SelectionStart = inputControlAccess.Text.Length;
                            }
                            else
                            {
                                historyWriter.WriteLine(processor.HistoryService.Settings.EndOfHistoryText);
                            }

                            break;
                    }
                }
            }
            else
            {
                eventArgs.Handled = true;
                eventArgs.SuppressKeyPress = true;
                inputControlAccess.SelectionStart = inputControlAccess.Text.Length;
            }
        }

        public void UpdateCommandLine()
        {
            inputControlAccess.Focus();
            inputControlAccess.Text = inputHandler.GetPrompt();
            inputControlAccess.SelectionStart = inputControlAccess.Text.Length;
        }

        private void CommandLineProcessor_ActiveCommandChanged(object sender, CommandLineCommandChangedEventArgs e)
        {
            WriteDiagnosticLine(
                $"Active Command Change: {GetCommandNameForStateText(e.PriorCommand)} -> {GetCommandNameForStateText(e.ActiveCommand)} Stack Depth: {processor.StackDepth}");
        }

        private void CommandLineProcessor_CommandRegistrationError(object sender, CommandLineErrorEventArgs e)
        {
            historyWriter.WriteLine($"Registration Error: {e.Exception.Message}");
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
            WriteDiagnosticLine($"{text}");
        }

        private void CommandLineProcessor_ProcessingRawInput(object sender, CommandLineProcessInputEventArgs e)
        {
            WriteDiagnosticLine($"Current Raw Input: {e.InputText}");
        }

        private void CommandLineProcessor_ProcessInputError(object sender, CommandLineErrorEventArgs e)
        {
            historyWriter.WriteLine($"Input Error: {e.Exception.Message}");
        }

        private void CommandLineProcessor_StatusChanged(object sender, CommandLineStatusChangedEventArgs e)
        {
            WriteDiagnosticLine($"Status Change: {e.PriorStatus} -> {e.Status}");
        }

        private void DisplayActiveCommandHelp(
            ICommandDescriptor activeCommandDescriptor,
            IEnumerable<ICommandDescriptor> subCommandDescriptors)
        {
            historyWriter.WriteLine("Current Command Help:");
            var aliases = GetAliasesForHelp(activeCommandDescriptor);
            historyWriter.WriteLine(GenerateHelpTextForCommand(activeCommandDescriptor, aliases));
            if (subCommandDescriptors != null)
            {
                historyWriter.WriteLine("Options:");
                foreach (var subCommand in subCommandDescriptors)
                {
                    aliases = GetAliasesForHelp(subCommand);
                    historyWriter.WriteLine($"\t{GenerateHelpTextForCommand(subCommand, aliases)}");
                }
            }
        }

        private void DisplayGlobalHelp(IEnumerable<ICommandDescriptor> commands)
        {
            historyWriter.WriteLine("Available Commands:");
            foreach (var command in commands)
            {
                var aliases = GetAliasesForHelp(command);
                historyWriter.WriteLine(GenerateHelpTextForCommand(command, aliases));
            }
        }

        private string GenerateHelpTextForCommand(ICommandDescriptor command, string aliases)
        {
            return $"{command.PrimarySelector}{aliases} - {command.Name}: {command.HelpText}";
        }

        private string GetAliasesForHelp(ICommandDescriptor command)
        {
            return command.AliasSelectors.Any() ? $" ({string.Join(",", command.AliasSelectors)})" : string.Empty;
        }

        private string GetCommandNameForStateText(ICommand command)
        {
            return $"{command?.Name ?? "None"} ({command?.GetType().Name ?? "N/A"})";
        }

        private void WriteDiagnosticLine(string text)
        {
            historyWriter.WriteLine($"DIAGNOSTIC: {text}");
            UpdateCommandLine();
        }
    }
}