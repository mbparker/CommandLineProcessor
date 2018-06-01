namespace CommandLineProcessorWinForms
{
    using System.Windows.Forms;

    public interface ICommandInputControlAccess
    {
        TextBox InputControl { get; set; }

        int SelectionStart { get; set; }

        string Text { get; set; }

        void Focus();
    }
}