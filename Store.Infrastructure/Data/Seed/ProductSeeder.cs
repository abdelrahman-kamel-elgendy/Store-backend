using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;

namespace Store.Infrastructure.Data.Seed;

public static class ProductSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Idempotent: only seed if the table is empty
        if (await db.Products.AnyAsync())
            return;

        // Load categories so we can resolve CategoryId by name.
        // (CategorySeeder must have run first.)
        var categories = await db.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);

        int CatId(string name) => categories.TryGetValue(name, out var id) ? id : throw new InvalidOperationException($"Category '{name}' not found. Run CategorySeeder first.");

        var products = new List<Product>
        {
            // electronics › smartphones › android Phones
            new()
            {
                Name = "Samsung Galaxy S24 Ultra 256GB",
                Description = "6.8\" QHD+ AMOLED, Snapdragon 8 Gen 3, 200MP camera, 5000mAh battery.",
                NewPrice = 1299.99m, OldPrice = 1399.99m,
                CategoryId = CatId("Android Phones")
            },
            new()
            {
                Name = "Google Pixel 8 Pro 128GB",
                Description = "Tensor G3, 6.7\" LTPO OLED, 50MP triple camera, 7 years of updates.",
                NewPrice = 899.00m, OldPrice = 999.00m,
                CategoryId = CatId("Android Phones")
            },
            new()
            {
                Name = "OnePlus 12 512GB",
                Description = "Snapdragon 8 Gen 3, 6.82\" 120Hz AMOLED, 100W SUPERVOOC charging.",
                NewPrice = 799.99m, OldPrice = 899.99m,
                CategoryId = CatId("Android Phones")
            },

            // electronics › smartphones › iPhones 
            new()
            {
                Name = "Apple iPhone 15 Pro 128GB",
                Description = "A17 Pro chip, titanium frame, 48MP main camera, USB-C.",
                NewPrice = 999.00m, OldPrice = 1099.00m,
                CategoryId = CatId("iPhones")
            },
            new()
            {
                Name = "Apple iPhone 14 128GB (Refurbished)",
                Description = "Certified refurbished, A15 Bionic, dual 12MP camera, 1-year warranty.",
                NewPrice = 599.00m, OldPrice = 699.00m,
                CategoryId = CatId("iPhones")
            },

            // electronics › headphones
            new()
            {
                Name = "Sony WH-1000XM5 Wireless Headphones",
                Description = "Industry-leading ANC, 30-hour battery, multipoint Bluetooth.",
                NewPrice = 349.99m, OldPrice = 399.99m,
                CategoryId = CatId("Headphones")
            },
            new()
            {
                Name = "Apple AirPods Pro (2nd Gen)",
                Description = "Adaptive audio, USB-C MagSafe case, up to 6h listening.",
                NewPrice = 229.00m, OldPrice = 249.00m,
                CategoryId = CatId("Wireless Earbuds")
            },
            new()
            {
                Name = "Bose QuietComfort Ultra Earbuds",
                Description = "Immersive audio, world-class noise cancellation, 6h battery.",
                NewPrice = 279.99m, OldPrice = 299.99m,
                CategoryId = CatId("Noise-Cancelling")
            },

            // electronics › tablets 
            new()
            {
                Name = "Apple iPad Air 11\" M2 128GB",
                Description = "M2 chip, Liquid Retina display, Apple Pencil Pro support.",
                NewPrice = 599.00m, OldPrice = 649.00m,
                CategoryId = CatId("Tablets")
            },
            new()
            {
                Name = "Samsung Galaxy Tab S9 FE 128GB",
                Description = "10.9\" TFT, S Pen included, IP68 water resistance.",
                NewPrice = 449.99m, OldPrice = 499.99m,
                CategoryId = CatId("Tablets")
            },

            // electronics › cameras 
            new()
            {
                Name = "Sony Alpha a7 IV Mirrorless Body",
                Description = "33MP full-frame, 4K60 video, real-time Eye AF.",
                NewPrice = 2498.00m, OldPrice = 2699.00m,
                CategoryId = CatId("Cameras")
            },
            new()
            {
                Name = "GoPro HERO12 Black",
                Description = "5.3K60 video, HyperSmooth 6.0, waterproof to 10m.",
                NewPrice = 349.00m, OldPrice = 399.00m,
                CategoryId = CatId("Cameras")
            },

            // electronics › smart watches
            new()
            {
                Name = "Apple Watch Series 9 GPS 45mm",
                Description = "S9 chip, double-tap gesture, always-on Retina display.",
                NewPrice = 399.00m, OldPrice = 429.00m,
                CategoryId = CatId("Smart Watches")
            },
            new()
            {
                Name = "Garmin Forerunner 265",
                Description = "AMOLED display, training readiness, multi-band GPS.",
                NewPrice = 449.99m, OldPrice = 479.99m,
                CategoryId = CatId("Smart Watches")
            },

            // Computers › laptops 
            new()
            {
                Name = "Apple MacBook Air 13\" M3 256GB",
                Description = "M3 chip, 8-core CPU, 18h battery, fanless design.",
                NewPrice = 1049.00m, OldPrice = 1099.00m,
                CategoryId = CatId("Ultrabooks")
            },
            new()
            {
                Name = "Dell XPS 13 Plus 9320",
                Description = "13th Gen Intel i7, 16GB RAM, 512GB SSD, 3.5K OLED.",
                NewPrice = 1299.00m, OldPrice = 1499.00m,
                CategoryId = CatId("Ultrabooks")
            },
            new()
            {
                Name = "ASUS ROG Zephyrus G14 (2024)",
                Description = "Ryzen 9 8945HS, RTX 4070, 14\" QHD 165Hz, 1TB SSD.",
                NewPrice = 1899.99m, OldPrice = 2099.99m,
                CategoryId = CatId("Gaming Laptops")
            },
            new()
            {
                Name = "Lenovo Legion Pro 5i Gen 9",
                Description = "Intel i7-14650HX, RTX 4060, 16\" WQXGA 240Hz, 1TB SSD.",
                NewPrice = 1499.00m, OldPrice = 1699.00m,
                CategoryId = CatId("Gaming Laptops")
            },

            // Computers › monitors
            new()
            {
                Name = "LG UltraGear 27GR95QE 27\" OLED",
                Description = "2560x1440, 240Hz, 0.03ms, HDR10, DisplayPort 1.4.",
                NewPrice = 799.99m, OldPrice = 999.99m,
                CategoryId = CatId("Monitors")
            },
            new()
            {
                Name = "Dell UltraSharp U2723QE 27\" 4K",
                Description = "IPS Black, 60Hz, USB-C 90W, factory calibrated.",
                NewPrice = 549.00m, OldPrice = 629.00m,
                CategoryId = CatId("Monitors")
            },

            // Computers › keyboards & mice
            new()
            {
                Name = "Logitech MX Master 3S",
                Description = "8K DPI any-surface tracking, quiet clicks, USB-C.",
                NewPrice = 89.99m, OldPrice = 99.99m,
                CategoryId = CatId("Keyboards & Mice")
            },
            new()
            {
                Name = "Keychron Q1 Pro Mechanical Keyboard",
                Description = "75% wireless, hot-swappable, QMK/VIA, aluminum body.",
                NewPrice = 199.00m, OldPrice = 219.00m,
                CategoryId = CatId("Keyboards & Mice")
            },

            // Computers › storage
            new()
            {
                Name = "Samsung 990 PRO 2TB NVMe SSD",
                Description = "PCIe 4.0, up to 7450 MB/s, heatsink option available.",
                NewPrice = 169.99m, OldPrice = 199.99m,
                CategoryId = CatId("Storage")
            },
            new()
            {
                Name = "WD My Passport 4TB Portable HDD",
                Description = "USB 3.2, hardware encryption, 3-year warranty.",
                NewPrice = 99.99m, OldPrice = 119.99m,
                CategoryId = CatId("Storage")
            },

            // home & kitchen› small appliances
            new()
            {
                Name = "Ninja AF101 Air Fryer 4QT",
                Description = "4-quart ceramic-coated basket, 105°F–400°F, dishwasher-safe.",
                NewPrice = 89.99m, OldPrice = 129.99m,
                CategoryId = CatId("Air Fryers")
            },
            new()
            {
                Name = "Cosori Pro II 5.8QT Air Fryer",
                Description = "11 presets, shake reminder, non-stick basket, 1700W.",
                NewPrice = 109.99m, OldPrice = 139.99m,
                CategoryId = CatId("Air Fryers")
            },
            new()
            {
                Name = "Breville Barista Express Espresso Machine",
                Description = "Built-in conical burr grinder, 15-bar pump, steam wand.",
                NewPrice = 699.95m, OldPrice = 799.95m,
                CategoryId = CatId("Coffee Makers")
            },
            new()
            {
                Name = "Keurig K-Supreme Plus Coffee Maker",
                Description = "Single-serve, MultiStream technology, 5 cup sizes.",
                NewPrice = 149.99m, OldPrice = 179.99m,
                CategoryId = CatId("Coffee Makers")
            },

            // home & kitchen› cookware
            new()
            {
                Name = "Lodge 12\" Cast Iron Skillet",
                Description = "Pre-seasoned, oven-safe, made in USA.",
                NewPrice = 34.99m, OldPrice = 44.99m,
                CategoryId = CatId("Cookware")
            },
            new()
            {
                Name = "All-Clad D3 Stainless 10-Piece Set",
                Description = "Tri-ply bonded construction, oven and broiler safe.",
                NewPrice = 799.99m, OldPrice = 999.99m,
                CategoryId = CatId("Cookware")
            },

            // home & kitchen› bedding
            new()
            {
                Name = "Brooklinen Luxe Core Sheet Set (Queen)",
                Description = "480 thread count sateen, long-staple cotton.",
                NewPrice = 159.00m, OldPrice = 189.00m,
                CategoryId = CatId("Bedding")
            },

            // home & kitchen› furniture 
            new()
            {
                Name = "Herman Miller Aeron Chair (Size B)",
                Description = "Ergonomic mesh chair, 12-year warranty, PostureFit SL.",
                NewPrice = 1499.00m, OldPrice = 1799.00m,
                CategoryId = CatId("Furniture")
            },

            // fashion › men's clothing
            new()
            {
                Name = "Levi's 511 Slim Fit Jeans",
                Description = "Stretch denim, 5-pocket, slim through thigh and leg.",
                NewPrice = 49.99m, OldPrice = 69.50m,
                CategoryId = CatId("Men's Clothing")
            },
            new()
            {
                Name = "Patagonia Better Sweater Fleece Jacket",
                Description = "Recycled polyester fleece, quarter-zip, Fair Trade certified.",
                NewPrice = 99.00m, OldPrice = 139.00m,
                CategoryId = CatId("Men's Clothing")
            },

            // fashion › women's clothing 
            new()
            {
                Name = "Lululemon Align High-Rise Leggings 28\"",
                Description = "Buttery-soft Nulu fabric, lightweight, four-way stretch.",
                NewPrice = 98.00m, OldPrice = 118.00m,
                CategoryId = CatId("Women's Clothing")
            },
            new()
            {
                Name = "Levi's 721 High-Rise Skinny Jeans",
                Description = "Slim through hip and thigh, mid-weight stretch denim.",
                NewPrice = 59.99m, OldPrice = 79.50m,
                CategoryId = CatId("Women's Clothing")
            },

            // fashion › shoes 
            new()
            {
                Name = "Nike Air Zoom Pegasus 40",
                Description = "Responsive ZoomX foam, breathable mesh, everyday trainer.",
                NewPrice = 109.97m, OldPrice = 140.00m,
                CategoryId = CatId("Sneakers")
            },
            new()
            {
                Name = "Adidas Ultraboost Light",
                Description = "Light BOOST midsole, Primeknit+ upper, Continental outsole.",
                NewPrice = 149.99m, OldPrice = 190.00m,
                CategoryId = CatId("Sneakers")
            },
            new()
            {
                Name = "Timberland 6\" Premium Waterproof Boot",
                Description = "Full-grain leather, seam-sealed waterproof, padded collar.",
                NewPrice = 169.99m, OldPrice = 210.00m,
                CategoryId = CatId("Boots")
            },

            // fashion › bags & accessories 
            new()
            {
                Name = "Ray-Ban Wayfarer Classic Sunglasses",
                Description = "Acetate frame, G-15 green lenses, 100% UV protection.",
                NewPrice = 129.00m, OldPrice = 163.00m,
                CategoryId = CatId("Bags & Accessories")
            },

            // beauty & personal care › skincare 
            new()
            {
                Name = "CeraVe Moisturizing Cream 16oz",
                Description = "Ceramides and hyaluronic acid, fragrance-free, non-comedogenic.",
                NewPrice = 15.99m, OldPrice = 19.99m,
                CategoryId = CatId("Skincare")
            },
            new()
            {
                Name = "The Ordinary Niacinamide 10% + Zinc 1%",
                Description = "High-strength vitamin and mineral blemish formula, 30ml.",
                NewPrice = 5.90m, OldPrice = 7.90m,
                CategoryId = CatId("Skincare")
            },

            // beauty & personal care › makeup 
            new()
            {
                Name = "Maybelline Lash Sensational Mascara",
                Description = "Fanning brush, layer-reveal formula, washable blackest black.",
                NewPrice = 8.49m, OldPrice = 10.99m,
                CategoryId = CatId("Makeup")
            },

            // beauty & personal care › hair care 
            new()
            {
                Name = "Olaplex No.3 Hair Perfector 100ml",
                Description = "At-home bond-building treatment, reduces breakage.",
                NewPrice = 28.00m, OldPrice = 32.00m,
                CategoryId = CatId("Hair Care")
            },

            // beauty & personal care › fragrance 
            new()
            {
                Name = "Dior Sauvage Eau de Parfum 100ml",
                Description = "Bergamot, Sichuan pepper, ambroxan, vanilla. Men's fragrance.",
                NewPrice = 124.99m, OldPrice = 145.00m,
                CategoryId = CatId("Fragrance")
            },

            // sports & outdoors › fitness equipment
            new()
            {
                Name = "Bowflex SelectTech 552 Adjustable Dumbbells",
                Description = "5–52.5 lb per dumbbell, quick dial weight change, pair.",
                NewPrice = 349.00m, OldPrice = 429.00m,
                CategoryId = CatId("Fitness Equipment")
            },
            new()
            {
                Name = "Manduka PRO Yoga Mat 6mm",
                Description = "Dense cushioning, closed-cell surface, lifetime warranty.",
                NewPrice = 108.00m, OldPrice = 128.00m,
                CategoryId = CatId("Fitness Equipment")
            },

            // sports & outdoors › camping & hiking
            new()
            {
                Name = "Big Agnes Copper Spur HV UL2 Tent",
                Description = "2-person, 3-season, ultralight backpacking tent, 3 lb 2 oz.",
                NewPrice = 479.95m, OldPrice = 549.95m,
                CategoryId = CatId("Camping & Hiking")
            },
            new()
            {
                Name = "Osprey Atmos AG 65 Backpack",
                Description = "65L men's backpacking pack, Anti-Gravity suspension.",
                NewPrice = 299.95m, OldPrice = 340.00m,
                CategoryId = CatId("Camping & Hiking")
            },

            // sports & outdoors › cycling 
            new()
            {
                Name = "Garmin Edge 840 GPS Bike Computer",
                Description = "2.6\" color display, multi-band GNSS, up to 32h battery.",
                NewPrice = 449.99m, OldPrice = 499.99m,
                CategoryId = CatId("Cycling")
            },

            // sports & outdoors › team sports 
            new()
            {
                Name = "Wilson Evolution Indoor Basketball (Official)",
                Description = "Composite leather, microfiber cover, official size 7.",
                NewPrice = 69.99m, OldPrice = 84.99m,
                CategoryId = CatId("Team Sports")
            },

            // toys & games › board games 
            new()
            {
                Name = "Catan Board Game (5th Edition)",
                Description = "Strategy game for 3–4 players, ages 10+, 60–90 min play time.",
                NewPrice = 44.99m, OldPrice = 54.99m,
                CategoryId = CatId("Board Games")
            },
            new()
            {
                Name = "Ticket to Ride Board Game",
                Description = "Cross-country train adventure for 2–5 players, ages 8+.",
                NewPrice = 39.99m, OldPrice = 49.99m,
                CategoryId = CatId("Board Games")
            },

            // toys & games › building blocks 
            new()
            {
                Name = "LEGO Icons Bonsai Tree 10281",
                Description = "878 pieces, adult building set, adjustable foliage.",
                NewPrice = 49.99m, OldPrice = 59.99m,
                CategoryId = CatId("Building Blocks")
            },

            // toys & games › video games 
            new()
            {
                Name = "PlayStation 5 Slim Disc Console",
                Description = "1TB SSD, 4K gaming, DualSense controller included.",
                NewPrice = 499.99m, OldPrice = 549.99m,
                CategoryId = CatId("Video Games")
            },

            // toys & games › puzzles 
            new()
            {
                Name = "Ravensburger 1000-Piece Puzzle: Sunset Over Lake",
                Description = "Softclick technology, linen finish, ages 12+.",
                NewPrice = 19.99m, OldPrice = 24.99m,
                CategoryId = CatId("Jigsaw Puzzles")
            },

            // books › fiction 
            new()
            {
                Name = "Fourth Wing by Rebecca Yarros (Hardcover)",
                Description = "Empyrean series #1, fantasy romance, 528 pages.",
                NewPrice = 19.99m, OldPrice = 29.99m,
                CategoryId = CatId("Fiction")
            },
            new()
            {
                Name = "The Midnight Library by Matt Haig (Paperback)",
                Description = "Contemporary fiction, 304 pages, New York Times bestseller.",
                NewPrice = 12.99m, OldPrice = 17.00m,
                CategoryId = CatId("Fiction")
            },

            // books › non-fiction 
            new()
            {
                Name = "Atomic Habits by James Clear (Hardcover)",
                Description = "An easy & proven way to build good habits, 320 pages.",
                NewPrice = 16.99m, OldPrice = 27.00m,
                CategoryId = CatId("Non-Fiction")
            },

            // books › children's books 
            new()
            {
                Name = "The Very Hungry Caterpillar (Board Book)",
                Description = "Eric Carle classic, board book edition, ages 1–3.",
                NewPrice = 7.99m, OldPrice = 10.99m,
                CategoryId = CatId("Children's Books")
            },

            // books › textbooks 
            new()
            {
                Name = "Clean Code by Robert C. Martin",
                Description = "A handbook of agile software craftsmanship, 464 pages.",
                NewPrice = 39.99m, OldPrice = 54.99m,
                CategoryId = CatId("Textbooks")
            },

            // automotive › car accessories 
            new()
            {
                Name = "Anker Roav SmartCharge F0 Bluetooth FM Transmitter",
                Description = "Dual USB, hands-free calling, bass boost, USB-C.",
                NewPrice = 19.99m, OldPrice = 27.99m,
                CategoryId = CatId("Car Accessories")
            },
            new()
            {
                Name = "NOCO Boost Plus GB40 Jump Starter",
                Description = "1000A 12V lithium jump starter, up to 6L gas engines.",
                NewPrice = 99.95m, OldPrice = 124.95m,
                CategoryId = CatId("Car Accessories")
            },

            // automotive › car parts 
            new()
            {
                Name = "Bosch ICON Wiper Blades 26\" + 19\" Pair",
                Description = "Beam blade design, all-weather performance, fits most vehicles.",
                NewPrice = 49.99m, OldPrice = 64.99m,
                CategoryId = CatId("Car Parts")
            },

            // automotive › tools & equipment 
            new()
            {
                Name = "DEWALT 20V MAX Impact Driver Kit",
                Description = "1/4\" impact driver, 2 batteries, charger and bag included.",
                NewPrice = 149.00m, OldPrice = 179.00m,
                CategoryId = CatId("Tools & Equipment")
            },

            // automotive › tires & wheels 
            new()
            {
                Name = "Michelin Defender T+H All-Season Tire 205/55R16",
                Description = "80,000-mile warranty, all-season traction, quiet ride.",
                NewPrice = 129.99m, OldPrice = 149.99m,
                CategoryId = CatId("Tires & Wheels")
            },

            // pet supplies › dog supplies 
            new()
            {
                Name = "Blue Buffalo Life Protection Dry Dog Food 30lb",
                Description = "Chicken & brown rice, adult formula, natural ingredients.",
                NewPrice = 59.99m, OldPrice = 72.99m,
                CategoryId = CatId("Dog Food")
            },
            new()
            {
                Name = "KONG Classic Dog Toy (Large)",
                Description = "Durable natural rubber, treat-stuffable, made in USA.",
                NewPrice = 14.99m, OldPrice = 19.99m,
                CategoryId = CatId("Dog Supplies")
            },

            // pet supplies › cat supplies 
            new()
            {
                Name = "Purina Pro Plan Dry Cat Food 16lb",
                Description = "Chicken & rice, adult formula, 40% protein.",
                NewPrice = 44.99m, OldPrice = 54.99m,
                CategoryId = CatId("Cat Food")
            },
            new()
            {
                Name = "PetFusion Ultimate Cat Scratcher Lounge",
                Description = "Recycled cardboard, reversible, catnip included.",
                NewPrice = 44.99m, OldPrice = 59.99m,
                CategoryId = CatId("Cat Supplies")
            },

            // pet supplies › fish & aquatics 
            new()
            {
                Name = "Fluval Spec V Aquarium Kit 5 Gallon",
                Description = "Includes LED lighting, pump and filter. 19\" desktop tank.",
                NewPrice = 119.99m, OldPrice = 149.99m,
                CategoryId = CatId("Fish & Aquatics")
            },

            // pet supplies › bird supplies 
            new()
            {
                Name = "Prevue Hendryx Flight Cage (Large)",
                Description = "Wrought iron flight cage, 4 feeder cups, removable grille.",
                NewPrice = 139.99m, OldPrice = 179.99m,
                CategoryId = CatId("Bird Supplies")
            },

            // office & stationery › printers & ink
            new()
            {
                Name = "Brother HL-L2350DW Mono Laser Printer",
                Description = "Wireless duplex printing, 32 ppm, Amazon Dash Replenishment.",
                NewPrice = 119.99m, OldPrice = 149.99m,
                CategoryId = CatId("Printers & Ink")
            },

            // office & stationery › paper & Notebooks
            new()
            {
                Name = "Moleskine Classic Notebook (Large, Ruled)",
                Description = "Hardcover, 240 pages, elastic closure, acid-free paper.",
                NewPrice = 22.95m, OldPrice = 28.95m,
                CategoryId = CatId("Paper & Notebooks")
            },

            // office & stationery › writing supplies
            new()
            {
                Name = "Pilot G2 Retractable Gel Pens (12 Pack, Black)",
                Description = "0.7mm fine point, smooth ink flow, refillable.",
                NewPrice = 14.49m, OldPrice = 19.99m,
                CategoryId = CatId("Writing Supplies")
            },

            // office & stationery › office furniture
            new()
            {
                Name = "Autonomous SmartDesk 2 (XL, White)",
                Description = "Electric standing desk, 53\" x 29\", memory presets.",
                NewPrice = 499.00m, OldPrice = 649.00m,
                CategoryId = CatId("Office Furniture")
            },

            // grocery › snacks 
            new()
            {
                Name = "Oreo Double Stuf Cookies (Party Size)",
                Description = "1lb 12.7oz resealable bag, family size.",
                NewPrice = 5.99m, OldPrice = 7.49m,
                CategoryId = CatId("Snacks")
            },

            // grocery › beverages 
            new()
            {
                Name = "Lavazza Super Crema Whole Bean Coffee 2.2lb",
                Description = "Medium roast, 100% Arabica, espresso blend.",
                NewPrice = 19.99m, OldPrice = 24.99m,
                CategoryId = CatId("Coffee & Tea")
            },
            new()
            {
                Name = "Keurig K-Cup Variety Pack (40 Count)",
                Description = "Assorted flavors, compatible with Keurig K-Cup brewers.",
                NewPrice = 22.99m, OldPrice = 27.99m,
                CategoryId = CatId("Coffee & Tea")
            },

            // grocery › pantry staples 
            new()
            {
                Name = "Barilla Spaghetti Pasta 16oz (6-Pack)",
                Description = "Classic Italian pasta, non-GMO, durum wheat semolina.",
                NewPrice = 8.99m, OldPrice = 11.49m,
                CategoryId = CatId("Pantry Staples")
            },

            // grocery › household essentials 
            new()
            {
                Name = "Bounty Select-A-Size Paper Towels (12 Rolls)",
                Description = "2-ply, 135 sheets per roll, quick-absorbing.",
                NewPrice = 21.99m, OldPrice = 26.99m,
                CategoryId = CatId("Household Essentials")
            },
            new()
            {
                Name = "Dawn Ultra Dish Soap (2-Pack, 19.4 oz)",
                Description = "Original scent, 2x more grease-cleaning power.",
                NewPrice = 7.99m, OldPrice = 9.99m,
                CategoryId = CatId("Household Essentials")
            },
        };

        db.Products.AddRange(products);
        await db.SaveChangesAsync();
    }
}