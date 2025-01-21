using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace JahnStarGames.Langpipe
{
    public interface IChatModel
    {
        public HttpClient httpClient { get; set; }
        public string Endpoint { get; set; }
        public string Id { get; set; }
        public string ApiKey { get; set; }
        public IChatRequest ChatRequest { get; set; }
        public IChatResponse ChatResponse { get; set; }
        public PipelineVerbose Verbose { get; set; }
        public Task<string> CallAsync(params string[] prompt);
        public Task<string> CallAsync(List<Message> messages);
    }
}
