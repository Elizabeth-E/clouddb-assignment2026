using Microsoft.Extensions.Configuration;

namespace BuyMyHouse.Api.Services
{
    public class OfferDocumentUrlService : IOfferDocumentUrlService
    {
        private readonly string? _baseUrl;
        private readonly string _container;

        public OfferDocumentUrlService(IConfiguration configuration)
        {
            _baseUrl = configuration["Blob:BaseUrl"];
            _container = configuration["Blob:OfferDocumentsContainer"] ?? "mortgage-offers";
        }

        public string GetOfferDocumentUrl(string blobKey)
        {
            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                return $"/{_container}/{blobKey.TrimStart('/')}";
            }

            return $"{_baseUrl.TrimEnd('/')}/{_container}/{blobKey.TrimStart('/')}";
        }
    }
}
