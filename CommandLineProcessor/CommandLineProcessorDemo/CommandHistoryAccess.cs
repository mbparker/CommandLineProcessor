namespace CommandLineProcessorDemo
{
    using System.Windows.Forms;

    public class CommandHistoryAccess : ICommandHistoryAccess
    {
        public TextBox CommandHistoryControl { get; set; }
    }
}