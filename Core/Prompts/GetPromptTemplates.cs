namespace JahnStarGames.Langpipe
{
    public static class GetPromptTemplates
    {
            public static string GetSummaryPrompt(string summary, string new_lines)
            {
                string template = @"
                Progressively summarize the lines of conversation provided, adding onto the previous summary returning a new summary.

                EXAMPLE
                Current summary:
                The human asks what the AI thinks of artificial intelligence.The AI thinks artificial intelligence is a force for good.

                New lines of conversation:
                Human: Why do you think artificial intelligence is a force for good?
                AI: Because artificial intelligence will help humans reach their full potential.

                New summary:
                The human asks what the AI thinks of artificial intelligence. The AI thinks artificial intelligence is a force for good because it will help humans reach their full potential.
                END OF EXAMPLE

                Current summary:
                {summary}

                New lines of conversation:
                {new_lines}

                New summary:";
                var promptTemplate = new PromptTemplate(template, new() { new("summary", summary), new("new_lines", new_lines) });
                return promptTemplate.Format();
            }
    }
}
