namespace CommandLineLibrary.Models.Exceptions
{
    using System;

    public class DuplicateCommandSelectorException : Exception
    {
        public DuplicateCommandSelectorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DuplicateCommandSelectorException(string message)
            : base(message)
        {
        }
    }
}