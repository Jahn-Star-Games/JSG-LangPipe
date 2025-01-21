using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JahnStarGames.Langpipe
{
    public class ChatOpenAIRequest : IChatRequest
    {
        public string Model { get; set; } = "gpt-3.5-turbo";
        public float Temperature { get; set; } = 1;
        public List<Message> Messages { get; set; } = new List<Message>();
        public List<Function> Functions { get; set; }  = new List<Function>();
        public JObject Wrap()
        {
            var model = Model;
            var temperature = Temperature;
            var messages = CreateMessages();
            if (Functions.Count > 0)
            {
                var functions = CreateFunctions();
                var function_call = "auto";
                return new JObject { { "model", model }, { "temperature", temperature }, { "messages", messages }, { "functions", functions }, { "function_call", function_call } };
            }
            else
            {
                return new JObject { { "model", model }, { "temperature", temperature }, { "messages", messages } };
            }
        }

        private JArray CreateMessages()
        {
            var messages = new JArray();
            foreach (var message in Messages)
            {
                messages.Add(new JObject
                {
                    { "role", message.Role.ToString() },
                    { "content", message.Content }
                });
            }
            return messages;
        }

        private JArray CreateFunctions()
        {
            var functions = new JArray();
            foreach (var function in Functions)
            {
                var parameters = CreateParameters(function);
                var functionObject = new JObject
                {
                    new JProperty("name", function.Name),
                    new JProperty("description", function.Description),
                    new JProperty("parameters", new JObject
                    {
                        new JProperty("type", function.Parameters.Type),
                        new JProperty("properties", parameters),
                        new JProperty("required", function.Parameters.Required)
                    })
                };
                functions.Add(functionObject);
            }
            return functions;
        }

        private JObject CreateParameters(Function function)
        {
            var parameters = new JObject();
            foreach (var property in function.Parameters.Properties)
            {
                parameters.Add(property.Key, new JObject
                {
                    { "type", property.Value?.GetType()?.Name },
                    { "description", property.Value?.Description }
                });
            }
            return parameters;
        }
    }
}