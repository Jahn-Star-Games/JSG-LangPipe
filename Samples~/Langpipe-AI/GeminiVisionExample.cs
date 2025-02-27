using UnityEngine;
using JahnStarGames.Langpipe;

public class GeminiVisionExample : MonoBehaviour 
{
    [SerializeField] private string token = "";
    // 2x2 grid example image in base64
    public string png_base64 = "iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAKElEQVQ4jWNgYGD4Twzu6FhFFGYYNXDUwGFpIAk2E4dHDRw1cDgaCAASFOffhEIO3gAAAABJRU5ErkJggg==";
    public string systemPrompt = "Do you see any picture if you do, what do you see?";
    [Space, TextArea(5, 10)]
    public string result;

    async void Start()
    {
        var model = new ChatGeminiAIModel(token, "your-project-id", new ChatGeminiAIRequest { Model = "gemini-1.0-pro-vision", Temperature = 0.5f }, verbose: new(true));
        result = await model.CallAsync(systemPrompt, ContentDeclaration.ImageFromBase64(png_base64));
    }
}
