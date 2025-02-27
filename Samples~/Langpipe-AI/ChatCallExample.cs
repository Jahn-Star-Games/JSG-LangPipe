using System.Collections.Generic;
using UnityEngine;
using JahnStarGames.Langpipe;

public class ChatCallExample : MonoBehaviour
{
    public List<Message> messages = new() { new Message(Role.system, "You are a helpful AI assistant"), new Message(Role.user, "What is the capital of Turkiye?") };
    [Space]
    public string result;
    private async void Start()
    {
        var model = new ChatOpenAIModel("sk-your-open-ai-api-key", new ChatOpenAIRequest { Model = "gpt-3.5-turbo", Temperature = 0.5f }, verbose: new(true));
        // Create a chat history
        result = await model.CallAsync(messages);
    }
}
