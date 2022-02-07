namespace Books.Utilities
{
    public class GoogleSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string project_id { get; set; } = string.Empty;
        public string auth_uri { get; set; } = string.Empty;
        public string token_uri { get; set; } = string.Empty;
        public string auth_provider_x509_cert_url { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string[]? redirect_uris { get; set; }
    }
}
