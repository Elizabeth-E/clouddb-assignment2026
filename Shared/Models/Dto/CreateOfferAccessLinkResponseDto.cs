using System;

namespace Shared.Models.Dto
{
    public class CreateOfferAccessLinkResponseDto
    {
        public Guid OfferId { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        public string AccessUrl { get; set; } = string.Empty;
    }
}
