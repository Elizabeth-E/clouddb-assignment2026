using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Functions.Services;

namespace Functions.Functions
{
    public class DailyBatchFunction
    {
        private readonly BatchProcessingService _batchProcessingService;
        private readonly ILogger<DailyBatchFunction> _logger;

        public DailyBatchFunction(BatchProcessingService batchProcessingService, ILogger<DailyBatchFunction> logger)
        {
            _batchProcessingService = batchProcessingService;
            _logger = logger;
        }

        [Function("DailyBatch")]
        public async Task Run(
            [TimerTrigger("0 0 2 * * *")] TimerInfo timer,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily batch started at {TimeUtc}", DateTime.UtcNow);

            var summary = await _batchProcessingService.RunAsync(cancellationToken);

            _logger.LogInformation(
                "Daily batch finished. BatchRunId={BatchRunId} ApplicationsProcessed={ApplicationsProcessed} OffersGenerated={OffersGenerated}",
                summary.BatchRunId,
                summary.ApplicationsProcessed,
                summary.OffersGenerated);
        }
    }
}
