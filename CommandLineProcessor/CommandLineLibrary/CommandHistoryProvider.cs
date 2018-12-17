namespace CommandLineLibrary
{
    using System.Collections.Generic;

    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Contracts.Commands;
    using CommandLineLibrary.Models;

    public class CommandHistoryProvider : ICommandHistoryService
    {
        private readonly List<ICommand> history = new List<ICommand>();

        private int historyIndex = -1;

        public CommandHistoryProvider()
        {
            Settings = new CommandHistorySettings();
        }

        public CommandHistorySettings Settings { get; set; }

        public ICommand First()
        {
            historyIndex = 0;
            return FetchHistoryItem();
        }

        public ICommand Last()
        {
            historyIndex = history.Count - 1;
            return FetchHistoryItem();
        }

        public ICommand Next()
        {
            if (historyIndex < 0)
            {
                historyIndex = history.Count - 1;
                return null;
            }

            if (historyIndex < history.Count - 1)
            {
                historyIndex++;
                return FetchHistoryItem();
            }

            return null;
        }

        public void NotifyCommandExecuting(ICommand command)
        {
            if (command != null)
            {
                if (command.Parent == null)
                {
                    if (history.Count == Settings.MaximumCommandsInHistory)
                    {
                        history.RemoveAt(0);
                    }

                    history.Add(command);
                    historyIndex = -1;
                }
            }
        }

        public ICommand Previous()
        {
            if (historyIndex < 0)
            {
                return Last();
            }

            if (historyIndex > 0)
            {
                historyIndex--;
                return FetchHistoryItem();
            }

            return null;
        }

        private ICommand FetchHistoryItem()
        {
            if (history.Count > historyIndex && historyIndex >= 0)
            {
                return history[historyIndex];
            }

            return null;
        }
    }
}