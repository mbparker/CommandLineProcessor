﻿namespace CommandLineLibrary.Models
{
    public class CommandLineSettings
    {
        public string CancelToken { get; set; } = "^C";

        public string CommandLevelIndicator { get; set; } = ">>";

        public string CommandPromptRoot { get; set; } = "Command";

        public string CommandSeparatorToken { get; set; } = "|";

        public int MaximumStackSize { get; set; } = 10;

        public string PauseForUserInputToken { get; set; } = "~";

        public string SuspendActiveCommandToken { get; set; } = "`";
    }
}