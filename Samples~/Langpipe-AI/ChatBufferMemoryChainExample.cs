using System.Collections.Generic;
using UnityEngine;
using JahnStarGames.Langpipe;

public class ChatBufferMemoryChainExample : MonoBehaviour
{
    public bool newChat = true;
    public string systemPromptTemplate = "Your name is {npc_name}. You are my best friend. And I am {user_name}.";
    public List<FormatPromptValue> systemPromptFormat = new() { new("npc_name", "Jessica"), new("user_name", "John") };
    public string userPrompt = "Hi!";
    [TextArea(3, 10)]
    public string result;
    public Pipeline pipeline;
    //
    // With Chain
    private async void Start()
    {
        // Create a model
        var model = new ChatOpenAIModel("sk-your-open-ai-api-key", new ChatOpenAIRequest { Model = "gpt-3.5-turbo", Temperature = 0.5f });
        // Create a buffer memory
        var memory = new BufferMemory("history");

        pipeline = new(model, memory, new(true));

        if (newChat)
        {
            pipeline.AddSystemMessage(systemPromptTemplate, systemPromptFormat)
                .AddUserMessage(userPrompt)
                .SaveMemory();
        }
        else pipeline.LoadMemory().AddUserMessage(userPrompt);

        result = await pipeline.RunAsync();
    }

    [ContextMenu("Continue")]
    public async void ConversationLoop()
    {
        pipeline.AddUserMessage(userPrompt)
        .SaveMemory();
        result = await pipeline.RunAsync();
    }
}