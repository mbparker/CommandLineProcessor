﻿namespace CommandLineProcessorContracts
{
    using System;

    public class CommandLineErrorEventArgs : EventArgs
    {
        public CommandLineErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}