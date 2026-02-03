using System;
using Shared.Models.Enums;

namespace Shared.Models.Dto
{
    public class BatchRunDto
    {
        public Guid BatchRunId { get; set; }

        public DateTime RunDateUtc { get; set; }

        public DateTime StartedAtUtc { get; set; }
        public DateTime? FinishedAtUtc { get; set; }

        public BatchStatus Status { get; set; }

        public int ApplicationsProcessed { get; set; }
        public int OffersGenerated { get; set; }
        public int EmailsQueued { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
