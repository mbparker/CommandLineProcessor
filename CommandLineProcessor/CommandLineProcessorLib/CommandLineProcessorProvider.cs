namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineProcessorContracts;

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
        }

        public ICommand ActiveCommand { get; private set; }

        public string LastInput { get; private set; }

        public CommandLineState State { get; private set; }

        public void ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Value is required.", nameof(input));
            }

            LastInput = input;

            ProcessInputBasedOnState(input);
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
            if (ActiveCommandHasNoChildren())
            {
                TryExecuteActiveCommand();
            }

            if (ActiveCommandHasOneChild())
            {
                MakeSingleChildActive();
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

        private void ProcessInputBasedOnState(string input)
        {
            if (State == CommandLineState.WaitingForCommand)
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

        private void TryExecuteActiveCommand()
        {
            var executable = ActiveCommand as IExecutableCommand;
            if (executable != null)
            {
                executable.Execute(executable.GetArguments());
                ActiveCommand = null;
            }
        }

        private void UpdateStateForEndOfProcessing()
        {
            if ((ActiveCommand as IInputCommand) != null)
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