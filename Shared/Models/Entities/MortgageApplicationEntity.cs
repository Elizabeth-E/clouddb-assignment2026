using System;
using Shared.Models.Enums;

namespace Shared.Models.Entities
{
    public class MortgageApplicationEntity
    {
        public Guid ApplicationId { get; set; }

        public Guid ApplicantId { get; set; }
        public Guid? HouseId { get; set; }

        public Guid IncomeRecordId { get; set; }

        public decimal RequestedLoanAmount { get; set; }
        public int DesiredTermYears { get; set; }

        public decimal? Interest { get; set; }

        public decimal? DownPayment { get; set; }

        public bool HasPartner { get; set; }
        public decimal? PartnerIncomeAnnual { get; set; }

        public decimal? CurrentRentOrMortgageMonthly { get; set; }

        public ApplicationStatus Status { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? SubmittedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
