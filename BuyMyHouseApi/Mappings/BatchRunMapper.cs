using Shared.Models.Dto;
using Shared.Models.Entities;

namespace BuyMyHouse.Api.Mappings
{
    public static class BatchRunMapper
    {
        public static BatchRunDto ToDto(BatchRunEntity entity)
        {
            return new BatchRunDto
            {
                BatchRunId = entity.BatchRunId,
                RunDateUtc = entity.RunDateUtc,
                StartedAtUtc = entity.StartedAtUtc,
                FinishedAtUtc = entity.FinishedAtUtc,
                Status = entity.Status,
                ApplicationsProcessed = entity.ApplicationsProcessed,
                OffersGenerated = entity.OffersGenerated,
                EmailsQueued = entity.EmailsQueued,
                ErrorMessage = entity.ErrorMessage
            };
        }
    }
}
