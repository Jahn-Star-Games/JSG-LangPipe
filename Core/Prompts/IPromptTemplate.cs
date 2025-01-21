using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace JahnStarGames.Langpipe
{
    public interface IPromptTemplate
    {
        public string Format();
    }
    [Serializable]
    public class FormatPromptValue
    {
        public string key;
        public string value;
        public FormatPromptValue() { }
        public FormatPromptValue(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
    public class PromptTemplateException : Exception
    {
        public PromptTemplateException(string message) : base(message) { }
    }
}
