using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JahnStarGames.Langpipe
{
    public abstract class ChatModel : IChatModel
    {
        public HttpClient httpClient { get; set; }
        public string Endpoint { get; set; }
        public string Id { get; set; }
        public string ApiKey { get; set; }
        public IChatRequest ChatRequest { get; set; }
        public IChatResponse ChatResponse { get; set; }
        public PipelineVerbose Verbose { get; set; }
        //
        public virtual async Task<string> CallAsync(params string[] prompt)
        {
            var messages = new List<Message>
            {
                new(Role.system, prompt)
            };
            return await CallAsync(messages);
        }
        public virtual async Task<string> CallAsync(List<Message> messages)
        {
            httpClient ??= new HttpClient();
            ChatRequest.Messages = new List<Message>(messages);
            
            var requestBody = ChatRequest.Wrap();

            string requestText = JsonConvert.SerializeObject(requestBody);
            Verbose?.Log("Request: " + requestText);

            var content = new StringContent(requestText, System.Text.Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiKey);

            var response = await httpClient.PostAsync(Endpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();

            try 
            { 
                if (string.IsNullOrWhiteSpace(responseString)) throw new HttpRequestException("Network request failed!"); 
                response.EnsureSuccessStatusCode();
                Verbose?.Log("Response: " + responseString);

                var responseContent = ChatResponse.Unwrap(responseString);
                return responseContent;
            }
            catch (HttpRequestException e) 
            {
                Verbose?.LogError(e.Message + "\nResponse: " + responseString);
                return null;
            }
        }
    }
}
