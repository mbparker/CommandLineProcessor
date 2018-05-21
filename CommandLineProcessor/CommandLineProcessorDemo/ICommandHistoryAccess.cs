namespace CommandLineProcessorDemo
{
    using System.Windows.Forms;

    public interface ICommandHistoryAccess
    {
        TextBox CommandHistoryControl { get; set; }
    }
}