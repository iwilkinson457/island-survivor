using System.Net.Http;
using System.Text;
using System.Text.Json;
using PepCare.Shopify.Models;

namespace PepCare.Shopify.Services;

/// <summary>
/// Thin Shopify Admin REST API client.
/// Supports read operations and guarded write operations for website/store management.
/// </summary>
public class ShopifyClient(HttpClient http, ShopifyConfig config)
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private async Task<T> GetJsonAsync<T>(string url)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        using var resp = await http.SendAsync(req);
        var body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new ShopifyApiException((int)resp.StatusCode, body, $"Shopify GET failed: {(int)resp.StatusCode} {resp.ReasonPhrase}");

        return JsonSerializer.Deserialize<T>(body, Json)
            ?? throw new InvalidOperationException($"Empty JSON response from {url}");
    }

    private async Task<T> PutJsonAsync<T>(string url, object payload)
    {
        var json = JsonSerializer.Serialize(payload, Json);
        using var req = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        using var resp = await http.SendAsync(req);
        var body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new ShopifyApiException((int)resp.StatusCode, body, $"Shopify PUT failed: {(int)resp.StatusCode} {resp.ReasonPhrase}");

        return JsonSerializer.Deserialize<T>(body, Json)
            ?? throw new InvalidOperationException($"Empty JSON response from {url}");
    }

    // ── Auth verify ──────────────────────────────────────────────────────────
    public async Task<ShopInfo> GetShopAsync()
    {
        var r = await GetJsonAsync<ShopRoot>($"{config.BaseUrl}/shop.json");
        return r.Shop;
    }

    // ── Products ─────────────────────────────────────────────────────────────
    public async Task<List<Product>> GetProductsAsync(int limit = 50, string? status = null)
    {
        var url = $"{config.BaseUrl}/products.json?limit={limit}";
        if (status != null) url += $"&status={status}";
        var r = await GetJsonAsync<ProductListRoot>(url);
        return r.Products ?? [];
    }

    public async Task<Product> GetProductAsync(long id)
    {
        var r = await GetJsonAsync<ProductRoot>($"{config.BaseUrl}/products/{id}.json");
        return r.Product;
    }

    public async Task<Product> UpdateProductAsync(ProductWriteFields fields)
    {
        var payload = new ProductUpdatePayload(fields);
        var r = await PutJsonAsync<ProductRoot>($"{config.BaseUrl}/products/{fields.Id}.json", payload);
        return r.Product;
    }

    // ── Collections ──────────────────────────────────────────────────────────
    public async Task<List<Collection>> GetCollectionsAsync(int limit = 50)
    {
        var r = await GetJsonAsync<CollectionListRoot>($"{config.BaseUrl}/custom_collections.json?limit={limit}");
        return r.Collections ?? [];
    }

    public async Task<Collection> GetCollectionAsync(long id)
    {
        var r = await GetJsonAsync<CollectionRoot>($"{config.BaseUrl}/custom_collections/{id}.json");
        return r.Collection;
    }

    public async Task<Collection> UpdateCollectionAsync(CollectionWriteFields fields)
    {
        var payload = new CollectionUpdatePayload(fields);
        var r = await PutJsonAsync<CollectionRoot>($"{config.BaseUrl}/custom_collections/{fields.Id}.json", payload);
        return r.Collection;
    }

    // ── Pages ─────────────────────────────────────────────────────────────────
    public async Task<List<StorePage>> GetPagesAsync(int limit = 50)
    {
        var r = await GetJsonAsync<PageListRoot>($"{config.BaseUrl}/pages.json?limit={limit}");
        return r.Pages ?? [];
    }

    public async Task<StorePage> GetPageAsync(long id)
    {
        var r = await GetJsonAsync<StorePageRoot>($"{config.BaseUrl}/pages/{id}.json");
        return r.Page;
    }

    public async Task<StorePage> UpdatePageAsync(PageWriteFields fields)
    {
        var payload = new PageUpdatePayload(fields);
        var r = await PutJsonAsync<StorePageRoot>($"{config.BaseUrl}/pages/{fields.Id}.json", payload);
        return r.Page;
    }

    // ── Themes ────────────────────────────────────────────────────────────────
    public async Task<List<Theme>> GetThemesAsync()
    {
        var r = await GetJsonAsync<ThemeListRoot>($"{config.BaseUrl}/themes.json");
        return r.Themes ?? [];
    }

    // ── Blogs ─────────────────────────────────────────────────────────────────
    public async Task<List<Blog>> GetBlogsAsync()
    {
        var r = await GetJsonAsync<BlogListRoot>($"{config.BaseUrl}/blogs.json");
        return r.Blogs ?? [];
    }

    // ── Product count ─────────────────────────────────────────────────────────
    public async Task<int> GetProductCountAsync()
    {
        var r = await GetJsonAsync<JsonElement>($"{config.BaseUrl}/products/count.json");
        return r.TryGetProperty("count", out var c) ? c.GetInt32() : 0;
    }
}
