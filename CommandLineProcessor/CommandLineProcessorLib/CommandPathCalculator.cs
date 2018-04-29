namespace CommandLineProcessorLib
{
    using System.Linq;

    using CommandLineProcessorContracts;

    public class CommandPathCalculator : ICommandPathCalculator
    {
        public string CalculateFullyQualifiedPath(ICommand activeCommand, string input)
        {
            var fullyQualifiedInput = input;
            if (activeCommand != null)
            {
                fullyQualifiedInput = activeCommand.Selectors.First() + "|" + fullyQualifiedInput;
                if (!string.IsNullOrWhiteSpace(activeCommand.Path))
                {
                    fullyQualifiedInput = activeCommand.Path + "|" + fullyQualifiedInput;
                }
            }

            return fullyQualifiedInput;
        }
    }
}