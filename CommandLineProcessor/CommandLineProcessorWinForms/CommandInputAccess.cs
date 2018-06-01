namespace CommandLineProcessorWinForms
{
    using System.Windows.Forms;

    public class CommandInputAccess : ICommandInputControlAccess
    {
        private TextBox inputControl;

        public int SelectionStart
        {
            get => inputControl.SelectionStart;
            set => inputControl.SelectionStart = value;
        }

        public string Text
        {
            get => inputControl.Text;
            set => inputControl.Text = value;
        }

        TextBox ICommandInputControlAccess.InputControl
        {
            get => inputControl;
            set => inputControl = value;
        }

        public void Focus()
        {
            inputControl.Focus();
        }
    }
}