using System;
using Shared.Models.Enums;

namespace Shared.Models.Dto
{
    public class CreateHouseRequestDto
    {
        public string AddressLine1 { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        public decimal AskingPrice { get; set; }

        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }

        public int LivingAreaM2 { get; set; }
        public int? PlotSizeM2 { get; set; }

        public int? YearBuilt { get; set; }
        public string? EnergyLabel { get; set; }

        public string? Description { get; set; }

        public HouseStatus Status { get; set; }
    }
}
