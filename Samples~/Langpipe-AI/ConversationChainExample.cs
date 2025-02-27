using System.Threading.Tasks;
using UnityEngine;
using JahnStarGames.Langpipe;

public class ConversationChainExample : MonoBehaviour
{
    public string systemPrompt = "You are a helpful AI assistant.";
    [Header("Input")]
    public string userPrompt  = "Hello, my name is Jim.";
    public string secondUserPrompt = "What's my name?";
    public string result;
    [Space]
    public Pipeline chain;
    // With Chain
    private async void Start()
    {
        // Create a model
        var model = new ChatOpenAIModel("sk-your-open-ai-api-key", new ChatOpenAIRequest { Model = "gpt-3.5-turbo", Temperature = 0.5f });
        // Create a buffer memory
        var memory = new BufferMemory("history");
        
        chain = new(model, memory, verbose: new(true));
        chain.AddSystemMessage(systemPrompt);
        result = await chain.AddUserMessage(userPrompt).RunAsync();
        await Task.Delay(1000);
        result = await chain.AddUserMessage(secondUserPrompt).RunAsync();
    }
}