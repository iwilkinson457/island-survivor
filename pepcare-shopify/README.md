# PepCare Shopify Connector

A small .NET 9 CLI connector for practical Shopify Admin API operations against the PepCare store.

## Auth model

This connector uses the now-confirmed working flow:

```http
POST /admin/oauth/access_token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&client_id=...
&client_secret=...
```

The returned access token is then used as:

```http
X-Shopify-Access-Token: <minted token>
```

## What it does

Read-first operations:
- verify auth / read shop info / show granted scopes
- list and inspect products
- list and inspect custom collections
- list and inspect pages
- list themes
- list blogs
- show connector capability summary

Controlled write operations:
- update product title / HTML body / tags / status
- update custom collection title / HTML body
- update page title / HTML body

All write operations require an interactive confirmation prompt.

## Safe credential handling

Primary config source:
- `pepcare-shopify.env` in the workspace root or beside the exe

Supported values:

```env
SHOPIFY_SHOP_DOMAIN=pepcare-lab.myshopify.com
SHOPIFY_CLIENT_ID=...
SHOPIFY_CLIENT_SECRET=...
SHOPIFY_API_VERSION=2024-10
```

Legacy fallback:
- `C:\ClareDocuments\shopify.txt`

Token caching:
- minted access token is cached under `%LOCALAPPDATA%\PepCare.Shopify\token-cache.json`
- token cache is local-only and not committed to git

## Config

Copy the example file:

```powershell
Copy-Item .\pepcare-shopify.env.example .\pepcare-shopify.env
```

Then fill in your client credentials.

`pepcare-shopify.env` is gitignored.

## Build

```powershell
cd C:\Users\Ian\.openclaw\workspace\pepcare-shopify
 dotnet build .\PepCare.Shopify.sln -c Release
```

## Run

```powershell
cd C:\Users\Ian\.openclaw\workspace\pepcare-shopify
 .\pepcare-shopify.ps1 auth-check
```

## Examples

### Verify auth / scopes / shop info

```powershell
.\pepcare-shopify.ps1 auth-check
```

### Force refresh the minted token

```powershell
.\pepcare-shopify.ps1 auth-refresh
```

### Show supported operations

```powershell
.\pepcare-shopify.ps1 capabilities
```

### List products

```powershell
.\pepcare-shopify.ps1 products --limit 10
```

### Inspect one product

```powershell
.\pepcare-shopify.ps1 product --id 1234567890
```

### List collections

```powershell
.\pepcare-shopify.ps1 collections
```

### List pages

```powershell
.\pepcare-shopify.ps1 pages
```

### List themes

```powershell
.\pepcare-shopify.ps1 themes
```

### Update product copy

```powershell
.\pepcare-shopify.ps1 update-product --id 1234567890 --title "New title" --body "<p>Updated copy</p>"
```

### Update collection copy

```powershell
.\pepcare-shopify.ps1 update-collection --id 1234567890 --title "New collection title"
```

### Update page content

```powershell
.\pepcare-shopify.ps1 update-page --id 1234567890 --body "<p>Updated page content</p>"
```

## Practical website-management support

Supported now:
- shop / store metadata
- product discovery and controlled product content updates
- custom collection discovery and controlled copy updates
- page discovery and controlled page content updates
- theme discovery (list only)
- blog discovery (list only)
- granted-scope visibility via auth-check

Not implemented yet:
- theme asset editing
- menu/navigation editing
- metafields
- orders / customers
- GraphQL operations
- publish / deploy workflow around theme changes

## Guardrails

- write operations always prompt before sending
- token and credentials are not printed by the CLI
- token cache is local only
- connector stays read-first; theme modification is intentionally not enabled yet
