using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JahnStarGames.Langpipe
{
    public interface IChatRequest
    {
        public string Model { get; set; }
        public float Temperature { get; set; }
        public List<Message> Messages { get; set; }
        public List<Function> Functions { get; set; }
        public JObject Wrap();
    }
    #region Chat Request
    [Serializable]
    public class Message
    {
        public Role Role = Role.user;
        public string Content;
        public string[] Details;
        public string[] Contents => new string[] { Content }.Concat(Details).ToArray();
        public Message() { }

        public Message(Role role, params string[] content)
        {
            Role = role;
            Content = content[0];
            Details = content.Skip(1).ToArray();
        }
    }

    public enum Role
    {
        system,
        user,
        assistant,
        function
    }

    public static class ContentDeclaration
    {
        private const string ImageDeclarationPrefix = "data:image/png;base64,";
        private const string AudioDeclarationPrefix = "data:audio/wav;base64,";
        private const string VideoDeclarationPrefix = "data:video/mp4;base64,";
        private const string FileDeclarationPrefix = "data:application/pdf;base64,";
        //
        public static string ImageFromBase64(string base64) => base64.StartsWith(ImageDeclarationPrefix) ? base64 : $"{ImageDeclarationPrefix}{base64}";
        public static string AudioFromBase64(string base64) => base64.StartsWith(AudioDeclarationPrefix) ? base64 : $"{AudioDeclarationPrefix}{base64}";
        public static string VideoFromBase64(string base64) => base64.StartsWith(VideoDeclarationPrefix) ? base64 : $"{VideoDeclarationPrefix}{base64}";
        public static string FileFromBase64(string base64) => base64.StartsWith(FileDeclarationPrefix) ? base64 : $"{FileDeclarationPrefix}{base64}";
        //
        /// <summary>
        /// Detects the content type of the given content
        /// </summary>
        /// <param name="content"></param>
        public static string GetMimeType(string content)
        {
            if (content.StartsWith(ImageDeclarationPrefix)) return "image/png";
            if (content.StartsWith(AudioDeclarationPrefix)) return "audio/wav";
            if (content.StartsWith(VideoDeclarationPrefix)) return "video/mp4";
            if (content.StartsWith(FileDeclarationPrefix)) return "application/pdf";
            return "text/plain";
        }
    }
    
    #endregion
    #region  Function Declaration
    public class Function
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Parameters Parameters { get; set; }
    }

    public class Parameters
    {
        public string Type { get; set; }
        public Dictionary<string, FunctionParameter> Properties { get; set; }
        public List<string> Required { get; set; }
    }

    public class FunctionParameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ObjectDetail Parameters { get; set; }
        public object Type { get; internal set; }
    }

    public class ObjectDetail
    {
        public string Type { get; set; }        
        public Dictionary<string, ParameterDetail> Properties { get; set; }
        public List<string> Required { get; set; }
    }

    public class ParameterDetail
    {
        public string Type { get; set; }
        public string Description { get; set; }
    }
    #endregion
}
