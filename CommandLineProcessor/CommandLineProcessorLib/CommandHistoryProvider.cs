namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class CommandHistoryProvider : ICommandHistoryService
    {
        private readonly List<ICommand> history = new List<ICommand>();

        private int historyIndex = -1;

        public event EventHandler<CommandExecutedEventArgs> CommandExecuting;

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
                    history.Add(command);
                    historyIndex = history.Count - 1;
                }

                CommandExecuting?.Invoke(this, new CommandExecutedEventArgs(command));
            }            
        }

        public ICommand Previous()
        {
            if (historyIndex > 0)
            {
                historyIndex--;
                return FetchHistoryItem();
            }

            return null;
        }

        private ICommand FetchHistoryItem()
        {
            if (history.Count > historyIndex)
            {
                return history[historyIndex];
            }

            return null;
        }
    }
}