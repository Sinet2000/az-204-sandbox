using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Dexlaris.Azf1HttpTrigger.Utils;

public class JsonHelper
{
    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, GetJsonOptions());

    public static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, GetJsonOptions());

    public static T? Deserialize<T>(Stream json) => JsonSerializer.Deserialize<T>(json, GetJsonOptions());

    public static bool TryDeserialize<T>(string json, out T? result)
    {
        result = default!;
        try
        {
            result = Deserialize<T>(json);

            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static JsonSerializerOptions GetJsonOptions()
    {
        var jsonOpt = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            AllowTrailingCommas = true
        };

        jsonOpt.Converters.Add(new JsonStringEnumConverter());

        return jsonOpt;
    }
}