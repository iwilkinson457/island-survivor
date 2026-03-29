# PepCare Shopify Connector

A small .NET 9 CLI connector for real Shopify Admin API operations against the PepCare store.

## What it does

Read-first operations:
- verify store auth / read shop info
- list and inspect products
- list and inspect custom collections
- list and inspect pages
- list themes
- list blogs

Controlled write operations:
- update product title / HTML body / tags / status
- update custom collection title / HTML body
- update page title / HTML body

All write operations require an interactive confirmation prompt.

## Important auth note

The credentials currently available from `C:\ClareDocuments\shopify.txt` are:
- API key / client ID
- API secret key

For Shopify **custom apps**, those are **not enough by themselves** for Admin API calls.

You need the app's **Admin API access token**, which normally starts with `shpat_`.

A value starting with `shpss_` is the app secret key, **not** the Admin API token.

## Config

Copy the example file:

```powershell
Copy-Item .\pepcare-shopify.env.example .\pepcare-shopify.env
```

Then fill in:

```env
SHOPIFY_SHOP_DOMAIN=pepcare-lab.myshopify.com
SHOPIFY_ACCESS_TOKEN=shpat_XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
SHOPIFY_API_VERSION=2024-10
```

`pepcare-shopify.env` is gitignored.

## Build

```powershell
cd C:\Users\Ian\.openclaw\workspace\pepcare-shopify
 dotnet build .\PepCare.Shopify.sln
```

## Run

```powershell
cd C:\Users\Ian\.openclaw\workspace\pepcare-shopify
 dotnet run --project .\PepCare.Shopify\PepCare.Shopify.csproj -- auth-check
```

## Examples

### Verify auth / store info

```powershell
 dotnet run --project .\PepCare.Shopify\PepCare.Shopify.csproj -- auth-check
```

### List products

```powershell
 dotnet run --project .\PepCare.Shopify\PepCare.Shopify.csproj -- products --limit 10
```

### Inspect one product

```powershell
 dotnet run --project .\PepCare.Shopify\PepCare.Shopify.csproj -- product --id 1234567890
```

### Update product copy

```powershell
 dotnet run --project .\PepCare.Shopify\PepCare.Shopify.csproj -- update-product --id 1234567890 --title "New title" --body "<p>Updated copy</p>"
```

### List collections

```powershell
 dotnet run --project .\PepCare.Shopify\PepCare.Shopify.csproj -- collections
```

### Update collection copy

```powershell
 dotnet run --project .\PepCare.Shopify\PepCare.Shopify.csproj -- update-collection --id 1234567890 --title "New collection title"
```

### List pages

```powershell
 dotnet run --project .\PepCare.Shopify\PepCare.Shopify.csproj -- pages
```

### Update page content

```powershell
 dotnet run --project .\PepCare.Shopify\PepCare.Shopify.csproj -- update-page --id 1234567890 --body "<p>Updated page content</p>"
```

## Practical website-management support

Supported now:
- shop / store metadata
- products
- custom collections
- online store pages
- themes (read only)
- blogs (read only)

Not implemented yet:
- theme asset editing
- navigation / menus
- metafields
- orders / customers
- GraphQL operations
- publish / deploy workflow around theme changes

## Realistic limitation

Without the `shpat_...` Admin API access token, the connector can build and run, but real authenticated store operations will fail with `401 Unauthorized`.

If PepCare only has the API key and secret, the missing step is to retrieve or regenerate the Admin API access token from the installed custom app in Shopify Admin.
