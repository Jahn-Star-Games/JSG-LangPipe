using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JahnStarGames.Langpipe
{
    public class SummaryMemory : BufferMemory
    {
        private string systemMessage;
        private string historySummary;
        private int tokenCount;
        //
        private readonly IChatModel model;
        private readonly int maxToken;
        private readonly string humanPrefix;
        private readonly string aiPrefix;
        public SummaryMemory((IChatModel model, (string human, string ai) prefixes, int maxToken) settings, string key, List<Message> transferMessages = null, PipelineVerbose verbose = null) : base(key, transferMessages, verbose)
        {
            this.model = settings.model;
            this.maxToken = settings.maxToken;
            this.humanPrefix = settings.prefixes.human;
            this.aiPrefix = settings.prefixes.ai;
        }
        public override async void Add(Message value)
        {
            if (string.IsNullOrEmpty(systemMessage) && messages.Count > 1 && messages.First().Role == Role.system) systemMessage = messages[0].Content;
            // Add the new message
            messages.Add(value);
            // Prune messages if they exceed the max token limit
            await PruneMessages();
        }
        
        private async Task PruneMessages()
        {
            if (messages.Count == 0) return;

            var prunedMessages = new List<Message>();
            var queue = new Queue<Message>(messages);
            queue.Dequeue(); // skip system message

            tokenCount = EstimatedTokenCounter(string.Join(' ', queue.Select(m => m.Content)));
            if (tokenCount > maxToken)
            {
                verbose?.Log("Pruning Messages: " + tokenCount + " tokens", true);
                while (tokenCount > maxToken)
                {
                    prunedMessages.Add(queue.Dequeue());
                    tokenCount = EstimatedTokenCounter(string.Join(' ', queue.Select(m => m.Content)));
                }

                historySummary = await Summarize(prunedMessages, historySummary);
                SetMessages(queue);
            }
            verbose?.Log("Current Token Count: " + tokenCount, true);
        }

        public async Task<string> Summarize(IEnumerable<Message> newMessages, string existingSummary)
        {
            string new_lines = string.Join("\n", newMessages.Select(message => $"{(message.Role == Role.assistant ? aiPrefix : humanPrefix)}: {message.Content}"));
            verbose?.Log("Summarizing: " + new_lines, true);
            var summarizePrompt = GetPromptTemplates.GetSummaryPrompt(existingSummary, new_lines);
            // Call the model to summarize the conversation
            Task.Delay(2500).Wait();
            string summarized = await model.CallAsync(summarizePrompt);
            verbose?.Log("Summarized: " + summarized);
            return summarized;
        }

        public static int EstimatedTokenCounter(string text)
        {
            string pattern = @"/“(?!“|\])*“|'(?:[st]|re|ve|m|ll|d)| ?\p{L}+| ?\p{N}+| ?[^s\p{L}\p{N}]+|\s+(?!\S)|\s+/";
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Multiline);
            return regex.Matches(text).Count;
        }

        public void SetMessages(IEnumerable<Message> messages)
        {
            messages = messages ?? throw new ArgumentNullException(nameof(messages));

            base.messages?.Clear();
            base.messages.Add(new(Role.system, systemMessage + "\nHistory Summary:\n" + historySummary + "\n"));
            base.messages.AddRange(messages);
        }
    }
}