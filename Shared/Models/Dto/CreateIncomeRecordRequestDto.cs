using Shared.Models.Enums;

namespace Shared.Models.Dto
{
    public class CreateIncomeRecordRequestDto
    {

        public string? EmployerName { get; set; }

        public decimal GrossAnnualIncome { get; set; }
        public decimal? NetMonthlyIncome { get; set; }

        public decimal? OtherIncomeAnnual { get; set; }
        public decimal? MonthlyDebtPayments { get; set; }
    }
}
