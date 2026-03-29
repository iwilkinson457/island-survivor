using PepCare.Shopify.Models;

namespace PepCare.Shopify.Services;

public static class ShopifyClientFactory
{
    public static ShopifyClient Create(ShopifyConfig config, TokenCacheRecord token)
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.Add("X-Shopify-Access-Token", token.AccessToken);
        http.DefaultRequestHeaders.Add("Accept", "application/json");
        http.DefaultRequestHeaders.UserAgent.ParseAdd("PepCare-Shopify/1.0");

        return new ShopifyClient(http, config);
    }
}
