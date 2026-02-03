using System;

namespace Shared.Models.Entities
{
    public class OfferAccessTokenEntity
    {
        public Guid TokenId { get; set; }

        public Guid OfferId { get; set; }

        public string TokenHash { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }
        public DateTime ExpiresAtUtc { get; set; }

        public DateTime? RevokedAtUtc { get; set; }
        public DateTime? UsedAtUtc { get; set; }
    }
}
