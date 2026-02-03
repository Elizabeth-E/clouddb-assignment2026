using System;
using Shared.Models.Dto;
using Shared.Models.Entities;

namespace BuyMyHouse.Api.Mappings
{
    public static class ApplicantMapper
    {
        public static ApplicantDto ToDto(ApplicantEntity entity)
        {
            return new ApplicantDto
            {
                ApplicantId = entity.ApplicantId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                Phone = entity.Phone,
                DateOfBirth = entity.DateOfBirth,
                CreatedAtUtc = entity.CreatedAtUtc
            };
        }

        public static ApplicantEntity ToEntity(CreateApplicantRequestDto dto, Guid applicantId, DateTime createdAtUtc)
        {
            return new ApplicantEntity
            {
                ApplicantId = applicantId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                CreatedAtUtc = createdAtUtc
            };
        }
    }
}
