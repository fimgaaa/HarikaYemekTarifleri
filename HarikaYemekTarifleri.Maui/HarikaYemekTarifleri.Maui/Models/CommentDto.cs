using System.Text.Json.Serialization;

namespace HarikaYemekTarifleri.Maui.Models;

public class CommentDto
{
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("createdBy")]
    public string UserName { get; set; } = string.Empty;
}