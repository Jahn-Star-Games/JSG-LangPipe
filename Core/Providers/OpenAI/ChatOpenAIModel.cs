namespace JahnStarGames.Langpipe
{
    public class ChatOpenAIModel : ChatModel
    {
        public ChatOpenAIModel(string apiKey, IChatRequest requestBody = null, PipelineVerbose verbose = null)
        {
            ApiKey = apiKey;
            ChatRequest = requestBody ?? new ChatOpenAIRequest();
            ChatResponse = new ChatOpenAIResponse();
            Verbose = verbose;
            //
            Endpoint = "https://api.openai.com/v1/chat/completions";
        }
    }
}