namespace Books.Domain.Utilities
{
    public class StripeSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string PublishableKey { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
