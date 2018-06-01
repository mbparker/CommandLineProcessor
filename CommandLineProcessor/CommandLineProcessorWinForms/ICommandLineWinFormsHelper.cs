namespace CommandLineProcessorWinForms
{
    using System.Windows.Forms;

    public interface ICommandLineWinFormsHelper
    {
        bool AutomaticHelp { get; set; }

        bool OutputDiagnostics { get; set; }

        bool OutputErrors { get; set; }

        void HandleKeyDown(KeyEventArgs eventArgs);

        void UpdateCommandLine();
    }
}