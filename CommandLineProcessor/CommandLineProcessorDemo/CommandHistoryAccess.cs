namespace CommandLineProcessorDemo
{
    using System;
    using System.Windows.Forms;

    using CommandLineProcessorContracts;

    public class CommandHistoryAccess : ICommandHistoryWriter, ICommandHistoryControlAccess
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