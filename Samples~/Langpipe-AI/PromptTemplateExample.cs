using System;
using System.Collections.Generic;
using JahnStarGames.Langpipe;
using UnityEngine;

public class PromptTemplateExample : MonoBehaviour
{
    public string template = "What is the {part} of {country}?";
    public List<FormatPromptValue> format = new();
    [Space, TextArea(5, 10)]
    public string result;
    private void Start()
    {
        // Create a prompt object
        var prompt = new PromptTemplate(template, format);

        // Add the values to the prompt
        string formattedPrompt = prompt.Format();

        // Print the formatted prompt
        result = formattedPrompt;
    }
}