using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestaurantOrder.IntegrationTests;

internal static class TestJsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };
}
