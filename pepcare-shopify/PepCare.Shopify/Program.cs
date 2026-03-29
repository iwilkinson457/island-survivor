using PepCare.Shopify.Cli;
using PepCare.Shopify.Services;
using System.Text.Json;

var cmd = args.Length > 0 ? args[0].ToLowerInvariant() : "help";

if (cmd is "help" or "--help" or "-h")
{
    PrintHelp();
    return;
}

ShopifyConfig config;
try
{
    config = ShopifyConfig.Load();
}
catch (InvalidOperationException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Config error: {ex.Message}");
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine("Create pepcare-shopify.env with:");
    Console.WriteLine("  SHOPIFY_SHOP_DOMAIN=pepcare-lab.myshopify.com");
    Console.WriteLine("  SHOPIFY_ACCESS_TOKEN=shpat_XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
    Console.WriteLine("  SHOPIFY_API_VERSION=2024-10    # optional");
    Environment.Exit(1);
    return;
}

var client = ShopifyClientFactory.Create(config);

try
{
    switch (cmd)
    {
        case "auth-check":
        case "shop":
            await CmdShop();
            break;
        case "products":
            await CmdProducts();
            break;
        case "product":
            await CmdProduct();
            break;
        case "collections":
            await CmdCollections();
            break;
        case "collection":
            await CmdCollection();
            break;
        case "pages":
            await CmdPages();
            break;
        case "page":
            await CmdPage();
            break;
        case "themes":
            await CmdThemes();
            break;
        case "blogs":
            await CmdBlogs();
            break;
        case "update-product":
            await CmdUpdateProduct();
            break;
        case "update-collection":
            await CmdUpdateCollection();
            break;
        case "update-page":
            await CmdUpdatePage();
            break;
        default:
            PrintHelp();
            break;
    }
}
catch (ShopifyApiException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.Message);
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine(ex.ResponseBody);
    Environment.ExitCode = 2;
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.Message);
    Console.ResetColor();
    Environment.ExitCode = 3;
}

async Task CmdShop()
{
    Console.WriteLine("Checking auth against Shopify Admin API...");
    var shop = await client.GetShopAsync();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"✓ Authenticated to: {shop.Name}");
    Console.ResetColor();
    Console.WriteLine($"  Domain       : {shop.Domain}");
    Console.WriteLine($"  Myshopify    : {shop.MyshopifyDomain}");
    Console.WriteLine($"  Email        : {shop.Email}");
    Console.WriteLine($"  Plan         : {shop.PlanName}");
    Console.WriteLine($"  Currency     : {shop.Currency}");
    Console.WriteLine($"  Timezone     : {shop.Timezone}");
    Console.WriteLine($"  Created      : {shop.CreatedAt:yyyy-MM-dd}");
    var count = await client.GetProductCountAsync();
    Console.WriteLine($"  Products     : {count}");
}

async Task CmdProducts()
{
    var limit = ParseArg<int>(args, "--limit") ?? 20;
    var status = ParseArgStr(args, "--status");
    var products = await client.GetProductsAsync(limit, status);
    Console.WriteLine($"Products ({products.Count}):");
    foreach (var p in products)
    {
        var price = p.Variants?.FirstOrDefault()?.Price ?? "—";
        Console.WriteLine($"  {p.Id,-14} [{p.Status,-8}] {p.Title}  (${price})");
    }
}

async Task CmdProduct()
{
    var id = ParseArg<long>(args, "--id")
        ?? (args.Length > 1 ? long.Parse(args[1]) : throw new ArgumentException("--id required"));
    var product = await client.GetProductAsync(id);
    PrintJson(product);
}

async Task CmdCollections()
{
    var collections = await client.GetCollectionsAsync();
    Console.WriteLine($"Custom Collections ({collections.Count}):");
    foreach (var c in collections)
        Console.WriteLine($"  {c.Id,-14} [{(c.Published ? "published" : "hidden "),-9}] {c.Title}");
}

async Task CmdCollection()
{
    var id = ParseArg<long>(args, "--id")
        ?? (args.Length > 1 ? long.Parse(args[1]) : throw new ArgumentException("--id required"));
    var col = await client.GetCollectionAsync(id);
    PrintJson(col);
}

async Task CmdPages()
{
    var pages = await client.GetPagesAsync();
    Console.WriteLine($"Pages ({pages.Count}):");
    foreach (var p in pages)
        Console.WriteLine($"  {p.Id,-14} [{(p.Published ? "published" : "hidden "),-9}] {p.Title}  (/{p.Handle})");
}

async Task CmdPage()
{
    var id = ParseArg<long>(args, "--id")
        ?? (args.Length > 1 ? long.Parse(args[1]) : throw new ArgumentException("--id required"));
    var page = await client.GetPageAsync(id);
    PrintJson(page);
}

async Task CmdThemes()
{
    var themes = await client.GetThemesAsync();
    Console.WriteLine($"Themes ({themes.Count}):");
    foreach (var t in themes)
    {
        var active = t.Role == "main" ? " ← ACTIVE" : string.Empty;
        Console.WriteLine($"  {t.Id,-14} [{t.Role,-12}] {t.Name}{active}");
    }
}

async Task CmdBlogs()
{
    var blogs = await client.GetBlogsAsync();
    Console.WriteLine($"Blogs ({blogs.Count}):");
    foreach (var b in blogs)
        Console.WriteLine($"  {b.Id,-14} {b.Title}  (/{b.Handle})");
}

async Task CmdUpdateProduct()
{
    var id = ParseArg<long>(args, "--id") ?? throw new ArgumentException("--id required");
    var title = ParseArgStr(args, "--title");
    var body = ParseArgStr(args, "--body");
    var tags = ParseArgStr(args, "--tags");
    var status = ParseArgStr(args, "--status");

    var current = await client.GetProductAsync(id);
    Console.WriteLine($"\nCurrent product: {current.Title} (status: {current.Status})");
    if (title != null) Console.WriteLine($"  New title  : {title}");
    if (body != null) Console.WriteLine($"  New body   : {Preview(body)}");
    if (tags != null) Console.WriteLine($"  New tags   : {tags}");
    if (status != null) Console.WriteLine($"  New status : {status}");

    ConfirmationGuard.RequireConfirmOrAbort($"Write changes to product {id}?");

    var updated = await client.UpdateProductAsync(new(id, title, body, tags, status));
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"✓ Updated: {updated.Title} (updated_at: {updated.UpdatedAt:u})");
    Console.ResetColor();
}

async Task CmdUpdateCollection()
{
    var id = ParseArg<long>(args, "--id") ?? throw new ArgumentException("--id required");
    var title = ParseArgStr(args, "--title");
    var body = ParseArgStr(args, "--body");

    var current = await client.GetCollectionAsync(id);
    Console.WriteLine($"\nCurrent collection: {current.Title}");
    if (title != null) Console.WriteLine($"  New title : {title}");
    if (body != null) Console.WriteLine($"  New body  : {Preview(body)}");

    ConfirmationGuard.RequireConfirmOrAbort($"Write changes to collection {id}?");

    var updated = await client.UpdateCollectionAsync(new(id, title, body));
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"✓ Updated: {updated.Title}");
    Console.ResetColor();
}

async Task CmdUpdatePage()
{
    var id = ParseArg<long>(args, "--id") ?? throw new ArgumentException("--id required");
    var title = ParseArgStr(args, "--title");
    var body = ParseArgStr(args, "--body");

    var current = await client.GetPageAsync(id);
    Console.WriteLine($"\nCurrent page: {current.Title} (/{current.Handle})");
    if (title != null) Console.WriteLine($"  New title : {title}");
    if (body != null) Console.WriteLine($"  New body  : {Preview(body)}");

    ConfirmationGuard.RequireConfirmOrAbort($"Write changes to page {id}?");

    var updated = await client.UpdatePageAsync(new(id, title, body));
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"✓ Updated: {updated.Title}");
    Console.ResetColor();
}

static T? ParseArg<T>(string[] args, string flag) where T : struct
{
    for (int i = 0; i < args.Length - 1; i++)
        if (args[i] == flag)
            return (T)Convert.ChangeType(args[i + 1], typeof(T));
    return null;
}

static string? ParseArgStr(string[] args, string flag)
{
    for (int i = 0; i < args.Length - 1; i++)
        if (args[i] == flag)
            return args[i + 1];
    return null;
}

static string Preview(string text) => text.Length <= 80 ? text : text[..80] + "...";

static void PrintJson(object obj) =>
    Console.WriteLine(JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true }));

static void PrintHelp()
{
    Console.WriteLine("""
PepCare Shopify CLI — Admin API connector
==========================================

READ OPERATIONS (safe, no confirmation needed):
  shop / auth-check          Verify auth and show store info
  products [--limit N]       List products (default 20)
                             [--status active|draft|archived]
  product --id <id>          Show full product detail (JSON)
  collections                List custom collections
  collection --id <id>       Show full collection detail (JSON)
  pages                      List store pages
  page --id <id>             Show full page detail (JSON)
  themes                     List installed themes (active marked)
  blogs                      List blogs

WRITE OPERATIONS (requires y/N confirmation):
  update-product --id <id>
    [--title "New Title"]
    [--body "<p>HTML body</p>"]
    [--tags "tag1,tag2"]
    [--status active|draft|archived]

  update-collection --id <id>
    [--title "New Title"]
    [--body "<p>HTML body</p>"]

  update-page --id <id>
    [--title "New Title"]
    [--body "<p>HTML body</p>"]

CREDENTIALS:
  Create pepcare-shopify.env in the workspace root or beside the exe:
    SHOPIFY_SHOP_DOMAIN=pepcare-lab.myshopify.com
    SHOPIFY_ACCESS_TOKEN=shpat_XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    SHOPIFY_API_VERSION=2024-10    # optional

  ⚠ Do NOT commit pepcare-shopify.env (it is in .gitignore).
  Note: a shpss_ value is the API secret key, not the Admin API access token.
""");
}
