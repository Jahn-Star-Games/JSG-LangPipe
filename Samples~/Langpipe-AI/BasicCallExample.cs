using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JahnStarGames.Langpipe;
public class BasicCallExample : MonoBehaviour
{
    public string systemPrompt = "What is the capital of France?";
    [Space, TextArea(5, 10)]
    public string result;
    private async void Start() 
    {
        var model = new ChatOpenAIModel("sk-your-open-ai-api-key", new ChatOpenAIRequest { Model = "gpt-3.5-turbo", Temperature = 0.5f }, verbose: new(true));
        result = await model.CallAsync(systemPrompt);
    }
}
