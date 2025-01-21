using System.Text.Json.Serialization;

namespace Dexlaris.Azf1HttpTrigger.Models;

public record User(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("role")] string Role
);