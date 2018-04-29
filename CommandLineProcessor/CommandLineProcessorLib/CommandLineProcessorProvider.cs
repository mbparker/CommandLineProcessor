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

        public CommandLineProcessorProvider(ICommandRepositoryService commandRepository, ICommandPathCalculator commandPathCalculator)
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

            if (State == CommandLineState.WaitingForCommand)
            {
                var fullyQualifiedInput = commandPathCalculator.CalculateFullyQualifiedPath(ActiveCommand, input);
                ActiveCommand = commandRepository[fullyQualifiedInput];
            }            
        }

        public void RegisterCommands(IEnumerable<ICommand> commands)
        {
            commandRepository.Load(commands);
            State = CommandLineState.WaitingForCommand;
        }
    }
}