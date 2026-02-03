using System;

namespace Shared.Models.Dto
{
    public class HouseSummaryDto
    {
        public Guid HouseId { get; set; }

        public string City { get; set; } = string.Empty;

        public decimal AskingPrice { get; set; }

        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }

        public int LivingAreaM2 { get; set; }

        public string? PrimaryPhotoUrl { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
