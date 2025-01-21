using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JahnStarGames.Langpipe
{
    public class ChatGeminiAIResponse : IChatResponse
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
            var candidatesArray = response["candidates"] as JArray;
            foreach (var candidate in candidatesArray)
            {
                var candidateJson = candidate as JObject;
                var content = candidateJson["content"] as JObject;
                var role = content["role"].ToString();
                if (role == "model") role = "assistant";
                var partsArray = content["parts"] as JArray;
                foreach (var part in partsArray)
                {
                    var partJson = part as JObject;
                    var text = partJson["text"].ToString();
                    var messageJson = new Message
                    {
                        Role = (Role)System.Enum.Parse(typeof(Role), role),
                        Content = text
                    };
                    Choices.Add(new Choice { Message = messageJson });
                }
            }
            TotalTokens = int.Parse(response["usageMetadata"]["totalTokenCount"].ToString());
            return Choices[0].Message.Content; 
        }
    }
}