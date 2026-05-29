using RestaurantOrder.Domain.Enums;
using RestaurantOrder.Domain.Exceptions;

namespace RestaurantOrder.Domain.Entities;

public class MenuItem
{
    private MenuItem()
    {
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public MenuItemCategory Category { get; private set; }
    public bool IsAvailable { get; private set; }
    public string? ImageUrl { get; private set; }

    public static MenuItem Create(string name, decimal price, MenuItemCategory category, bool isAvailable = true, string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Menu item name cannot be empty.");
        if (price <= 0)
            throw new DomainException("Menu item price must be greater than zero.");

        return new MenuItem
        {
            Name = name.Trim(),
            Price = price,
            Category = category,
            IsAvailable = isAvailable,
            ImageUrl = imageUrl,
        };
    }

    public void Update(string name, decimal price, MenuItemCategory category, bool isAvailable, string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Menu item name cannot be empty.");
        if (price <= 0)
            throw new DomainException("Menu item price must be greater than zero.");

        Name = name.Trim();
        Price = price;
        Category = category;
        IsAvailable = isAvailable;
        ImageUrl = imageUrl;
    }
}
