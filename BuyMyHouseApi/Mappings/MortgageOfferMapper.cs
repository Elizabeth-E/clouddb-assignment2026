using Shared.Models.Dto;
using Shared.Models.Entities;

namespace BuyMyHouse.Api.Mappings
{
    public static class MortgageOfferMapper
    {
        public static MortgageOfferDto ToDto(MortgageOfferEntity entity, string? documentDownloadUrl)
        {
            return new MortgageOfferDto
            {
                OfferId = entity.OfferId,
                ApplicationId = entity.ApplicationId,
                GeneratedAtUtc = entity.GeneratedAtUtc,
                ValidUntilUtc = entity.ValidUntilUtc,
                Decision = entity.Decision,
                ApprovedLoanAmount = entity.ApprovedLoanAmount,
                InterestRate = entity.InterestRate,
                MonthlyPayment = entity.MonthlyPayment,
                TermYears = entity.TermYears,
                Notes = entity.Notes,
                DocumentDownloadUrl = documentDownloadUrl
            };
        }

        public static OfferViewDto ToViewDto(MortgageOfferEntity entity, string? documentInlineUrl)
        {
            return new OfferViewDto
            {
                OfferId = entity.OfferId,
                ApplicationId = entity.ApplicationId,
                GeneratedAtUtc = entity.GeneratedAtUtc,
                ValidUntilUtc = entity.ValidUntilUtc,
                Decision = entity.Decision,
                ApprovedLoanAmount = entity.ApprovedLoanAmount,
                InterestRate = entity.InterestRate,
                MonthlyPayment = entity.MonthlyPayment,
                TermYears = entity.TermYears,
                Notes = entity.Notes,
                DocumentInlineUrl = documentInlineUrl
            };
        }
    }
}
