using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Models.Entities;
using Shared.Models.Dto;

namespace BuyMyHouse.Api.Mappings
{
    public static class HouseMapper
    {
        public static HouseSummaryDto ToSummaryDto(
            HouseEntity house,
            string? primaryPhotoUrl)
        {
            return new HouseSummaryDto
            {
                HouseId = house.HouseId,
                City = house.City,
                AskingPrice = house.AskingPrice,
                Bedrooms = house.Bedrooms,
                Bathrooms = house.Bathrooms,
                LivingAreaM2 = house.LivingAreaM2,
                PrimaryPhotoUrl = primaryPhotoUrl,
                Status = house.Status.ToString()
            };
        }

        public static HouseDetailDto ToDetailDto(
            HouseEntity house,
            List<HousePhotoDto> photos)
        {
            return new HouseDetailDto
            {
                HouseId = house.HouseId,
                AddressLine1 = house.AddressLine1,
                PostalCode = house.PostalCode,
                City = house.City,
                AskingPrice = house.AskingPrice,
                Bedrooms = house.Bedrooms,
                Bathrooms = house.Bathrooms,
                LivingAreaM2 = house.LivingAreaM2,
                PlotSizeM2 = house.PlotSizeM2,
                YearBuilt = house.YearBuilt,
                EnergyLabel = house.EnergyLabel,
                Description = house.Description,
                Status = house.Status.ToString(),
                Photos = photos
            };
        }

        public static HousePhotoDto ToPhotoDto(Guid photoId, bool isPrimary, int sortOrder, string url)
        {
            return new HousePhotoDto
            {
                PhotoId = photoId,
                Url = url
            };
        }
    }
}
