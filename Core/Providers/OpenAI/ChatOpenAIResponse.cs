using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JahnStarGames.Langpipe
{
    public class ChatOpenAIResponse : IChatResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public string Model { get; set; }
        public List<Choice> Choices { get; set; }
        public int TotalTokens { get; set; }

        public string Unwrap(string responseString)
        {
            Choices = new List<Choice>();
            var response = JObject.Parse(responseString);
            Id = response["id"].ToString();
            Object = response["object"].ToString();
            Created = long.Parse(response["created"].ToString());
            Model = response["model"].ToString();
            TotalTokens = int.Parse(response["usage"]["total_tokens"].ToString());
            var choicesArray = response["choices"] as JArray;
            foreach (var choice in choicesArray)
            {
                var choiceJson = choice as JObject;
                var message = choiceJson["message"] as JObject;
                var messageRole = message["role"].ToString();
                var messageContent = message["content"].ToString();
                var messageJson = new Message
                {
                    Role = (Role)System.Enum.Parse(typeof(Role), messageRole),
                    Content = messageContent
                };
                // Check if the response is a function call
                if (message["function_call"] != null)
                {
                    var functionCall = message["function_call"] as JObject;
                    var functionName = functionCall["name"].ToString();
                    var functionArguments = functionCall["arguments"].ToString();
                    var functionArgumentsJson = new Dictionary<string, string>();
                    foreach (var argument in JObject.Parse(functionArguments)) functionArgumentsJson.Add(argument.Key, argument.Value.ToString());
                    var functionCallJson = new FunctionCall
                    {
                        Name = functionName,
                        Arguments = functionArgumentsJson
                    };
                    Choices.Add(new Choice
                    {
                        Message = messageJson,
                        FunctionCall = functionCallJson
                    });
                }
                else Choices.Add(new Choice { Message = messageJson });
            }
            return Choices[0].Message.Content;
        }
    }
}