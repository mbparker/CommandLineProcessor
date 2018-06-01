namespace CommandLineProcessorEntity
{
    public class CommandHistorySettings
    {
        public string BeginningOfHistoryText { get; set; } = "*At beginning of command history*";

        public string EndOfHistoryText { get; set; } = "*At end of command history*";

        public int MaximumCommandsInHistory { get; set; } = 100;
    }
}