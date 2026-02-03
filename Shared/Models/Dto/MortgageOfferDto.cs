using System;
using Shared.Models.Enums;

namespace Shared.Models.Dto
{
    public class MortgageOfferDto
    {
        public Guid OfferId { get; set; }
        public Guid ApplicationId { get; set; }

        public DateTime GeneratedAtUtc { get; set; }
        public DateTime ValidUntilUtc { get; set; }

        public OfferDecision Decision { get; set; }

        public decimal? ApprovedLoanAmount { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? MonthlyPayment { get; set; }
        public int? TermYears { get; set; }

        public string? Notes { get; set; }

        public string? DocumentDownloadUrl { get; set; }
    }
}
