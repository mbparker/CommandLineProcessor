namespace CommandLineProcessorContracts
{
    public interface IInputCommand : ICommand
    {
        string Data { get; set; }
    }
}