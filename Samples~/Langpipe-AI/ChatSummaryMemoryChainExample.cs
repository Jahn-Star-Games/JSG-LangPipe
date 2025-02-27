using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using JahnStarGames.Langpipe;
using JahnStarGames.Langpipe.Providers.GoogleApis;

public class ChatSummaryMemoryChainExample : MonoBehaviour
{
    public bool newChat = true;
    public string systemPromptTemplate = "Your name is {npc_name}. You are my best friend. And I am {user_name}.";
    public List<FormatPromptValue> systemPromptFormat = new() { new("npc_name", "Jessica"), new("user_name", "John") };
    [Space]
    public string userPrefix = "John";
    public string aiPrefix = "Jessica";
    public string userPrompt = "What's my name?";
    [Space]
    public List<Message> messages = new();
    [TextArea(3, 10)]
    public string result;
    public Pipeline pipeline;
    //
    // With Chain
    IChatModel model;
    private async Task Start()
    {
        // Create a model
        // model = new ChatOpenAIModel("sk-your-open-ai-api-key", new ChatOpenAIRequest { Model = "gpt-3.5-turbo", Temperature = 0.5f });
        // model = new ChatGeminiAIModel("your-gemini-ai-api-key", new ChatGeminiAIRequest { Model = "gemini-1.0-pro-vision", Temperature = 0.5f }, verbose: new(true));

        // VERTEX AI setup
        string vertex_ai_access_token = await GoogleQauth.GetAccessTokenFromJSONKeyAsync("Assets/GoogleKey.json");
        model = new ChatGeminiAIModel(vertex_ai_access_token, "your-project-name", new ChatGeminiAIRequest { Model = "gemini-1.0-pro-vision", Temperature = 0.5f }, verbose: new(true));
        // Create a buffer memory
        var memory = new SummaryMemory((model, (userPrefix, aiPrefix), 200), "history");

        pipeline = new(model, memory, new(true));

        if (newChat)
        {
            pipeline.AddSystemMessage(systemPromptTemplate, systemPromptFormat)
                .AddUserMessage(userPrompt)
                .SaveMemory();
        }
        else pipeline.LoadMemory().AddUserMessage(userPrompt);

        result = await pipeline.RunAsync();
        //
        messages = pipeline.GetMessages();
    }
    [ContextMenu("Auth Conversation")]
    public async void AutoConversation()
    {
        userPrompt = await model.CallAsync($"[Don't use prefix, be yourself. Don't be obsessive, be natural.] You are {userPrefix}, answer as {userPrefix}. You are having conversation with {aiPrefix}. \nChat: {string.Join("\n", messages.Select(m => (m.Role == Role.user ? userPrefix : aiPrefix) + ": " + m.Content))}");
        await Task.Delay(1000);
        await ConversationLoop();
    }

    [ContextMenu("Continue")]
    public async Task ConversationLoop()
    {
        pipeline.AddUserMessage(userPrompt)
        .SaveMemory();
        result = await pipeline.RunAsync();
        //
        messages = pipeline.GetMessages();
    }
}