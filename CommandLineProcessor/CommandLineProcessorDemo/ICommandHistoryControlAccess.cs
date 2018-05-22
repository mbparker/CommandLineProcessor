namespace CommandLineProcessorDemo
{
    using System.Windows.Forms;

    public interface ICommandHistoryControlAccess
    {
        TextBox HistoryControl { get; set; }
    }
}