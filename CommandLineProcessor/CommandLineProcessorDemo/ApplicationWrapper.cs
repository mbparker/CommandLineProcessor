namespace CommandLineProcessorDemo
{
    using System.Windows.Forms;

    using CommandLineProcessorContracts;

    public class ApplicationWrapper : IApplication
    {
        public void Exit()
        {
            Application.Exit();
        }
    }
}