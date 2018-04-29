namespace CommandLineProcessorDemo
{
    using System;
    using System.Windows.Forms;
   
    using CommandLineProcessorCommon.Ioc.Windsor;

    static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            IocContainerHolder.CreateContainer();
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                ContainerRegistration.RegisterServices(IocContainerHolder.Container);
                Application.Run(new FormMain());
            }
            finally
            {
                IocContainerHolder.DisposeContainer();
            }
        }
    }
}