namespace CommandLineLibrary.Demo
{
    using System;

    using CommandLineLibrary.Contracts;

    public class ApplicationInstance : IApplication, ICommandHistoryWriter
    {
        public void Exit()
        {
            Environment.Exit(0);
        }

        public void Write(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                Console.Write(text);
            }
        }

        public void WriteLine(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine(text);
            }
        }
    }
}