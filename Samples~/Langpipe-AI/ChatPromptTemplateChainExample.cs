using System.Collections.Generic;
using UnityEngine;
using JahnStarGames.Langpipe;
public class ChatPromptTemplateChainExample : MonoBehaviour
{
    public string systemPromptTemplate = "You are a translator who translates {input_language} to {output_language}.";
    public List<FormatPromptValue> systemPromptFormat = new() { new("input_language", "English"), new("output_language", "Turkish") };
    public string userPromptTemplate = "Translate the given text: {text}";
    public List<FormatPromptValue> userPromptFormat = new() { new("text", "{question}") };
    [Space]
    public List<FormatPromptValue> inputFormat = new() { new("question", "I love programming...") };
    [TextArea(3, 10)]
    public string result;
    public Pipeline pipeline;
    //
    private ChatOpenAIModel model;
    // With Chain
    private async void Start()
    {
        // Create a model
        model = new ChatOpenAIModel("sk-your-open-ai-api-key", new ChatOpenAIRequest { Model = "gpt-3.5-turbo", Temperature = 0.5f });
        // Create a buffer memory
        var memory = new BufferMemory("history");

        pipeline = new(model, memory, verbose: new(true));
        pipeline.AddSystemMessage(systemPromptTemplate, systemPromptFormat)
            .AddUserMessage(userPromptTemplate, userPromptFormat)
            .ApplyTemplateToAll(inputFormat);
        result = await pipeline.RunAsync();
    }

    [ContextMenu("Continue")]
    public async void ConversationLoop()
    {
        pipeline.AddUserMessage(userPromptTemplate, userPromptFormat).ApplyTemplateToAll(inputFormat);
        result = await pipeline.RunAsync();
    }
}