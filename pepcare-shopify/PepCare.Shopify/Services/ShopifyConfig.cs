namespace PepCare.Shopify.Services;

/// <summary>
/// Configuration loaded from pepcare-shopify.env or environment variables.
/// Never hard-code tokens here.
/// </summary>
public record ShopifyConfig(
    string ShopDomain,        // e.g. pepcare-lab.myshopify.com
    string AccessToken,       // shpat_... (Admin API access token)
    string ApiVersion         // e.g. 2024-10
)
{
    public string BaseUrl => $"https://{ShopDomain}/admin/api/{ApiVersion}";

    public static ShopifyConfig Load(string? envFilePath = null)
    {
        // 1. Try explicit env file path
        if (envFilePath != null && File.Exists(envFilePath))
            LoadEnvFile(envFilePath);

        // 2. Try local pepcare-shopify.env beside the exe
        var localEnv = Path.Combine(
            AppContext.BaseDirectory, "pepcare-shopify.env");
        if (File.Exists(localEnv))
            LoadEnvFile(localEnv);

        // 3. Try workspace root
        var workspaceEnv = @"C:\Users\Ian\.openclaw\workspace\pepcare-shopify\pepcare-shopify.env";
        if (File.Exists(workspaceEnv))
            LoadEnvFile(workspaceEnv);

        var domain = Env("SHOPIFY_SHOP_DOMAIN")
            ?? throw new InvalidOperationException("SHOPIFY_SHOP_DOMAIN not set. See pepcare-shopify.env.");
        var token = Env("SHOPIFY_ACCESS_TOKEN")
            ?? throw new InvalidOperationException("SHOPIFY_ACCESS_TOKEN not set. See pepcare-shopify.env.");
        var version = Env("SHOPIFY_API_VERSION") ?? "2024-10";

        return new ShopifyConfig(domain, token, version);
    }

    private static void LoadEnvFile(string path)
    {
        foreach (var line in File.ReadAllLines(path))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith('#') || !trimmed.Contains('=')) continue;
            var idx = trimmed.IndexOf('=');
            var key = trimmed[..idx].Trim();
            var val = trimmed[(idx + 1)..].Trim().Trim('"').Trim('\'');
            if (!string.IsNullOrEmpty(key))
                Environment.SetEnvironmentVariable(key, val);
        }
    }

    private static string? Env(string key) =>
        Environment.GetEnvironmentVariable(key);
}
