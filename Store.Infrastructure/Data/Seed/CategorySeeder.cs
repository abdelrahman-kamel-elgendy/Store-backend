using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;

namespace Store.Infrastructure.Data.Seed;

public static class CategorySeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Idempotent: only seed if the table is empty
        if (await db.Categories.AnyAsync())
            return;

        // level 1: root departments
        var roots = new List<Category>
        {
            new() { Name = "Electronics", Description = "Phones, laptops, audio, cameras and smart devices" },
            new() { Name = "Computers", Description = "Desktops, laptops, monitors, components and peripherals" },
            new() { Name = "Home & Kitchen", Description = "Furniture, cookware, bedding, decor and appliances" },
            new() { Name = "Fashion", Description = "Men's and women's clothing, shoes and accessories" },
            new() { Name = "Beauty & Personal Care",Description = "Skincare, makeup, fragrance, hair and grooming" },
            new() { Name = "Sports & Outdoors", Description = "Fitness, camping, cycling, team sports and outdoor gear" },
            new() { Name = "Toys & Games", Description = "Toys, board games, Puzzles, video games and collectibles" },
            new() { Name = "Books", Description = "Fiction, non-fiction, textbooks, comics and e-books" },
            new() { Name = "Automotive", Description = "Car parts, tools, accessories, tires and oils" },
            new() { Name = "Pet Supplies", Description = "Food, toys, beds, grooming and health for pets" },
            new() { Name = "Office & Stationery", Description = "Desks, chairs, Printers, Paper and writing supplies" },
            new() { Name = "Grocery", Description = "Pantry staples, snacks, beverages and household essentials" },
        };

        db.Categories.AddRange(roots);
        await db.SaveChangesAsync();

        // Helper: find a root by name (Id is populated after SaveChanges)
        Category Root(string name) => roots.Single(c => c.Name == name);

        // level 2: subcategories
        var subs = new List<Category>
        {
            // Electronics
            new() { Name = "Smartphones", Description = "Android and iOS phones, unlocked and carrier models", ParentCategoryId = Root("Electronics").Id },
            new() { Name = "Tablets", Description = "iPad, Android and Windows tablets", ParentCategoryId = Root("Electronics").Id },
            new() { Name = "Headphones", Description = "Over-ear, in-ear, wireless and noise-cancelling", ParentCategoryId = Root("Electronics").Id },
            new() { Name = "Cameras", Description = "DSLR, mirrorless, action cams and lenses", ParentCategoryId = Root("Electronics").Id },
            new() { Name = "Smart Watches", Description = "Fitness and smartwatches for every budget", ParentCategoryId = Root("Electronics").Id },

            // Computers
            new() { Name = "Laptops", Description = "Ultrabooks, gaming and business laptops", ParentCategoryId = Root("Computers").Id },
            new() { Name = "Desktops", Description = "Gaming PCs, workstations and all-in-ones", ParentCategoryId = Root("Computers").Id },
            new() { Name = "Monitors", Description = "1080p, 1440p, 4K and ultrawide displays", ParentCategoryId = Root("Computers").Id },
            new() { Name = "Keyboards & Mice", Description = "Mechanical keyboards, gaming mice and combos", ParentCategoryId = Root("Computers").Id },
            new() { Name = "Storage", Description = "SSDs, HDDs, NAS and external drives", ParentCategoryId = Root("Computers").Id },

            // Home & Kitchen
            new() { Name = "Cookware", Description = "Pots, Pans, skillets and bakeware", ParentCategoryId = Root("Home & Kitchen").Id },
            new() { Name = "Small Appliances", Description = "Coffee makers, blenders, air fryers and toasters", ParentCategoryId = Root("Home & Kitchen").Id },
            new() { Name = "Bedding", Description = "Sheets, duvets, Pillows and mattress toppers", ParentCategoryId = Root("Home & Kitchen").Id },
            new() { Name = "Furniture", Description = "Sofas, beds, desks, tables and storage", ParentCategoryId = Root("Home & Kitchen").Id },

            // Fashion
            new() { Name = "Men's Clothing", Description = "Shirts, jeans, jackets and activewear", ParentCategoryId = Root("Fashion").Id },
            new() { Name = "Women's Clothing", Description = "Dresses, tops, jeans, outerwear and activewear", ParentCategoryId = Root("Fashion").Id },
            new() { Name = "Shoes", Description = "Sneakers, boots, sandals and formal footwear", ParentCategoryId = Root("Fashion").Id },
            new() { Name = "Bags & Accessories", Description = "Backpacks, wallets, belts, hats and sunglasses", ParentCategoryId = Root("Fashion").Id },

            // Beauty & Personal Care
            new() { Name = "Skincare", Description = "Cleansers, moisturizers, serums and sunscreen", ParentCategoryId = Root("Beauty & Personal Care").Id },
            new() { Name = "Makeup", Description = "Foundation, lipstick, eyeshadow and tools", ParentCategoryId = Root("Beauty & Personal Care").Id },
            new() { Name = "Hair Care", Description = "Shampoo, conditioner, styling and treatments", ParentCategoryId = Root("Beauty & Personal Care").Id },
            new() { Name = "Fragrance", Description = "Perfume, cologne, body mists and gift sets", ParentCategoryId = Root("Beauty & Personal Care").Id }, 
            // Sports & Outdoors
            new() { Name = "Fitness Equipment", Description = "Dumbbells, resistance bands, mats and machines", ParentCategoryId = Root("Sports & Outdoors").Id },
            new() { Name = "Camping & Hiking", Description = "Tents, sleeping bags, backpacks and gear", ParentCategoryId = Root("Sports & Outdoors").Id },
            new() { Name = "Cycling", Description = "Bikes, helmets, lights and accessories", ParentCategoryId = Root("Sports & Outdoors").Id },
            new() { Name = "Team Sports", Description = "Soccer, basketball, baseball and volleyball", ParentCategoryId = Root("Sports & Outdoors").Id },

            // Toys & Games
            new() { Name = "Board Games", Description = "Family, strategy and party board games", ParentCategoryId = Root("Toys & Games").Id },
            new() { Name = "Building Blocks", Description = "LEGO, magnetic tiles and construction sets", ParentCategoryId = Root("Toys & Games").Id },
            new() { Name = "Video Games", Description = "Consoles, controllers and game titles", ParentCategoryId = Root("Toys & Games").Id },
            new() { Name = "Puzzles", Description = "Jigsaw, 3D and brain-teaser puzzles", ParentCategoryId = Root("Toys & Games").Id },

            // Books
            new() { Name = "Fiction", Description = "Novels, thrillers, romance and fantasy", ParentCategoryId = Root("Books").Id },
            new() { Name = "Non-Fiction", Description = "Biography, history, science and self-help", ParentCategoryId = Root("Books").Id },
            new() { Name = "Children's Books", Description = "Picture books, early readers and young adult", ParentCategoryId = Root("Books").Id },
            new() { Name = "Textbooks", Description = "Academic and professional study materials", ParentCategoryId = Root("Books").Id },

            // Automotive
            new() { Name = "Car Parts", Description = "Brakes, filters, belts and engine components", ParentCategoryId = Root("Automotive").Id },
            new() { Name = "Car Accessories", Description = "Covers, mats, chargers and organizers", ParentCategoryId = Root("Automotive").Id },
            new() { Name = "Tools & Equipment", Description = "Wrenches, jacks, diagnostics and shop tools", ParentCategoryId = Root("Automotive").Id },
            new() { Name = "Tires & Wheels", Description = "Tires, rims, hubcaps and wheel accessories", ParentCategoryId = Root("Automotive").Id },

            // Pet Supplies
            new() { Name = "Dog Supplies", Description = "Food, treats, toys, beds and leashes", ParentCategoryId = Root("Pet Supplies").Id },
            new() { Name = "Cat Supplies", Description = "Food, litter, scratchers, toys and beds", ParentCategoryId = Root("Pet Supplies").Id },
            new() { Name = "Fish & Aquatics", Description = "Tanks, filters, food and decorations", ParentCategoryId = Root("Pet Supplies").Id },
            new() { Name = "Bird Supplies", Description = "Cages, food, Perches and toys", ParentCategoryId = Root("Pet Supplies").Id },

            // Office & Stationery
            new() { Name = "Printers & Ink", Description = "Inkjet, laser printers, toner and ink", ParentCategoryId = Root("Office & Stationery").Id },
            new() { Name = "Paper & Notebooks", Description = "Notebooks, journals, copy paper and planners", ParentCategoryId = Root("Office & Stationery").Id },
            new() { Name = "Writing Supplies", Description = "Pens, Pencils, markers and highlighters", ParentCategoryId = Root("Office & Stationery").Id },
            new() { Name = "Office Furniture", Description = "Desks, chairs, filing cabinets and shelving", ParentCategoryId = Root("Office & Stationery").Id },

            // Grocery
            new() { Name = "Snacks", Description = "Chips, cookies, candy and crackers", ParentCategoryId = Root("Grocery").Id },
            new() { Name = "Beverages", Description = "Coffee, tea, soda, juice and water", ParentCategoryId = Root("Grocery").Id },
            new() { Name = "Pantry Staples", Description = "Rice, Pasta, canned goods and cooking oils", ParentCategoryId = Root("Grocery").Id },
            new() { Name = "Household Essentials", Description = "Cleaning supplies, Paper goods and storage", ParentCategoryId = Root("Grocery").Id },
        };

        db.Categories.AddRange(subs);
        await db.SaveChangesAsync();

        // Helper: find a sub by name
        Category Sub(string name) => subs.Single(c => c.Name == name);

        // level 3: examples of sub-subcategories 
        var leafCategories = new List<Category>
        {
            new() { Name = "Android Phones", Description = "Samsung, Pixel, OnePlus and more", ParentCategoryId = Sub("Smartphones").Id },
            new() { Name = "iPhones", Description = "iPhone models from latest to refurbished", ParentCategoryId = Sub("Smartphones").Id },
            new() { Name = "Wireless Earbuds", Description = "AirPods, Galaxy Buds and alternatives", ParentCategoryId = Sub("Headphones").Id },
            new() { Name = "Noise-Cancelling", Description = "Over-ear ANC headphones from top brands", ParentCategoryId = Sub("Headphones").Id },
            new() { Name = "Gaming Laptops", Description = "High-refresh, dedicated-GPU laptops", ParentCategoryId = Sub("Laptops").Id },
            new() { Name = "Ultrabooks", Description = "Thin, light, long-battery laptops", ParentCategoryId = Sub("Laptops").Id },
            new() { Name = "Air Fryers", Description = "Compact and family-size air fryers", ParentCategoryId = Sub("Small Appliances").Id },
            new() { Name = "Coffee Makers", Description = "Drip, espresso and pod coffee machines", ParentCategoryId = Sub("Small Appliances").Id },
            new() { Name = "Sneakers", Description = "Running, casual and lifestyle sneakers", ParentCategoryId = Sub("Shoes").Id },
            new() { Name = "Boots", Description = "Ankle, hiking and dress boots", ParentCategoryId = Sub("Shoes").Id },
            new() { Name = "Jigsaw Puzzles", Description = "500, 1000 and 2000-piece puzzles", ParentCategoryId = Sub("Puzzles").Id },
            new() { Name = "Dog Food", Description = "Dry, wet and raw dog food", ParentCategoryId = Sub("Dog Supplies").Id },
            new() { Name = "Cat Food", Description = "Wet and dry cat food for all life stages", ParentCategoryId = Sub("Cat Supplies").Id },
            new() { Name = "Coffee & Tea", Description = "Beans, ground, Pods, loose-leaf and bags", ParentCategoryId = Sub("Beverages").Id },
        };

        db.Categories.AddRange(leafCategories);
        await db.SaveChangesAsync();
    }
}