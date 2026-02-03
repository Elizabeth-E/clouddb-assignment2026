using System;
using Shared.Models.Dto;
using Shared.Models.Entities;

namespace BuyMyHouse.Api.Mappings
{
    public static class IncomeRecordMapper
    {
        public static IncomeRecordDto ToDto(IncomeRecordEntity entity)
        {
            return new IncomeRecordDto
            {
                IncomeRecordId = entity.IncomeRecordId,
                ApplicantId = entity.ApplicantId,
                EmployerName = entity.EmployerName,
                GrossAnnualIncome = entity.GrossAnnualIncome,
                NetMonthlyIncome = entity.NetMonthlyIncome,
                OtherIncomeAnnual = entity.OtherIncomeAnnual,
                MonthlyDebtPayments = entity.MonthlyDebtPayments,
                RecordedAtUtc = entity.RecordedAtUtc
            };
        }

        public static IncomeRecordEntity ToEntity(
            CreateIncomeRecordRequestDto dto,
            Guid incomeRecordId,
            Guid applicantId,
            DateTime recordedAtUtc)
        {
            return new IncomeRecordEntity
            {
                IncomeRecordId = incomeRecordId,
                ApplicantId = applicantId,
                EmployerName = dto.EmployerName,
                GrossAnnualIncome = dto.GrossAnnualIncome,
                NetMonthlyIncome = dto.NetMonthlyIncome,
                OtherIncomeAnnual = dto.OtherIncomeAnnual,
                MonthlyDebtPayments = dto.MonthlyDebtPayments,
                RecordedAtUtc = recordedAtUtc
            };
        }
    }
}
