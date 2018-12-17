namespace CommandLineLibrary.Contracts
{
    public interface ICommandHistoryWriter
    {
        void Write(string text);

        void WriteLine(string text);
    }
}