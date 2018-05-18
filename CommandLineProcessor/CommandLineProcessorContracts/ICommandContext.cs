﻿namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public interface ICommandContext
    {
        ICommandDataStore DataStore { get; }

        T GetService<T>();
    }
}