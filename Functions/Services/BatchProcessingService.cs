using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Shared.Models.Enums;

namespace Functions.Services
{
    public class BatchProcessingService
    {
        private static readonly TimeSpan MaxRunDuration = TimeSpan.FromMinutes(10);
        private readonly Func<SqlConnection> _connectionFactory;
        private readonly OfferGenerationService _offerGenerationService;
        private readonly ILogger<BatchProcessingService> _logger;

        public BatchProcessingService(
            Func<SqlConnection> connectionFactory,
            OfferGenerationService offerGenerationService,
            ILogger<BatchProcessingService> logger)
        {
            _connectionFactory = connectionFactory;
            _offerGenerationService = offerGenerationService;
            _logger = logger;
        }

        public async Task<BatchRunSummary> RunAsync(CancellationToken cancellationToken)
        {
            var batchRunId = Guid.NewGuid();
            var startedAt = DateTime.UtcNow;
            var runStopwatch = Stopwatch.StartNew();

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(MaxRunDuration);
            var effectiveToken = timeoutCts.Token;

            _logger.LogInformation("Batch run {BatchRunId} starting at {StartedAtUtc}.", batchRunId, startedAt);

            await InsertBatchRunAsync(batchRunId, startedAt, effectiveToken);

            var applicationIds = await LoadPendingApplicationsAsync(effectiveToken);

            var processed = 0;
            var offersGenerated = 0;
            var emailsQueued = 0;

            _logger.LogInformation(
                "Batch run {BatchRunId} loaded {ApplicationCount} pending applications.",
                batchRunId,
                applicationIds.Count);

            try
            {
                foreach (var applicationId in applicationIds)
                {
                    effectiveToken.ThrowIfCancellationRequested();

                    processed++;

                    _logger.LogInformation(
                        "Batch run {BatchRunId} processing application {ApplicationId} ({Index}/{Total}).",
                        batchRunId,
                        applicationId,
                        processed,
                        applicationIds.Count);

                    var appStopwatch = Stopwatch.StartNew();
                    var result = await _offerGenerationService.GenerateAsync(applicationId, effectiveToken);
                    appStopwatch.Stop();

                    _logger.LogInformation(
                        "Batch run {BatchRunId} finished application {ApplicationId}. Success={Success} ElapsedMs={ElapsedMs}.",
                        batchRunId,
                        applicationId,
                        result.Success,
                        appStopwatch.ElapsedMilliseconds);

                    if (result.Success)
                    {
                        offersGenerated++;
                        emailsQueued++;
                    }
                }

                await UpdateBatchRunAsync(
                    batchRunId,
                    DateTime.UtcNow,
                    BatchStatus.Completed,
                    processed,
                    offersGenerated,
                    emailsQueued,
                    null,
                    effectiveToken);

                runStopwatch.Stop();
                _logger.LogInformation(
                    "Batch run {BatchRunId} completed. Processed={Processed} OffersGenerated={OffersGenerated} EmailsQueued={EmailsQueued} ElapsedMs={ElapsedMs}.",
                    batchRunId,
                    processed,
                    offersGenerated,
                    emailsQueued,
                    runStopwatch.ElapsedMilliseconds);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(
                    "Batch run {BatchRunId} timed out after {TimeoutMinutes} minutes.",
                    batchRunId,
                    MaxRunDuration.TotalMinutes);

                await UpdateBatchRunAsync(
                    batchRunId,
                    DateTime.UtcNow,
                    BatchStatus.Failed,
                    processed,
                    offersGenerated,
                    emailsQueued,
                    $"Batch timed out after {MaxRunDuration.TotalMinutes:0} minutes.",
                    CancellationToken.None);

                return new BatchRunSummary(batchRunId, processed, offersGenerated, emailsQueued);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Batch run {BatchRunId} was cancelled by request.", batchRunId);

                await UpdateBatchRunAsync(
                    batchRunId,
                    DateTime.UtcNow,
                    BatchStatus.Failed,
                    processed,
                    offersGenerated,
                    emailsQueued,
                    "Batch was cancelled.",
                    CancellationToken.None);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Batch run {BatchRunId} failed with an exception.", batchRunId);

                await UpdateBatchRunAsync(
                    batchRunId,
                    DateTime.UtcNow,
                    BatchStatus.Failed,
                    processed,
                    offersGenerated,
                    emailsQueued,
                    ex.Message,
                    CancellationToken.None);

                throw;
            }

            return new BatchRunSummary(batchRunId, processed, offersGenerated, emailsQueued);
        }

        private async Task InsertBatchRunAsync(Guid batchRunId, DateTime startedAtUtc, CancellationToken cancellationToken)
        {
            const string sql = @"
INSERT INTO BatchRuns
(BatchRunId, RunDateUtc, StartedAtUtc, FinishedAtUtc, Status, ApplicationsProcessed, OffersGenerated, EmailsQueued, ErrorMessage)
VALUES
(@BatchRunId, @RunDateUtc, @StartedAtUtc, NULL, @Status, 0, 0, 0, NULL);";

            await using var connection = _connectionFactory();
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@BatchRunId", batchRunId);
            command.Parameters.AddWithValue("@RunDateUtc", startedAtUtc.Date);
            command.Parameters.AddWithValue("@StartedAtUtc", startedAtUtc);
            command.Parameters.AddWithValue("@Status", (int)BatchStatus.Running);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private async Task UpdateBatchRunAsync(
            Guid batchRunId,
            DateTime finishedAtUtc,
            BatchStatus status,
            int applicationsProcessed,
            int offersGenerated,
            int emailsQueued,
            string? errorMessage,
            CancellationToken cancellationToken)
        {
            const string sql = @"
UPDATE BatchRuns
SET FinishedAtUtc = @FinishedAtUtc,
    Status = @Status,
    ApplicationsProcessed = @ApplicationsProcessed,
    OffersGenerated = @OffersGenerated,
    EmailsQueued = @EmailsQueued,
    ErrorMessage = @ErrorMessage
WHERE BatchRunId = @BatchRunId;";

            await using var connection = _connectionFactory();
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@BatchRunId", batchRunId);
            command.Parameters.AddWithValue("@FinishedAtUtc", finishedAtUtc);
            command.Parameters.AddWithValue("@Status", (int)status);
            command.Parameters.AddWithValue("@ApplicationsProcessed", applicationsProcessed);
            command.Parameters.AddWithValue("@OffersGenerated", offersGenerated);
            command.Parameters.AddWithValue("@EmailsQueued", emailsQueued);
            command.Parameters.AddWithValue("@ErrorMessage", (object?)errorMessage ?? DBNull.Value);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private async Task<List<Guid>> LoadPendingApplicationsAsync(CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT a.ApplicationId
FROM MortgageApplications a
WHERE a.Status = @Status
  AND NOT EXISTS (
      SELECT 1 FROM MortgageOffers o WHERE o.ApplicationId = a.ApplicationId
  );";

            var result = new List<Guid>();

            await using var connection = _connectionFactory();
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Status", (int)ApplicationStatus.Submitted);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(reader.GetGuid(0));
            }

            return result;
        }
    }

    public record BatchRunSummary(Guid BatchRunId, int ApplicationsProcessed, int OffersGenerated, int EmailsQueued);
}
