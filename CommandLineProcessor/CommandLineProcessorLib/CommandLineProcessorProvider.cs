namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class CommandLineProcessorProvider : ICommandLineProcessorService
    {
        private readonly ICommandRepositoryService commandRepository;

        public CommandLineProcessorProvider(ICommandRepositoryService commandRepository)
        {
            this.commandRepository = commandRepository;
        }

        public string CurrentInput { get; private set; }

        public void ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Value is required.", nameof(input));
            }

            CurrentInput = input;
        }

        public void RegisterCommands(IEnumerable<ICommand> commands)
        {
            commandRepository.Load(commands);
        }
    }
}