using System.Collections.Generic;
using UnityEngine;
using JahnStarGames.Langpipe;
public class PromptTemplateChainExample : MonoBehaviour
{
    public string systemPromptTemplate = "What is a {property} for a company that makes {product}?";
    public List<FormatPromptValue> systemPromptFormat = new() { new("property", "good name"), new() { key = "product", value = "{question}" } };
    [Space]
    public List<FormatPromptValue> inputFormat = new() { new FormatPromptValue("question", "laptop") };
    [Space]
    public Pipeline chain;
    public string result;
    // With Chain
    private async void Start()
    {
        // Create a model
        var model = new ChatOpenAIModel("sk-your-open-ai-api-key", new ChatOpenAIRequest { Model = "gpt-3.5-turbo", Temperature = 0.5f });
        // Create a buffer memory
        var memory = new BufferMemory("history");

        chain = new(model, memory, verbose: new(true));
        chain.AddMessage(new Message(Role.system, systemPromptTemplate), systemPromptFormat);
        result = await chain.ApplyTemplateToAll(inputFormat).RunAsync();
    }
}