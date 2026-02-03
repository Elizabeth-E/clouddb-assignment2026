using System;

namespace BuyMyHouse.Api.Services
{
    public interface IPhotoUrlService
    {
        // Returns either a SAS URL to blob storage or an API route that serves the photo
        string GetHousePhotoUrl(Guid houseId, Guid photoId);
    }
}
