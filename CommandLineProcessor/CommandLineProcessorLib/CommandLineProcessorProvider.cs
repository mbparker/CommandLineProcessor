namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineProcessorContracts;

    using CommandLineProcessorEntity;

    public class CommandLineProcessorProvider : ICommandLineProcessorService
    {
        private readonly ICommandPathCalculator commandPathCalculator;

        private readonly ICommandRepositoryService commandRepository;

        public CommandLineProcessorProvider(
            ICommandRepositoryService commandRepository,
            ICommandPathCalculator commandPathCalculator)
        {
            this.commandRepository = commandRepository;
            this.commandPathCalculator = commandPathCalculator;
            State = CommandLineState.WaitingForCommandRegistration;
            Settings = new CommandLineSettings();
        }

        public ICommand ActiveCommand { get; private set; }

        public string LastInput { get; private set; }

        public CommandLineSettings Settings { get; set; }

        public CommandLineState State { get; private set; }

        public void ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Value is required.", nameof(input));
            }            

            string[] inputElements = SplitInput(input);
            ProcessInput(inputElements);
        }

        public void RegisterCommands(IEnumerable<ICommand> commands)
        {
            commandRepository.Load(commands);
            State = CommandLineState.WaitingForCommand;
        }

        private bool ActiveCommandHasNoChildren()
        {
            return !((ActiveCommand as IContainerCommand)?.Children.Any() ?? false);
        }

        private bool ActiveCommandHasOneChild()
        {
            return ((ActiveCommand as IContainerCommand)?.Children.Count() ?? 0) == 1;
        }

        private void CancelCurrentCommand()
        {
            ActiveCommand = ActiveCommand?.Parent;
            while (ActiveCommand != null)
            {
                if (ActiveCommand.CommandIs<IInputCommand>() || ActiveCommand.CommandIs<IContainerCommand>())
                {
                    UpdateStateForEndOfProcessing();
                    return;
                }

                ActiveCommand = ActiveCommand.Parent;
            }
        }

        private string GetFullyQualifiedInput(string input)
        {
            return commandPathCalculator.CalculateFullyQualifiedPath(ActiveCommand, input);
        }

        private void HandleCommand(string input)
        {
            var fullyQualifiedInput = GetFullyQualifiedInput(input);
            SelectActiveCommandFromQuialifiedInput(fullyQualifiedInput);
            HandleCommand();
        }

        private void HandleCommand()
        {
            if (ActiveCommand.CommandIs<IContainerCommand>())
            {
                if (ActiveCommandHasNoChildren())
                {
                    TryExecuteActiveCommand();
                }
                else if (ActiveCommandHasOneChild())
                {
                    MakeSingleChildActive();
                    TryExecuteActiveCommand();
                }
            }
            else if (ActiveCommand.CommandIs<IExecutableCommand>())
            {
                TryExecuteActiveCommand();
            }

            UpdateStateForEndOfProcessing();
        }

        private void HandleInput(string input)
        {
            (ActiveCommand as IInputCommand)?.ApplyInput(input);
            ActiveCommand = (ActiveCommand as IInputCommand)?.NextCommand;
            HandleCommand();
        }

        private void MakeSingleChildActive()
        {
            ActiveCommand = (ActiveCommand as IContainerCommand)?.Children.First();
        }

        private void ProcessInput(IEnumerable<string> inputs)
        {
            foreach (var input in inputs)
            {
                ProcessInputBasedOnState(input);
            }
        }

        private void ProcessInputBasedOnState(string input)
        {
            LastInput = input;

            if (input.ToUpper() == Settings.CancelToken)
            {
                CancelCurrentCommand();
            }
            else if (State == CommandLineState.WaitingForCommand)
            {
                HandleCommand(input);
            }
            else if (State == CommandLineState.WaitingForInput)
            {
                HandleInput(input);
            }
        }

        private void SelectActiveCommandFromQuialifiedInput(string fullyQualifiedInput)
        {
            ActiveCommand = commandRepository[fullyQualifiedInput];
        }

        private string[] SplitInput(string input)
        {
            return input.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void TryExecuteActiveCommand()
        {
            if (ActiveCommand is IExecutableCommand exe)
            {
                exe.Execute(exe.GetArguments());
                ActiveCommand = null;
            }
        }

        private void UpdateStateForEndOfProcessing()
        {
            if (ActiveCommand?.CommandIs<IInputCommand>() ?? false)
            {
                State = CommandLineState.WaitingForInput;
            }
            else
            {
                State = CommandLineState.WaitingForCommand;
            }
        }
    }
}