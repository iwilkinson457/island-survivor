namespace PepCare.Shopify.Services;

public sealed class ShopifyApiException : Exception
{
    public int StatusCode { get; }
    public string ResponseBody { get; }

    public ShopifyApiException(int statusCode, string responseBody, string message)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}
