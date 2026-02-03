using System;
using Shared.Models.Dto;
using Shared.Models.Entities;
using Shared.Models.Enums;

namespace BuyMyHouse.Api.Mappings
{
    public static class MortgageApplicationMapper
    {
        public static MortgageApplicationDto ToDto(MortgageApplicationEntity entity)
        {
            return new MortgageApplicationDto
            {
                ApplicationId = entity.ApplicationId,
                ApplicantId = entity.ApplicantId,
                HouseId = entity.HouseId,
                RequestedLoanAmount = entity.RequestedLoanAmount,
                DesiredTermYears = entity.DesiredTermYears,
                Interest = entity.Interest,
                DownPayment = entity.DownPayment,
                HasPartner = entity.HasPartner,
                PartnerIncomeAnnual = entity.PartnerIncomeAnnual,
                CurrentRentOrMortgageMonthly = entity.CurrentRentOrMortgageMonthly,
                Status = entity.Status,
                CreatedAtUtc = entity.CreatedAtUtc,
                SubmittedAtUtc = entity.SubmittedAtUtc,
                UpdatedAtUtc = entity.UpdatedAtUtc
            };
        }

        public static MortgageApplicationEntity ToEntity(
            CreateMortgageApplicationRequestDto dto,
            Guid applicationId,
            Guid incomeRecordId,
            DateTime createdAtUtc)
        {
            return new MortgageApplicationEntity
            {
                ApplicationId = applicationId,
                ApplicantId = dto.ApplicantId,
                HouseId = dto.HouseId,
                IncomeRecordId = incomeRecordId,
                RequestedLoanAmount = dto.RequestedLoanAmount,
                DesiredTermYears = dto.DesiredTermYears,
                Interest = dto.Interest,
                DownPayment = dto.DownPayment,
                HasPartner = dto.HasPartner,
                PartnerIncomeAnnual = dto.PartnerIncomeAnnual,
                CurrentRentOrMortgageMonthly = dto.CurrentRentOrMortgageMonthly,
                Status = ApplicationStatus.Submitted,
                CreatedAtUtc = createdAtUtc,
                SubmittedAtUtc = createdAtUtc
            };
        }

        public static void ApplyUpdate(
            MortgageApplicationEntity entity,
            UpdateMortgageApplicationRequestDto dto,
            DateTime updatedAtUtc)
        {
            entity.RequestedLoanAmount = dto.RequestedLoanAmount;
            entity.DesiredTermYears = dto.DesiredTermYears;
            entity.Interest = dto.Interest;
            entity.DownPayment = dto.DownPayment;
            entity.HasPartner = dto.HasPartner;
            entity.PartnerIncomeAnnual = dto.PartnerIncomeAnnual;
            entity.CurrentRentOrMortgageMonthly = dto.CurrentRentOrMortgageMonthly;
            entity.UpdatedAtUtc = updatedAtUtc;
        }
    }
}
