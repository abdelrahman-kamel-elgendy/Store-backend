using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Store.Core.Entities;
using Store.Infrastructure.Data;

namespace Store.Infrastructure.Data.Seeding;

public sealed class DbSeeder(
    AppDbContext dbContext,
    IHostEnvironment hostEnvironment,
    ILogger<DbSeeder> logger)
{
    private sealed record CategorySeed(string Name, string Description, string? ParentName);

    private sealed record ProductSeed(
        string Name,
        string Description,
        decimal NewPrice,
        decimal OldPrice,
        string CategoryName);

    // Parents must appear before their children.
    private static readonly IReadOnlyList<CategorySeed> CategorySeeds =
    [
        // Root categories (6)
        new("Electronics", "Devices and accessories for everyday use.", null),
        new("Computers", "Laptops, peripherals, and networking gear for home and office.", null),
        new("Home & Kitchen", "Appliances, furniture, and decor for every room in the house.", null),
        new("Fashion", "Clothing for men and women across all seasons.", null),
        new("Sports & Outdoors", "Equipment and gear for training, sports, and outdoor activities.", null),
        new("Books & Stationery", "Notebooks, planners, and everyday writing supplies.", null),

        // Subcategories (14)
        new("Audio", "Headphones, speakers, and other audio equipment.", "Electronics"),
        new("Mobile Phones", "Smartphones and mobile phone accessories.", "Electronics"),
        new("Cameras", "Digital cameras, action cameras, and photography accessories.", "Electronics"),
        new("Wearables", "Smartwatches, fitness bands, and other wearable devices.", "Electronics"),
        new("Laptops", "Ultrabooks, gaming laptops, and everyday notebooks.", "Computers"),
        new("Computer Accessories", "Keyboards, mice, webcams, and other desk essentials.", "Computers"),
        new("Networking", "Routers, switches, and network accessories.", "Computers"),
        new("Kitchen Appliances", "Small appliances that make cooking faster and easier.", "Home & Kitchen"),
        new("Furniture", "Chairs, desks, and storage for home and office.", "Home & Kitchen"),
        new("Home Decor", "Lighting, wall art, and decorative accents for your space.", "Home & Kitchen"),
        new("Men's Clothing", "Shirts, trousers, jackets, and more for men.", "Fashion"),
        new("Women's Clothing", "Tops, dresses, jackets, and more for women.", "Fashion"),
        new("Fitness Equipment", "Weights, mats, and gear for training at home or in the gym.", "Sports & Outdoors"),
        new("Notebooks & Planners", "Notebooks, journals, and planners for work and study.", "Books & Stationery"),
    ];

    private static readonly IReadOnlyList<ProductSeed> ProductSeeds =
    [
        new("Studio Headphones", "Comfortable over-ear headphones for work and listening.", 89.99m, 119.99m, "Audio"),
        new("Portable Bluetooth Speaker", "A compact wireless speaker for indoor and outdoor use.", 59.99m, 79.99m, "Audio"),
        new("Wireless Earbuds Pro", "Noise-cancelling true wireless earbuds with a charging case.", 129.99m, 159.99m, "Audio"),
        new("Smartphone X12 128GB", "6.5-inch OLED smartphone with a dual camera and all-day battery.", 499.99m, 599.99m, "Mobile Phones"),
        new("Smartphone Lite 64GB", "Affordable smartphone with a bright display and fast charging.", 249.99m, 299.99m, "Mobile Phones"),
        new("Mirrorless Camera Kit", "24MP mirrorless camera with a versatile 18-55mm kit lens.", 799.99m, 949.99m, "Cameras"),
        new("Action Camera 4K", "Waterproof 4K action camera with image stabilization.", 149.99m, 189.99m, "Cameras"),
        new("Fitness Smartwatch", "Smartwatch with heart rate tracking, GPS, and sleep monitoring.", 119.99m, 149.99m, "Wearables"),
        new("Ultrabook 14 inch", "Lightweight 14-inch laptop with a fast SSD and long battery life.", 899.99m, 1049.99m, "Laptops"),
        new("Gaming Laptop 15 inch", "High-refresh-rate gaming laptop with a dedicated graphics card.", 1199.99m, 1399.99m, "Laptops"),
        new("Mechanical Keyboard", "Backlit mechanical keyboard with tactile switches and a full layout.", 69.99m, 89.99m, "Computer Accessories"),
        new("Wireless Ergonomic Mouse", "Comfortable wireless mouse with adjustable DPI and silent clicks.", 29.99m, 39.99m, "Computer Accessories"),
        new("Wi-Fi 6 Router", "Dual-band Wi-Fi 6 router with wide coverage and parental controls.", 79.99m, 99.99m, "Networking"),
        new("Air Fryer 5L", "Large-capacity air fryer with digital presets for healthy cooking.", 74.99m, 99.99m, "Kitchen Appliances"),
        new("Espresso Machine", "Semi-automatic espresso machine with a steam wand for milk.", 189.99m, 249.99m, "Kitchen Appliances"),
        new("Ergonomic Office Chair", "Adjustable office chair with lumbar support and breathable mesh.", 159.99m, 219.99m, "Furniture"),
        new("Ceramic Table Lamp", "Modern ceramic table lamp with a linen shade and warm light.", 34.99m, 44.99m, "Home Decor"),
        new("Men's Cotton Polo Shirt", "Classic-fit polo shirt made from soft breathable cotton.", 24.99m, 34.99m, "Men's Clothing"),
        new("Women's Running Jacket", "Lightweight water-resistant jacket with reflective details.", 54.99m, 74.99m, "Women's Clothing"),
        new("Adjustable Dumbbell Set", "Space-saving adjustable dumbbells for full-body home workouts.", 99.99m, 129.99m, "Fitness Equipment"),
    ];

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!hostEnvironment.IsDevelopment())
            throw new InvalidOperationException("Development seed data can only be applied in the Development environment.");

        logger.LogInformation("Applying pending database migrations before development seeding.");
        await dbContext.Database.MigrateAsync(cancellationToken);

        var seededItems = new List<string>();
        var skippedItems = new List<string>();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var categories = await SeedCategoriesAsync(seededItems, skippedItems, cancellationToken);
        await SeedProductsAsync(categories, seededItems, skippedItems, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation(
            "Development seed completed. Added {Count} item(s): {Items}",
            seededItems.Count,
            string.Join(", ", seededItems));

        if (skippedItems.Count > 0)
        {
            logger.LogInformation(
                "Development seed skipped {Count} existing item(s): {Items}",
                skippedItems.Count,
                string.Join(", ", skippedItems));
        }
    }

    private async Task<Dictionary<string, Category>> SeedCategoriesAsync(
        ICollection<string> seededItems,
        ICollection<string> skippedItems,
        CancellationToken cancellationToken)
    {
        // One query for all existing categories instead of one per item.
        var existing = await dbContext.Categories
            .IgnoreQueryFilters()
            .ToListAsync(cancellationToken);

        var categories = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);
        foreach (var category in existing)
            categories.TryAdd(category.Name, category);

        foreach (var seed in CategorySeeds)
        {
            if (categories.ContainsKey(seed.Name))
            {
                skippedItems.Add($"category '{seed.Name}'");
                continue;
            }

            Category? parent = null;
            if (seed.ParentName is not null && !categories.TryGetValue(seed.ParentName, out parent))
            {
                throw new InvalidOperationException(
                    $"Parent category '{seed.ParentName}' for '{seed.Name}' was not found. Parents must be listed before their children.");
            }

            var newCategory = new Category
            {
                Name = seed.Name,
                Description = seed.Description,
                ParentCategory = parent
            };

            dbContext.Categories.Add(newCategory);
            categories[seed.Name] = newCategory;
            seededItems.Add($"category '{seed.Name}'");
        }

        return categories;
    }

    private async Task SeedProductsAsync(
        IReadOnlyDictionary<string, Category> categories,
        ICollection<string> seededItems,
        ICollection<string> skippedItems,
        CancellationToken cancellationToken)
    {
        var existingNames = (await dbContext.Products
                .IgnoreQueryFilters()
                .Select(product => product.Name)
                .ToListAsync(cancellationToken))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var seed in ProductSeeds)
        {
            if (existingNames.Contains(seed.Name))
            {
                skippedItems.Add($"product '{seed.Name}'");
                continue;
            }

            if (!categories.TryGetValue(seed.CategoryName, out var category))
            {
                throw new InvalidOperationException(
                    $"Category '{seed.CategoryName}' for product '{seed.Name}' was not found.");
            }

            dbContext.Products.Add(new Product
            {
                Name = seed.Name,
                Description = seed.Description,
                NewPrice = seed.NewPrice,
                OldPrice = seed.OldPrice,
                Category = category
            });

            seededItems.Add($"product '{seed.Name}'");
        }
    }
}