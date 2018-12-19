namespace CommandLineLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Contracts.Commands;
    using CommandLineLibrary.Contracts.Events;

    public class CommandLineInterface : ICommandLineInterface
    {
        private readonly ICommandLineProcessorService commandLineProcessor;

        private readonly ICommandHistoryWriter historyWriter;

        private readonly IInputHandlerService inputHandler;

        private readonly List<char> keyedCharacters;

        private bool automaticHelp;

        private int keyedCharacterIndex;

        private bool outputDiagnostics;

        private bool outputErrors;

        public CommandLineInterface(
            ICommandLineProcessorService commandLineProcessor,
            IInputHandlerService inputHandler,
            ICommandHistoryWriter historyWriter)
        {
            this.commandLineProcessor = commandLineProcessor;
            this.inputHandler = inputHandler;
            this.inputHandler.Processor = this.commandLineProcessor;
            this.historyWriter = historyWriter;
            keyedCharacters = new List<char>();
            keyedCharacterIndex = -1;
            Console.TreatControlCAsInput = true;
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
                        commandLineProcessor.HelpRequest += CommandLineProcessor_HelpRequest;
                    }
                    else
                    {
                        commandLineProcessor.HelpRequest -= CommandLineProcessor_HelpRequest;
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
                        commandLineProcessor.ActiveCommandChanged += CommandLineProcessor_ActiveCommandChanged;
                        commandLineProcessor.StatusChanged += CommandLineProcessor_StatusChanged;
                        commandLineProcessor.ProcessingRawInput += CommandLineProcessor_ProcessingRawInput;
                        commandLineProcessor.ProcessingInputElement += CommandLineProcessor_ProcessingInputElement;
                    }
                    else
                    {
                        commandLineProcessor.ActiveCommandChanged -= CommandLineProcessor_ActiveCommandChanged;
                        commandLineProcessor.StatusChanged -= CommandLineProcessor_StatusChanged;
                        commandLineProcessor.ProcessingRawInput -= CommandLineProcessor_ProcessingRawInput;
                        commandLineProcessor.ProcessingInputElement -= CommandLineProcessor_ProcessingInputElement;
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
                        commandLineProcessor.ProcessInputError += CommandLineProcessor_ProcessInputError;
                        commandLineProcessor.CommandRegistrationError += CommandLineProcessor_CommandRegistrationError;
                    }
                    else
                    {
                        commandLineProcessor.ProcessInputError -= CommandLineProcessor_ProcessInputError;
                        commandLineProcessor.CommandRegistrationError -= CommandLineProcessor_CommandRegistrationError;
                    }

                    outputErrors = value;
                }
            }
        }

        public void Run(IEnumerable<ICommand> commandLexicon)
        {
            commandLineProcessor.RegisterCommands(commandLexicon);
            UpdateCommandLine();
            while (true)
            {
                HandleKeyPress(Console.ReadKey(true));
            }
        }

        private void ClearKeyBuffer()
        {
            keyedCharacters.Clear();
            keyedCharacterIndex = -1;
        }

        private void ClearLine()
        {
            Console.CursorLeft = 0;
            Console.Write(new string(' ', Console.BufferWidth));
            Console.CursorLeft = 0;
            Console.CursorTop--;
        }

        private void CommandLineProcessor_ActiveCommandChanged(object sender, CommandLineCommandChangedEventArgs e)
        {
            WriteDiagnosticLine(
                $"Active Command Change: {GetCommandNameForStateText(e.PriorCommand)} -> {GetCommandNameForStateText(e.ActiveCommand)} Stack Depth: {commandLineProcessor.StackDepth}");
        }

        private void CommandLineProcessor_CommandRegistrationError(object sender, CommandLineErrorEventArgs e)
        {
            historyWriter.WriteLine($"Registration Error: {e.Exception.Message}");
        }

        private void CommandLineProcessor_HelpRequest(object sender, CommandLineHelpEventArgs e)
        {
            DisplaySyntaxHelp(e.SyntaxInfo);
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
            historyWriter.WriteLine($"{Environment.NewLine}Input Error: {e.Exception.Message}");
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

        private void DisplaySyntaxHelp(IDictionary<string, string> syntaxInfo)
        {
            historyWriter.WriteLine("Syntax Tokens:");
            foreach (var pair in syntaxInfo)
            {
                historyWriter.WriteLine($"{pair.Key}: {pair.Value}");
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

        private void HandleKeyPress(ConsoleKeyInfo info)
        {
            if (inputHandler.AllowKeyPress(info.KeyChar, Console.CursorLeft))
            {
                if (info.Key == ConsoleKey.Enter)
                {
                    var input = string.Join(string.Empty, keyedCharacters).Trim();
                    commandLineProcessor.ProcessInput(input);
                    Console.WriteLine();
                    UpdateCommandLine();
                }
                else if (((info.Modifiers & ConsoleModifiers.Control) != 0 && info.Key == ConsoleKey.C)
                         || info.Key == ConsoleKey.Escape)
                {
                    commandLineProcessor.ProcessInput(commandLineProcessor.Settings.CancelToken);
                    UpdateCommandLine();
                }
                else if (info.Key == ConsoleKey.UpArrow || info.Key == ConsoleKey.DownArrow)
                {
                    switch (info.Key)
                    {
                        case ConsoleKey.UpArrow:
                            UpdateCommandLine();
                            var previousCommand = commandLineProcessor.HistoryService.Previous();
                            if (previousCommand != null)
                            {
                                Console.Write(previousCommand.PrimarySelector);
                                keyedCharacters.AddRange(previousCommand.PrimarySelector);
                                keyedCharacterIndex += previousCommand.PrimarySelector.Length;
                            }
                            else
                            {
                                historyWriter.Write(
                                    commandLineProcessor.HistoryService.Settings.BeginningOfHistoryText);
                            }

                            break;
                        case ConsoleKey.DownArrow:
                            UpdateCommandLine();
                            var nextCommand = commandLineProcessor.HistoryService.Next();
                            if (nextCommand != null)
                            {
                                Console.Write(nextCommand.PrimarySelector);
                                keyedCharacters.AddRange(nextCommand.PrimarySelector);
                                keyedCharacterIndex += nextCommand.PrimarySelector.Length;
                            }
                            else
                            {
                                historyWriter.Write(commandLineProcessor.HistoryService.Settings.EndOfHistoryText);
                            }

                            break;
                    }
                }
                else
                {
                    if (info.Key == ConsoleKey.LeftArrow || info.Key == ConsoleKey.RightArrow)
                    {
                        return;
                    }

                    if (info.Key == ConsoleKey.Delete || info.Key == ConsoleKey.Backspace)
                    {
                        if (keyedCharacterIndex < 0)
                        {
                            UpdateCommandLine();
                        }
                        else
                        {
                            Console.CursorLeft--;
                            Console.Write(' ');
                            Console.CursorLeft--;
                            keyedCharacters.RemoveAt(keyedCharacterIndex);
                            keyedCharacterIndex--;
                        }

                        return;
                    }

                    Console.Write(info.KeyChar);
                    keyedCharacters.Add(info.KeyChar);
                    keyedCharacterIndex++;
                }
            }
        }

        private void UpdateCommandLine()
        {
            ClearKeyBuffer();
            ClearLine();
            WriteCommandPrompt();
        }

        private void WriteCommandPrompt()
        {
            var prompt = inputHandler.GetPrompt();
            Console.Write(prompt);
        }

        private void WriteDiagnosticLine(string text)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            historyWriter.WriteLine($"{Environment.NewLine}DIAGNOSTIC: {text}");
            Console.ForegroundColor = color;
            UpdateCommandLine();
        }
    }
}