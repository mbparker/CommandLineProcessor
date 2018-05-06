namespace CommandLineProcessorDemo
{
    using System;
    using System.Windows.Forms;
   
    using CommandLineProcessorCommon.Ioc.Windsor;

    using CommandLineProcessorContracts;

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
                var processor = IocContainerHolder.Container.Resolve<ICommandLineProcessorService>();
                Application.Run(new FormMain(processor));
            }
            finally
            {
                IocContainerHolder.DisposeContainer();
            }
        }
    }
}