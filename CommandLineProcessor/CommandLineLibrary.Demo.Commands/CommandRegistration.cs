namespace CommandLineLibrary.Demo.Commands
{
    using System.Collections.Generic;

    using CommandLineLibrary.Contracts.Commands;
    using CommandLineLibrary.Contracts.Commands.Registration;

    public static class CommandRegistration
    {
        public static IEnumerable<ICommand> Register(IRootCommandRegistration registration)
        {
            registration.RegisterInputCommand<EchoCommand, CommandDescriptors>(
                    x => x.Echo_Descriptor,
                    x => x.Echo_GetPromptText(null),
                    x => x.Echo_ApplyInputMethod(null, null),
                    x => x.Echo_GetDefault(null))
                .SetChildToExecutableCommand<EchoCommand>(x => x.Echo_Execute(null));

            registration.RegisterInputCommand<ExitCommand, CommandDescriptors>(
                x => x.Exit_Descriptor,
                x => x.Exit_GetPromptText(null),
                x => x.Exit_ApplyInputMethod(null, null),
                x => x.Exit_GetDefault(null));

            var mathCommand = registration.RegisterContainerCommand<MathCommand, CommandDescriptors>(
                x => x.Math_Descriptor,
                x => x.Math_GetDefaultCommand(null, null));

            mathCommand.AddInputCommand<MathCommand, CommandDescriptors>(
                    x => x.Math_Add_Descriptor,
                    x => x.Math_GetPromptText_1(null),
                    x => x.Math_ApplyInput_1(null, null))
                .SetChildToInputCommand<MathCommand>(
                    x => x.Math_GetPromptText_2(null),
                    x => x.Math_ApplyInput_2(null, null))
                .SetChildToExecutableCommand<MathCommand>(x => x.Math_Add_Execute(null));

            mathCommand.AddInputCommand<MathCommand, CommandDescriptors>(
                    x => x.Math_Mult_Descriptor,
                    x => x.Math_GetPromptText_1(null),
                    x => x.Math_ApplyInput_1(null, null))
                .SetChildToInputCommand<MathCommand>(
                    x => x.Math_GetPromptText_2(null),
                    x => x.Math_ApplyInput_2(null, null))
                .SetChildToExecutableCommand<MathCommand>(x => x.Math_Mult_Execute(null));

            var configCommand = registration.RegisterContainerCommand<ConfigCommand, CommandDescriptors>(
                x => x.Config_Descriptor,
                x => x.Config_GetDefaultCommand(null, null));

            configCommand.AddInputCommand<ConfigCommand, CommandDescriptors>(
                x => x.Config_AutomaticHelp_Descriptor,
                x => x.Config_AutomaticHelp_GetPromptText(null),
                x => x.Config_AutomaticHelp_ApplyInputMethod(null, null),
                x => x.Config_AutomaticHelp_GetDefault(null));

            configCommand.AddInputCommand<ConfigCommand, CommandDescriptors>(
                x => x.Config_OutputDiagnostics_Descriptor,
                x => x.Config_OutputDiagnostics_GetPromptText(null),
                x => x.Config_OutputDiagnostics_ApplyInputMethod(null, null),
                x => x.Config_OutputDiagnostics_GetDefault(null));

            configCommand.AddInputCommand<ConfigCommand, CommandDescriptors>(
                x => x.Config_OutputErrors_Descriptor,
                x => x.Config_OutputErrors_GetPromptText(null),
                x => x.Config_OutputErrors_ApplyInputMethod(null, null),
                x => x.Config_OutputErrors_GetDefault(null));

            return registration.RegisteredCommands;
        }
    }
}