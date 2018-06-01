namespace CommandLineProcessorDemo
{
    using System;
    using System.Windows.Forms;
   
    using CommandLineProcessorCommon.Ioc.Windsor;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands.Registration;

    using CommandLineProcessorWinForms;

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
                var commandRegistration = IocContainerHolder.Container.Resolve<IRootCommandRegistration>();
                var commandInputAccess = IocContainerHolder.Container.Resolve<ICommandInputControlAccess>();
                var commandHistoryAccess = IocContainerHolder.Container.Resolve<ICommandHistoryControlAccess>();
                var helper = IocContainerHolder.Container.Resolve<ICommandLineWinFormsHelper>();
                Application.Run(new FormMain(processor, commandRegistration, commandInputAccess, commandHistoryAccess, helper));
            }
            finally
            {
                IocContainerHolder.DisposeContainer();
            }
        }
    }
}