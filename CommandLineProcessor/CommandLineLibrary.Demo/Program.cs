namespace CommandLineLibrary.Demo
{
    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Contracts.Commands.Registration;
    using CommandLineLibrary.Demo.Commands;

    using global::Autofac;

    class Program
    {
        static void Main(string[] args)
        {
            using (var container = ContainerRegistration.BuildContainer())
            {
                var rootCommandRegistration = container.Resolve<IRootCommandRegistration>();
                var commands = CommandRegistration.Register(rootCommandRegistration);
                var cli = container.Resolve<ICommandLineInterface>();
                cli.AutomaticHelp = true;
                cli.OutputDiagnostics = true;
                cli.OutputErrors = true;
                cli.Run(commands);
            }
        }
    }
}