using System;

namespace Shared.Models.Dto
{
    public class ApplicantDto
    {
        public Guid ApplicantId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}