using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TronusDesktop.Models
{
    public class ApiErrorResponse
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        // message can be a string or an array of strings in NestJS validations. 
        // We will represent it as an object and handle the deserialization logic if needed, 
        // but for now let's use JsonElement or an object to be flexible.
        [JsonPropertyName("message")]
        public object Message { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        public string GetErrorMessage()
        {
            if (Message == null) return Error ?? "Erro desconhecido.";

            if (Message is System.Text.Json.JsonElement element)
            {
                if (element.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    var errors = new List<string>();
                    foreach (var item in element.EnumerateArray())
                    {
                        errors.Add(item.GetString());
                    }
                    return string.Join(Environment.NewLine, errors);
                }
                else if (element.ValueKind == System.Text.Json.JsonValueKind.String)
                {
                    return element.GetString();
                }
            }

            return Message.ToString();
        }
    }
}
