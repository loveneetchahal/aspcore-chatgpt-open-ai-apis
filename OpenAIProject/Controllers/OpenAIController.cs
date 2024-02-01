using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenAIController : ControllerBase
    {
        [HttpGet("completion")]
        public async Task<IActionResult> GetMessage(string message)
        {

            using (HttpClient httpClient = new())
            {

                              ChatCompletionRequest completionRequest = new()
                {
                    Model = "gpt-3.5-turbo",
                    MaxTokens =4000,
                    Messages = new List<Message> // Initialize with a new list of messages
                    {
                        new Message
                        {
                            Role = "user",
                            Content = message,
                        }
                    }
                };
                ChatCompletionResponse completionResponse = new();
                string apiKey = "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";

                using var httpReq = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
                httpReq.Headers.Add("Authorization", $"Bearer {apiKey}");

                string requestString = JsonSerializer.Serialize(completionRequest);
                httpReq.Content = new StringContent(requestString, Encoding.UTF8, "application/json");
                using HttpResponseMessage? httpResponse = await httpClient.SendAsync(httpReq);

                if (httpResponse is not null)
                {
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        string responseString = await httpResponse.Content.ReadAsStringAsync();
                        {
                            if (!string.IsNullOrWhiteSpace(responseString))
                            {
                                completionResponse = JsonSerializer.Deserialize<ChatCompletionResponse>(responseString);
                            }
                        }
                    }
                }
                if (completionResponse is not null)
                {
                    string? completionText = completionResponse.Choices?[0]?.Message?.Content;
                    return Ok(completionText);
                }
            }

            return NotFound();
        }


    }


    public class ChatCompletionRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }
        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }
        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }

    }



    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("logprobs")]
        public object Logprobs { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class ChatCompletionResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("created")]
        public int Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }

        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }

        [JsonPropertyName("system_fingerprint")]
        public object SystemFingerprint { get; set; }
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


}