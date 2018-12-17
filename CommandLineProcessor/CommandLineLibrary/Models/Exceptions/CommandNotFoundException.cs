namespace CommandLineLibrary.Models.Exceptions
{
    using System;

    public class CommandNotFoundException : Exception
    {
        public CommandNotFoundException(string message, string selector, Exception innerException)
            : base(message, innerException)
        {
            Selector = selector;
        }

        public CommandNotFoundException(string message, string selector)
            : this(message, selector, null)
        {
        }

        public string Selector { get; }
    }
}