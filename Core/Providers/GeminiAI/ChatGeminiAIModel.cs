using System.Collections.Generic;
using System.Threading.Tasks;

namespace JahnStarGames.Langpipe
{
    public class ChatGeminiAIModel : ChatModel
    {
        /// <summary>
        /// Constructor for the VertexAI model
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="projectID"></param>
        /// <param name="requestBody"></param>
        /// <param name="verbose"></param>
        public ChatGeminiAIModel(string apiKey, string projectID,  IChatRequest requestBody = null, PipelineVerbose verbose = null)
        {
            ApiKey = apiKey;
            ChatRequest = requestBody ?? new ChatGeminiAIRequest();
            ChatResponse = new ChatGeminiAIResponse();
            Verbose = verbose;
            //
            string query = "generateContent";
            // for other models, look at: https://cloud.google.com/vertex-ai/generative-ai/docs/learn/models
            Endpoint = $"https://us-central1-aiplatform.googleapis.com/v1/projects/{projectID}/locations/us-central1/publishers/google/models/{ChatRequest.Model}:{query}";
        }
        /// <summary>
        /// Constructor for the GeminiAI model
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="endpoint"></param>
        /// <param name="requestBody"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public ChatGeminiAIModel(string apiKey, IChatRequest requestBody = null, PipelineVerbose verbose = null)
        {
            ChatRequest = requestBody ?? new ChatGeminiAIRequest();
            ChatResponse = new ChatGeminiAIResponse();
            Verbose = verbose;
            //
            string query = "generateContent";
            Endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{ChatRequest.Model}:{query}?key={apiKey}";
        }
        public override async Task<string> CallAsync(params string[] prompt)
        {
            var messages = new List<Message>
            {
                new(Role.user, prompt)
            };
            return await CallAsync(messages);
        }
    }
}