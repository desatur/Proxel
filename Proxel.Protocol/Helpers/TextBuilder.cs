using System;
using System.Collections.Generic;

namespace Proxel.Protocol.Helpers
{
    public class TextBuilder : IDisposable
    {
        public string Text { get; set; } = string.Empty;
        public bool Bold { get; set; } = false;
        public bool Italic { get; set; } = false;
        public bool Underlined { get; set; } = false;
        public bool Strikethrough { get; set; } = false;
        public bool Obfuscated { get; set; } = false;

        private string finalJson;

        private void FormatJson()
        {
            var jsonParts = new List<string>();
            if (!string.IsNullOrEmpty(Text))
            {
                jsonParts.Add($"\"text\": \"{Text}\"");
            }
            if (Bold)
            {
                jsonParts.Add($"\"bold\": {Bold.ToString().ToLower()}");
            }
            if (Italic)
            {
                jsonParts.Add($"\"italic\": {Italic.ToString().ToLower()}");
            }
            if (Underlined)
            {
                jsonParts.Add($"\"underlined\": {Underlined.ToString().ToLower()}");
            }
            if (Strikethrough)
            {
                jsonParts.Add($"\"strikethrough\": {Strikethrough.ToString().ToLower()}");
            }
            if (Obfuscated)
            {
                jsonParts.Add($"\"obfuscated\": {Obfuscated.ToString().ToLower()}");
            }
            finalJson = "{" + string.Join(",", jsonParts) + "}";
        }

        public string GetFinalJson(bool format = true)
        {
            if (format) FormatJson();
            return finalJson;
        }

        public void Dispose()
        {
            Text = null;
            finalJson = null;
            GC.SuppressFinalize(this);
        }
    }
}
