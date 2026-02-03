using System;
using Shared.Models.Enums;

namespace Shared.Models.Entities
{
    public class MortgageOfferEntity
    {
        public Guid OfferId { get; set; }

        public Guid ApplicationId { get; set; }
        public Guid ApplicantId { get; set; }

        public OfferDecision Decision { get; set; }
        public DateTime GeneratedAtUtc { get; set; }
        public DateTime ValidUntilUtc { get; set; }

        public decimal? ApprovedLoanAmount { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? MonthlyPayment { get; set; }
        public int? TermYears { get; set; }

        public string? Notes { get; set; }

        /*
         * Azure Blob Storage design:
         * Container: mortgage-offers
         * Blob name example:
         * offers/{applicantId}/{applicationId}/{offerId}.pdf
         */
        public OfferDocumentFormat DocumentFormat { get; set; }
        public string DocumentBlobKey { get; set; } = string.Empty;

    }
}
