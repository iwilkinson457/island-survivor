namespace PepCare.Shopify.Services;

/// <summary>
/// Configuration loaded from pepcare-shopify.env / environment variables,
/// with optional fallback import from C:\ClareDocuments\shopify.txt.
/// Secrets are never committed.
/// </summary>
public record ShopifyConfig(
    string ShopDomain,
    string ClientId,
    string ClientSecret,
    string ApiVersion,
    string TokenCachePath
)
{
    public string BaseUrl => $"https://{ShopDomain}/admin/api/{ApiVersion}";
    public string TokenEndpoint => $"https://{ShopDomain}/admin/oauth/access_token";

    public static ShopifyConfig Load(string? envFilePath = null)
    {
        if (envFilePath != null && File.Exists(envFilePath))
            LoadEnvFile(envFilePath);

        var localEnv = Path.Combine(AppContext.BaseDirectory, "pepcare-shopify.env");
        if (File.Exists(localEnv))
            LoadEnvFile(localEnv);

        var workspaceEnv = @"C:\Users\Ian\.openclaw\workspace\pepcare-shopify\pepcare-shopify.env";
        if (File.Exists(workspaceEnv))
            LoadEnvFile(workspaceEnv);

        ImportLegacyCredentialFileIfPresent(@"C:\ClareDocuments\shopify.txt");

        var domain = Env("SHOPIFY_SHOP_DOMAIN") ?? "pepcare-lab.myshopify.com";
        var clientId = Env("SHOPIFY_CLIENT_ID")
            ?? throw new InvalidOperationException("SHOPIFY_CLIENT_ID not set. See pepcare-shopify.env.");
        var clientSecret = Env("SHOPIFY_CLIENT_SECRET")
            ?? throw new InvalidOperationException("SHOPIFY_CLIENT_SECRET not set. See pepcare-shopify.env.");
        var version = Env("SHOPIFY_API_VERSION") ?? "2024-10";

        var cacheDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PepCare.Shopify");
        Directory.CreateDirectory(cacheDir);

        return new ShopifyConfig(
            domain,
            clientId,
            clientSecret,
            version,
            Path.Combine(cacheDir, "token-cache.json"));
    }

    private static void ImportLegacyCredentialFileIfPresent(string path)
    {
        if (!File.Exists(path)) return;
        if (!string.IsNullOrWhiteSpace(Env("SHOPIFY_CLIENT_ID")) &&
            !string.IsNullOrWhiteSpace(Env("SHOPIFY_CLIENT_SECRET")))
            return;

        var lines = File.ReadAllLines(path)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        for (var i = 0; i < lines.Count - 1; i++)
        {
            if (lines[i].Equals("Client ID", StringComparison.OrdinalIgnoreCase))
                Environment.SetEnvironmentVariable("SHOPIFY_CLIENT_ID", lines[i + 1]);

            if (lines[i].Equals("Secret", StringComparison.OrdinalIgnoreCase))
                Environment.SetEnvironmentVariable("SHOPIFY_CLIENT_SECRET", lines[i + 1]);
        }
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

    private static string? Env(string key) => Environment.GetEnvironmentVariable(key);
}
