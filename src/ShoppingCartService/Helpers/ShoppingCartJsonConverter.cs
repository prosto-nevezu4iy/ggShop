using System.Text.Json;
using System.Text.Json.Serialization;
using ShoppingCartService.Entities;
using ShoppingCartItemEntity = ShoppingCartService.Entities.ShoppingCartItem;

namespace ShoppingCartService.Helpers;

public class ShoppingCartJsonConverter : JsonConverter<ShoppingCart>
{
    public override ShoppingCart Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var userId = root.GetProperty(nameof(ShoppingCart.UserId)).GetGuid();
        var cart = new ShoppingCart(userId);

        if (root.TryGetProperty(nameof(ShoppingCart.Items), out var itemsElement))
        {
            var items = JsonSerializer.Deserialize<List<ShoppingCartItemEntity>>(
                itemsElement.GetRawText(),
                options
            ) ?? [];

            foreach (var item in items)
            {
                cart.AddItem(item.GameId, item.Name, item.Price, item.Quantity, item.ImageUrl);
            }
        }

        return cart;
    }

    public override void Write(Utf8JsonWriter writer, ShoppingCart value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString(nameof(ShoppingCart.UserId), value.UserId);

        writer.WritePropertyName(nameof(ShoppingCart.Items));
        JsonSerializer.Serialize(writer, value.Items, options);

        writer.WriteEndObject();
    }
}
