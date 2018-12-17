namespace CommandLineLibrary
{
    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Contracts.Commands;

    public class CommandPathCalculator : ICommandPathCalculator
    {
        public string CalculateFullyQualifiedPath(ICommand activeCommand, string input)
        {
            var fullyQualifiedInput = input;
            if (activeCommand != null)
            {
                fullyQualifiedInput = activeCommand.PrimarySelector + Constants.InternalTokens.SelectorSeperator + fullyQualifiedInput;
                if (!string.IsNullOrWhiteSpace(activeCommand.Path))
                {
                    fullyQualifiedInput = activeCommand.Path + Constants.InternalTokens.SelectorSeperator + fullyQualifiedInput;
                }
            }

            return fullyQualifiedInput;
        }
    }
}