using System;

namespace Shared.Models.Dto
{
    public class IncomeRecordDto
    {
        public Guid IncomeRecordId { get; set; }
        public Guid ApplicantId { get; set; }

        public string? EmployerName { get; set; }

        public decimal GrossAnnualIncome { get; set; }
        public decimal? NetMonthlyIncome { get; set; }

        public decimal? OtherIncomeAnnual { get; set; }
        public decimal? MonthlyDebtPayments { get; set; }

        public DateTime RecordedAtUtc { get; set; }
    }
}
