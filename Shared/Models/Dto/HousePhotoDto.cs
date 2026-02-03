using System;

namespace Shared.Models.Dto
{
    public class HousePhotoDto
    {
        public Guid PhotoId { get; set; }
        
        // SAS URL or routed API endpoint
        public string Url { get; set; } = string.Empty;
    }
}
