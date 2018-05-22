namespace CommandLineProcessorDemo
{
    using System;
    using System.Windows.Forms;

    public class CommandHistoryAccess : ICommandHistoryAccess, ICommandHistoryControlAccess
    {
        private TextBox historyControl;

        TextBox ICommandHistoryControlAccess.HistoryControl
        {
            get => historyControl;
            set => historyControl = value;
        }

        public void WriteLine(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                historyControl?.AppendText($"{text}{Environment.NewLine}");
            }
        }
    }
}