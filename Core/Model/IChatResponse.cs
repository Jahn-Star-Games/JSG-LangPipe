using System.Collections.Generic;

namespace JahnStarGames.Langpipe
{
    public interface IChatResponse 
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public string Model { get; set; }
        public List<Choice> Choices { get; set; }
        public int TotalTokens { get; set; }
        public string Unwrap(string responseString);
    }

    public class Choice
    {
        public Message Message { get; set; }
        public FunctionCall FunctionCall { get; set; }
    }

    public class FunctionCall
    {
        public string Name { get; set; }
        public Dictionary<string, string> Arguments { get; set; }
    }
}
