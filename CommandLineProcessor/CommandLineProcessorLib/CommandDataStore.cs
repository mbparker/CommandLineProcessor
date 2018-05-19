namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class CommandDataStore : ICommandDataStore
    {
        private readonly IDictionary<string, object> innerStore;

        public CommandDataStore(IDictionary<string, object> innerStore)
        {
            this.innerStore = innerStore;
        }

        public CommandDataStore()
            : this(new Dictionary<string, object>())
        {
        }

        public void Clear()
        {
            innerStore.Clear();
        }

        public T Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(
                    $"{nameof(key)} is a required argument when getting a value from the data store.",
                    nameof(key));
            }

            key = key.ToUpper();
            if (innerStore.ContainsKey(key))
            {
                return (T)innerStore[key];
            }

            return default(T);
        }

        public void Set<T>(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(
                    $"{nameof(key)} is a required argument when setting a value to the data store.",
                    nameof(key));
            }

            key = key.ToUpper();
            if (innerStore.ContainsKey(key))
            {
                innerStore[key] = value;
            }
            else
            {
                innerStore.Add(key, value);
            }
        }
    }
}