using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace JahnStarGames.Langpipe
{
    public class Pipeline
    {
        private readonly IChatModel Model;
        private readonly IBaseMemory Memory;
        private readonly PipelineVerbose Verbose;

        public Pipeline(IChatModel model = null, IBaseMemory memory = null, PipelineVerbose verbose = null)
        {
            if (model != null)
            {
                Model = model;
                Model.Verbose = verbose;
            }
            if (memory != null)
            {
                Memory = memory;
                Memory.verbose = verbose;
            }
            Verbose = verbose;
        }

        public Pipeline LoadMemory()
        {
            if (Memory == null) throw new BaseMemoryException("Memory is not set. Please set in the constructor.");
            bool success = Memory.LoadFromCache();
            Verbose?.Log(success ? "Memory Loaded: " + Memory.messages.Count + " messages" : "Memory Not Loaded: " + Memory.key);
            return this;
        }

        public Pipeline SaveMemory()
        {
            if (Memory == null) throw new BaseMemoryException("Memory is not set. Please set in the constructor.");
            bool success = Memory.SaveToCache();
            Verbose?.Log(success ? "Memory Saved: " + Memory.messages.Count + " messages" : "Memory Not Saved: " + Memory.key);
            return this;
        }

        public List<Message> GetMessages() => Memory.messages;

        public Pipeline AddMessage(Message message, List<FormatPromptValue> format)
        {
            if (Memory == null) throw new BaseMemoryException("Memory is not set. Please set in the constructor.");
            if (format != null)
            {
                string content = message.Content;
                var messagePromptTemplate = new PromptTemplate(content, format);
                content = messagePromptTemplate.Format();
                Verbose.Log($"Format Applying to Message:\n{string.Join(',', format.Select(f => $" {f.key}>\"{f.value}\""))}", true);
                message.Content = content;
            }
            Verbose?.Log("Memory Added: 1 message\n{ " + message.Role.ToString() + ": \"" + message.Content + "\" }");
            Memory.Add(message);
            return this;
        }

        public Pipeline AddSystemMessage(string template, List<FormatPromptValue> format = null) => AddMessage(new Message(Role.system, template), format);
        public Pipeline AddUserMessage(string template, List<FormatPromptValue> format = null) => AddMessage(new Message(Role.user, template), format);

        public Pipeline ApplyTemplateToAll(List<FormatPromptValue> format)
        {
            Verbose.Log($"Format Applying:\n{string.Join(',', format.Select(f => $" {f.key}>\"{f.value}\""))}", true);
            foreach (var message in Memory) 
            {
                string content = message.Content;
                var messagePromptTemplate = new PromptTemplate(content, format);
                content = messagePromptTemplate.Format();
                if (content != message.Content) Verbose.Log("Format Applied: " + content, true);
                message.Content = content;
            }
            return this;
        }

        public async Task<string> RunAsync()
        {
            if (Model == null) throw new BaseMemoryException("Model is not set. Please set in the constructor.");
            Verbose?.Log("Chain Running: " + Memory.Count() + " messages");
            string result = await Model.CallAsync(Memory.messages);
            AddMessage(new Message(Role.assistant, result), null);
            Verbose?.Log("Chain Result: " + result);
            return result;
        }
    }
}