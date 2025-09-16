using System.Text.Json.Serialization;

namespace HarikaYemekTarifleri.Maui.Models;

public class CommentDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("createdBy")]
    public string UserName { get; set; } = string.Empty;

    [JsonIgnore]
    public bool IsMine { get; set; }
}