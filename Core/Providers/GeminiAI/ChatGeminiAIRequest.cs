using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JahnStarGames.Langpipe
{
    public class ChatGeminiAIRequest : IChatRequest
    {
        public string Model { get; set; } = "gemini-1.0-pro-vision";
        public float Temperature { get; set; } = 1;
        // 
        public List<Message> Messages { get; set; } = new List<Message>();
        public List<Function> Functions { get; set; }  = new List<Function>();
        // API Ref: https://cloud.google.com/vertex-ai/generative-ai/docs/model-reference/gemini
        public JObject Wrap()
        {
            var contents = new JArray(Messages.Select(CreateContent));
            return new JObject { { "contents", contents } };
        }

        private JObject CreateContent(Message message)
        {
            var parts = new JArray(message.Contents.Select(CreatePart));
            var content = new JObject
            {
                { "role", message.Role == Role.assistant ? "model" : "user" },
                { "parts", parts }
            };
            if (message.Role == Role.system) parts.Add(new JObject { { "text", "Yes, this is undoubtedly true." } });
            return content;
        }

        private JObject CreatePart(string content)
        {
            if (ContentDeclaration.GetMimeType(content) == "text/plain") return new JObject { { "text", content } };
            return new JObject
            {
                { "text", (string)null },
                { "inlineData", new JObject
                    {
                        { "mimeType", ContentDeclaration.GetMimeType(content) },
                        { "data", content.Split(",")[1] }
                    }
                }
            };
        }
    }
}