namespace CommandLineProcessorDemo.DemoCommands
{
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands;

    public class MathCommand
    {
        private readonly ICommandHistoryWriter commandHistoryAccess;

        public MathCommand(ICommandHistoryWriter commandHistoryAccess)
        {
            this.commandHistoryAccess = commandHistoryAccess;
        }

        public void Math_Add_Execute(ICommandContext context)
        {
            var num1 = context.DataStore.Get<double>("N1");
            var num2 = context.DataStore.Get<double>("N2");
            var result = num1 + num2;
            commandHistoryAccess.WriteLine($"{num1} + {num2} = {result}");
        }

        public void Math_ApplyInput_1(ICommandContext context, string input)
        {
            context.DataStore.Set("N1", double.Parse(input));
        }

        public void Math_ApplyInput_2(ICommandContext context, string input)
        {
            context.DataStore.Set("N2", double.Parse(input));
        }

        public ICommand Math_GetDefaultCommand(ICommandContext context, IEnumerable<ICommand> commands)
        {
            return commands.Single(x => x.PrimarySelector.ToUpper() == "MULT");
        }

        public string Math_GetPromptText_1(ICommandContext context)
        {
            return "Enter first number";
        }

        public string Math_GetPromptText_2(ICommandContext context)
        {
            return "Enter second number";
        }

        public void Math_Mult_Execute(ICommandContext context)
        {
            var num1 = context.DataStore.Get<double>("N1");
            var num2 = context.DataStore.Get<double>("N2");
            var result = num1 * num2;
            commandHistoryAccess.WriteLine($"{num1} X {num2} = {result}");
        }
    }
}