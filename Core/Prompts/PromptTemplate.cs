using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace JahnStarGames.Langpipe
{
    [Serializable]
    public class PromptTemplate
    {
        public string Template;
        public List<FormatPromptValue> FormatPromptValues = new();
        /// <param name="template">The template string. e.g. "What is the {part} of {country}?"</param>
        /// <param name="format">A list of tuples containing the veriable name and the new value. e.g. ("country", "Turkey"), ("part", "capital").</param>
        public PromptTemplate(string template, List<FormatPromptValue> format)
        {
            Template = template;
            FormatPromptValues = format;
        }

        public string Format() => Format(FormatPromptValues);
        /// <summary>
        /// Format values the prompt with the values from a list of tuples.
        /// </summary>
        /// <returns>The formatted prompt.</returns>
        private string Format(List<FormatPromptValue> formatPromptValues)
        {
            string formatted = Template;
            foreach (var format in formatPromptValues)
            {
                string formatKeyDeclaration = "{" + format.key + "}";
                if (formatted.Contains(formatKeyDeclaration)) formatted = formatted.Replace(formatKeyDeclaration, format.value);
                else throw new PromptTemplateException("The template does not contain the key: " + format.key);
            }
            return formatted;
        }
        /// <summary>
        /// Format values the prompt with the values from a JSON array.
        /// </summary>
        /// <param name="jsonArray">A JSON array containing the values for the veriables. e.g. "[{ \"country\": \"Turkey\" }, { \"part\": \"capital\" }]".</param>
        public List<FormatPromptValue> ParseFormatValues(string jsonArray) => JsonConvert.DeserializeObject<List<FormatPromptValue>>(jsonArray);
    }
}
