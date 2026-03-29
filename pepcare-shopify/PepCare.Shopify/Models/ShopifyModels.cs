using System.Text.Json.Serialization;

namespace PepCare.Shopify.Models;

// ── OAuth / Auth ───────────────────────────────────────────────────────────
public record ClientCredentialsTokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("scope")] string? Scope,
    [property: JsonPropertyName("scopes")] string? Scopes
);

public record TokenCacheRecord(
    string ShopDomain,
    string ApiVersion,
    string AccessToken,
    string? Scope,
    DateTime RetrievedAtUtc
);

// ── Shop ────────────────────────────────────────────────────────────────────
public record ShopRoot([property: JsonPropertyName("shop")] ShopInfo Shop);
public record ShopInfo(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("domain")] string Domain,
    [property: JsonPropertyName("myshopify_domain")] string MyshopifyDomain,
    [property: JsonPropertyName("plan_name")] string PlanName,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("timezone")] string Timezone,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt
);

// ── Products ────────────────────────────────────────────────────────────────
public record ProductListRoot([property: JsonPropertyName("products")] List<Product> Products);
public record ProductRoot([property: JsonPropertyName("product")] Product Product);
public record Product(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("body_html")] string? BodyHtml,
    [property: JsonPropertyName("vendor")] string? Vendor,
    [property: JsonPropertyName("product_type")] string? ProductType,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("tags")] string? Tags,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTime UpdatedAt,
    [property: JsonPropertyName("variants")] List<ProductVariant>? Variants,
    [property: JsonPropertyName("images")] List<ProductImage>? Images
);
public record ProductVariant(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("price")] string? Price,
    [property: JsonPropertyName("sku")] string? Sku,
    [property: JsonPropertyName("inventory_quantity")] int? InventoryQuantity
);
public record ProductImage(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("src")] string? Src,
    [property: JsonPropertyName("alt")] string? Alt
);

// ── Collections ─────────────────────────────────────────────────────────────
public record CollectionListRoot([property: JsonPropertyName("custom_collections")] List<Collection> Collections);
public record CollectionRoot([property: JsonPropertyName("custom_collection")] Collection Collection);
public record Collection(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("body_html")] string? BodyHtml,
    [property: JsonPropertyName("handle")] string? Handle,
    [property: JsonPropertyName("published")] bool Published,
    [property: JsonPropertyName("updated_at")] DateTime? UpdatedAt
);

// ── Pages ───────────────────────────────────────────────────────────────────
public record PageListRoot([property: JsonPropertyName("pages")] List<StorePage> Pages);
public record StorePageRoot([property: JsonPropertyName("page")] StorePage Page);
public record StorePage(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("handle")] string? Handle,
    [property: JsonPropertyName("body_html")] string? BodyHtml,
    [property: JsonPropertyName("published")] bool Published,
    [property: JsonPropertyName("updated_at")] DateTime? UpdatedAt
);

// ── Theme ────────────────────────────────────────────────────────────────────
public record ThemeListRoot([property: JsonPropertyName("themes")] List<Theme> Themes);
public record Theme(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("updated_at")] DateTime? UpdatedAt
);

// ── Blog / Articles ──────────────────────────────────────────────────────────
public record BlogListRoot([property: JsonPropertyName("blogs")] List<Blog> Blogs);
public record Blog(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("handle")] string? Handle,
    [property: JsonPropertyName("updated_at")] DateTime? UpdatedAt
);

// ── Write payloads ───────────────────────────────────────────────────────────
public record ProductUpdatePayload(
    [property: JsonPropertyName("product")] ProductWriteFields Product
);
public record ProductWriteFields(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("body_html")] string? BodyHtml,
    [property: JsonPropertyName("tags")] string? Tags,
    [property: JsonPropertyName("status")] string? Status
);
public record CollectionUpdatePayload(
    [property: JsonPropertyName("custom_collection")] CollectionWriteFields Collection
);
public record CollectionWriteFields(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("body_html")] string? BodyHtml
);
public record PageUpdatePayload(
    [property: JsonPropertyName("page")] PageWriteFields Page
);
public record PageWriteFields(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("body_html")] string? BodyHtml
);
