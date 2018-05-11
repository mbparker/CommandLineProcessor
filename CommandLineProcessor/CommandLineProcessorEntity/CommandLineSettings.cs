namespace CommandLineProcessorEntity
{
    public class CommandLineSettings
    {
        public string CancelToken { get; set; } = "^C";

        public string SuspendActiveCommandToken { get; set; } = "`";
    }
}