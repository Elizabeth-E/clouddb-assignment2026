using System;
using Microsoft.Extensions.Configuration;

namespace BuyMyHouse.Api.Services
{
    public class PhotoUrlService : IPhotoUrlService
    {
        private readonly string? _baseUrl;
        private readonly string _container;

        public PhotoUrlService(IConfiguration configuration)
        {
            _baseUrl = configuration["Blob:BaseUrl"];
            _container = configuration["Blob:HousePhotosContainer"] ?? "house-photos";
        }

        public string GetHousePhotoUrl(Guid houseId, Guid photoId)
        {
            var blobPath = $"houses/{houseId}/photos/{photoId}.jpg";

            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                return $"/{_container}/{blobPath}";
            }

            return $"{_baseUrl.TrimEnd('/')}/{_container}/{blobPath}";
        }
    }
}
