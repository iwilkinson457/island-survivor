using System.Net.Http;
using System.Text;
using System.Text.Json;
using PepCare.Shopify.Models;

namespace PepCare.Shopify.Services;

public class ShopifyAuthService(ShopifyConfig config)
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public async Task<TokenCacheRecord> GetAccessTokenAsync(bool forceRefresh = false)
    {
        if (!forceRefresh)
        {
            var cached = TryReadCache();
            if (cached is not null)
                return cached;
        }

        using var http = new HttpClient();
        using var req = new HttpRequestMessage(HttpMethod.Post, config.TokenEndpoint)
        {
            Content = new StringContent(
                $"grant_type=client_credentials&client_id={Uri.EscapeDataString(config.ClientId)}&client_secret={Uri.EscapeDataString(config.ClientSecret)}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded")
        };

        using var resp = await http.SendAsync(req);
        var body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new ShopifyApiException((int)resp.StatusCode, body, $"Shopify token request failed: {(int)resp.StatusCode} {resp.ReasonPhrase}");

        var token = JsonSerializer.Deserialize<ClientCredentialsTokenResponse>(body, Json)
            ?? throw new InvalidOperationException("Shopify token response was empty.");

        var scope = token.Scope ?? token.Scopes;
        var record = new TokenCacheRecord(
            config.ShopDomain,
            config.ApiVersion,
            token.AccessToken,
            scope,
            DateTime.UtcNow);

        WriteCache(record);
        return record;
    }

    private TokenCacheRecord? TryReadCache()
    {
        if (!File.Exists(config.TokenCachePath)) return null;
        try
        {
            var json = File.ReadAllText(config.TokenCachePath);
            var record = JsonSerializer.Deserialize<TokenCacheRecord>(json, Json);
            if (record is null) return null;
            if (!string.Equals(record.ShopDomain, config.ShopDomain, StringComparison.OrdinalIgnoreCase)) return null;
            if (!string.Equals(record.ApiVersion, config.ApiVersion, StringComparison.OrdinalIgnoreCase)) return null;
            if (string.IsNullOrWhiteSpace(record.AccessToken)) return null;
            return record;
        }
        catch
        {
            return null;
        }
    }

    private void WriteCache(TokenCacheRecord record)
    {
        var json = JsonSerializer.Serialize(record, Json);
        File.WriteAllText(config.TokenCachePath, json);
    }
}
