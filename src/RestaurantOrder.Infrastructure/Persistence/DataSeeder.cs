using Microsoft.EntityFrameworkCore;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(RestaurantDbContext context)
    {
        var existingItems = await context.MenuItems.ToListAsync();
        if (existingItems.Count == 0)
        {
            var menuItems = new[]
            {
                MenuItem.Create("Soupe à l'oignon", 7.50m, MenuItemCategory.Starter, imageUrl: "/images/soupe_oignon.jpg"),
                MenuItem.Create("Salade César", 9.00m, MenuItemCategory.Starter, imageUrl: "/images/salade_cesar.jpg"),
                MenuItem.Create("Foie gras", 14.00m, MenuItemCategory.Starter, imageUrl: "/images/foie_gras.jpg"),
                MenuItem.Create("Entrecôte grillée", 24.00m, MenuItemCategory.MainCourse, imageUrl: "/images/entrecote.jpg"),
                MenuItem.Create("Poulet rôti", 18.00m, MenuItemCategory.MainCourse, imageUrl: "/images/poulet_roti.jpg"),
                MenuItem.Create("Saumon en croûte", 22.00m, MenuItemCategory.MainCourse, imageUrl: "/images/saumon.jpg"),
                MenuItem.Create("Risotto aux champignons", 16.00m, MenuItemCategory.MainCourse, imageUrl: "/images/risotto.jpg"),
                MenuItem.Create("Crème brûlée", 7.00m, MenuItemCategory.Dessert, imageUrl: "/images/creme_brulee.jpg"),
                MenuItem.Create("Tarte tatin", 8.00m, MenuItemCategory.Dessert, imageUrl: "/images/tarte_tatin.jpg"),
                MenuItem.Create("Moelleux au chocolat", 8.50m, MenuItemCategory.Dessert, imageUrl: "/images/moelleux_chocolat.jpg"),
                MenuItem.Create("Eau minérale", 2.50m, MenuItemCategory.Beverage, imageUrl: "/images/eau_minerale.jpg"),
                MenuItem.Create("Coca-Cola", 3.00m, MenuItemCategory.Beverage, imageUrl: "/images/coca_cola.jpg"),
                MenuItem.Create("Café", 2.00m, MenuItemCategory.Beverage, imageUrl: "/images/cafe.jpg"),
                MenuItem.Create("Vin rouge (verre)", 5.00m, MenuItemCategory.Beverage, imageUrl: "/images/vin_rouge.jpg"),
                MenuItem.Create("Frites", 4.00m, MenuItemCategory.Side, imageUrl: "/images/frites.jpg"),
                MenuItem.Create("Légumes du jour", 4.50m, MenuItemCategory.Side, imageUrl: "/images/legumes.jpg"),
            };
            await context.MenuItems.AddRangeAsync(menuItems);
        }
        else
        {
            var imageMap = new Dictionary<string, string>
            {
                { "Soupe à l'oignon", "/images/soupe_oignon.jpg" },
                { "Salade César", "/images/salade_cesar.jpg" },
                { "Foie gras", "/images/foie_gras.jpg" },
                { "Entrecôte grillée", "/images/entrecote.jpg" },
                { "Poulet rôti", "/images/poulet_roti.jpg" },
                { "Saumon en croûte", "/images/saumon.jpg" },
                { "Risotto aux champignons", "/images/risotto.jpg" },
                { "Crème brûlée", "/images/creme_brulee.jpg" },
                { "Tarte tatin", "/images/tarte_tatin.jpg" },
                { "Moelleux au chocolat", "/images/moelleux_chocolat.jpg" },
                { "Eau minérale", "/images/eau_minerale.jpg" },
                { "Coca-Cola", "/images/coca_cola.jpg" },
                { "Café", "/images/cafe.jpg" },
                { "Vin rouge (verre)", "/images/vin_rouge.jpg" },
                { "Frites", "/images/frites.jpg" },
                { "Légumes du jour", "/images/legumes.jpg" }
            };

            foreach (var item in existingItems)
            {
                if (string.IsNullOrEmpty(item.ImageUrl) && imageMap.TryGetValue(item.Name, out var url))
                {
                    item.Update(item.Name, item.Price, item.Category, item.IsAvailable, url);
                }
            }
        }

        if (!await context.Tables.AnyAsync())
        {
            var tables = new[]
            {
                Table.Create(1, 2),
                Table.Create(2, 2),
                Table.Create(3, 4),
                Table.Create(4, 4),
                Table.Create(5, 6),
                Table.Create(6, 8),
            };
            await context.Tables.AddRangeAsync(tables);
        }

        await context.SaveChangesAsync();
    }
}
