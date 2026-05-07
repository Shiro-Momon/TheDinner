using Microsoft.EntityFrameworkCore;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(RestaurantDbContext context)
    {
        if (await context.MenuItems.AnyAsync() || await context.Tables.AnyAsync())
            return;

        var menuItems = new[]
        {
            MenuItem.Create("Soupe à l'oignon", 7.50m, MenuItemCategory.Starter),
            MenuItem.Create("Salade César", 9.00m, MenuItemCategory.Starter),
            MenuItem.Create("Foie gras", 14.00m, MenuItemCategory.Starter),
            MenuItem.Create("Entrecôte grillée", 24.00m, MenuItemCategory.MainCourse),
            MenuItem.Create("Poulet rôti", 18.00m, MenuItemCategory.MainCourse),
            MenuItem.Create("Saumon en croûte", 22.00m, MenuItemCategory.MainCourse),
            MenuItem.Create("Risotto aux champignons", 16.00m, MenuItemCategory.MainCourse),
            MenuItem.Create("Crème brûlée", 7.00m, MenuItemCategory.Dessert),
            MenuItem.Create("Tarte tatin", 8.00m, MenuItemCategory.Dessert),
            MenuItem.Create("Moelleux au chocolat", 8.50m, MenuItemCategory.Dessert),
            MenuItem.Create("Eau minérale", 2.50m, MenuItemCategory.Beverage),
            MenuItem.Create("Coca-Cola", 3.00m, MenuItemCategory.Beverage),
            MenuItem.Create("Café", 2.00m, MenuItemCategory.Beverage),
            MenuItem.Create("Vin rouge (verre)", 5.00m, MenuItemCategory.Beverage),
            MenuItem.Create("Frites", 4.00m, MenuItemCategory.Side),
            MenuItem.Create("Légumes du jour", 4.50m, MenuItemCategory.Side),
        };

        var tables = new[]
        {
            Table.Create(1, 2),
            Table.Create(2, 2),
            Table.Create(3, 4),
            Table.Create(4, 4),
            Table.Create(5, 6),
            Table.Create(6, 8),
        };

        await context.MenuItems.AddRangeAsync(menuItems);
        await context.Tables.AddRangeAsync(tables);
        await context.SaveChangesAsync();
    }
}
