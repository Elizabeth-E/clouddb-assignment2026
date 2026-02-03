using Shared.Models.Enums;

namespace Shared.Models.Dto
{
    public class UpdateMortgageApplicationRequestDto
    {
        public decimal RequestedLoanAmount { get; set; }
        public int DesiredTermYears { get; set; }

        public decimal? Interest { get; set; }

        public decimal? DownPayment { get; set; }

        public bool HasPartner { get; set; }
        public decimal? PartnerIncomeAnnual { get; set; }

        public decimal? CurrentRentOrMortgageMonthly { get; set; }
    }
}
