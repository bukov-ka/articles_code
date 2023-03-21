using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;

public class ChatGPTResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("object")]
    public string ObjectType { get; set; } // Renamed to avoid error

    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("usage")]
    public Usage Usage { get; set; }

    [JsonPropertyName("choices")]
    public List<Choice> Choices { get; set; }
}

public class Usage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}

public class Choice
{
    [JsonPropertyName("message")]
    public Message Message { get; set; }

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }
}

public class Message
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }
}


public class ChatGPTMessage
{
    public string role { get; set; }
    public string content { get; set; }
}

public class ChatGPTCompletionData
{
    public string model { get; set; }
    public List<ChatGPTMessage> messages { get; set; }
}

namespace Translator
{
    public class ChatGptTranslator
    {
        private readonly string _apiKey;


        public ChatGptTranslator(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task TranslateAsync(string inputFile, string outputFile)
        {
            var inputText = await File.ReadAllTextAsync(inputFile);
            var outputText = await TranslateTextAsync(inputText);

            await File.WriteAllTextAsync(outputFile, outputText);
        }

        public async Task<string> TranslateTextAsync(string textToTranslate)
        {
            if (string.IsNullOrWhiteSpace(textToTranslate))
            {
                return string.Empty;
            }

            var chunks = Chunkify(textToTranslate);

            // var translatedChunks = await Task.WhenAll(Array.ConvertAll(chunks, TranslateChunkAsync));
            List<string> translatedChunks = new List<string>();
            foreach (var chunk in chunks)
            {
                var translatedChunk = await TranslateChunkAsync(chunk);
                translatedChunks.Add(translatedChunk);
            }
            return string.Join("\n\n", translatedChunks);
        }


        private async Task<string> TranslateChunkAsync(string chunk)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new("Bearer", _apiKey);
            var requestBody = new ChatGPTCompletionData
            {
                model = "gpt-3.5-turbo",
                messages = new List<ChatGPTMessage> {
                    new ChatGPTMessage {
                        role = "system",
                        content = "Russian to English translator keeping all the LaTeX markup intact",
                    },
                    new ChatGPTMessage {
                        role = "user",
                        content = chunk,
                    }
                }
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("\n\nRequest:\n"+requestJson);
            Console.WriteLine();
            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", 
            new StringContent(requestJson, Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"The server returned an error: {response.StatusCode.ToString()}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseBody = JsonSerializer.Deserialize<ChatGPTResponse>(responseJson);
            // Write to Console responseBody
            Console.WriteLine("Response:");
            Console.WriteLine(JsonSerializer.Serialize(responseBody));
            return responseBody.Choices[0].Message.Content;
        }

        private string[] Chunkify(string text)
        {
            var chunks = text.Split(new[] { "\r\n\r\n" }, StringSplitOptions.None);
            chunks = chunks.Where(w=>w.Length>0 && w!="\r\n").ToArray();
            return chunks;
        }

    }

    public class ChatGPTResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object")]
        public string ObjectType { get; set; } // Renamed to avoid error

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }
    }

    public class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }


}
