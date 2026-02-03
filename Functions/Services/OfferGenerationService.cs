using System;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Functions.Options;
using Newtonsoft.Json;
using Shared.Models.Enums;

namespace Functions.Services
{
    public class OfferGenerationService
    {
        private readonly Func<SqlConnection> _connectionFactory;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly FunctionsOptions _options;
        private readonly ILogger<OfferGenerationService> _logger;

        public OfferGenerationService(
            Func<SqlConnection> connectionFactory,
            BlobServiceClient blobServiceClient,
            IOptions<FunctionsOptions> options,
            ILogger<OfferGenerationService> logger)
        {
            _connectionFactory = connectionFactory;
            _blobServiceClient = blobServiceClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<OfferGenerationResult> GenerateAsync(Guid applicationId, CancellationToken cancellationToken)
        {
            var overallStopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Offer generation starting for ApplicationId={ApplicationId}.", applicationId);

            var loadStopwatch = Stopwatch.StartNew();
            var application = await LoadApplicationAsync(applicationId, cancellationToken);
            loadStopwatch.Stop();

            if (application is null)
            {
                _logger.LogWarning(
                    "Offer generation could not find ApplicationId={ApplicationId}. ElapsedMs={ElapsedMs}.",
                    applicationId,
                    loadStopwatch.ElapsedMilliseconds);
                return OfferGenerationResult.Missing();
            }

            var offerId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var validUntil = now.AddDays(7);

            var interestRate = 3.5m;
            var approvedAmount = application.RequestedLoanAmount;
            var termYears = application.DesiredTermYears > 0 ? application.DesiredTermYears : 30;
            var monthlyPayment = CalculateMonthlyPayment(approvedAmount, interestRate, termYears);

            _logger.LogInformation(
                "Offer generation {OfferId} writing offer document for ApplicationId={ApplicationId}.",
                offerId,
                applicationId);
            var blobStopwatch = Stopwatch.StartNew();
            var blobKey = await WriteOfferDocumentAsync(application, offerId, interestRate, approvedAmount, termYears, monthlyPayment, cancellationToken);
            blobStopwatch.Stop();
            _logger.LogInformation(
                "Offer generation {OfferId} wrote offer document to {BlobKey}. ElapsedMs={ElapsedMs}.",
                offerId,
                blobKey,
                blobStopwatch.ElapsedMilliseconds);

            _logger.LogInformation(
                "Offer generation {OfferId} inserting offer record for ApplicationId={ApplicationId}.",
                offerId,
                applicationId);
            var insertStopwatch = Stopwatch.StartNew();
            await InsertOfferAsync(
                offerId,
                application.ApplicationId,
                application.ApplicantId,
                now,
                validUntil,
                approvedAmount,
                interestRate,
                monthlyPayment,
                termYears,
                blobKey,
                cancellationToken);
            insertStopwatch.Stop();
            _logger.LogInformation(
                "Offer generation {OfferId} inserted offer record. ElapsedMs={ElapsedMs}.",
                offerId,
                insertStopwatch.ElapsedMilliseconds);

            _logger.LogInformation(
                "Offer generation {OfferId} updating application status for ApplicationId={ApplicationId}.",
                offerId,
                applicationId);
            var statusStopwatch = Stopwatch.StartNew();
            await UpdateApplicationStatusAsync(application.ApplicationId, ApplicationStatus.OfferReady, cancellationToken);
            statusStopwatch.Stop();
            _logger.LogInformation(
                "Offer generation {OfferId} updated application status. ElapsedMs={ElapsedMs}.",
                offerId,
                statusStopwatch.ElapsedMilliseconds);

            string? token = null;

            overallStopwatch.Stop();
            _logger.LogInformation(
                "Offer generation {OfferId} completed for ApplicationId={ApplicationId}. TotalElapsedMs={ElapsedMs}.",
                offerId,
                applicationId,
                overallStopwatch.ElapsedMilliseconds);

            return OfferGenerationResult.Ok(offerId, blobKey, token);
        }

        private async Task<MortgageApplicationRow?> LoadApplicationAsync(Guid applicationId, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT TOP 1 a.ApplicationId,
       a.ApplicantId,
       a.RequestedLoanAmount,
       a.DesiredTermYears,
       ap.FirstName,
       ap.LastName
FROM MortgageApplications a
JOIN Applicants ap ON ap.ApplicantId = a.ApplicantId
WHERE a.ApplicationId = @ApplicationId;";

            await using var connection = _connectionFactory();
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ApplicationId", applicationId);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken)) return null;

            return new MortgageApplicationRow
            {
                ApplicationId = reader.GetGuid(0),
                ApplicantId = reader.GetGuid(1),
                RequestedLoanAmount = reader.IsDBNull(2) ? 0m : reader.GetDecimal(2),
                DesiredTermYears = reader.IsDBNull(3) ? 30 : reader.GetInt32(3),
                FirstName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                LastName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
            };
        }

        private async Task<string> WriteOfferDocumentAsync(
            MortgageApplicationRow application,
            Guid offerId,
            decimal interestRate,
            decimal approvedAmount,
            int termYears,
            decimal monthlyPayment,
            CancellationToken cancellationToken)
        {
            var container = _blobServiceClient.GetBlobContainerClient(_options.OfferDocumentsContainer);
            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var applicantSlug = BuildApplicantSlug(application.FirstName, application.LastName);
            var blobKey = $"offers/{applicantSlug}/{application.ApplicationId}/{offerId}.json";

            var doc = new
            {
                offerId,
                applicationId = application.ApplicationId,
                applicantId = application.ApplicantId,
                approvedAmount,
                interestRate,
                termYears,
                monthlyPayment,
                generatedAtUtc = DateTime.UtcNow
            };

            var json = JsonConvert.SerializeObject(doc, Formatting.Indented);
            var blobClient = container.GetBlobClient(blobKey);

            await blobClient.UploadAsync(BinaryData.FromString(json), overwrite: true, cancellationToken: cancellationToken);

            return blobKey;
        }

        private async Task InsertOfferAsync(
            Guid offerId,
            Guid applicationId,
            Guid applicantId,
            DateTime generatedAtUtc,
            DateTime validUntilUtc,
            decimal approvedAmount,
            decimal interestRate,
            decimal monthlyPayment,
            int termYears,
            string blobKey,
            CancellationToken cancellationToken)
        {
            const string sql = @"
INSERT INTO MortgageOffers
(OfferId, ApplicationId, ApplicantId, Decision, GeneratedAtUtc, ValidUntilUtc, ApprovedLoanAmount, InterestRate, MonthlyPayment, TermYears, Notes, DocumentFormat, DocumentBlobKey)
VALUES
(@OfferId, @ApplicationId, @ApplicantId, @Decision, @GeneratedAtUtc, @ValidUntilUtc, @ApprovedLoanAmount, @InterestRate, @MonthlyPayment, @TermYears, @Notes, @DocumentFormat, @DocumentBlobKey);";

            await using var connection = _connectionFactory();
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@OfferId", offerId);
            command.Parameters.AddWithValue("@ApplicationId", applicationId);
            command.Parameters.AddWithValue("@ApplicantId", applicantId);
            command.Parameters.AddWithValue("@Decision", (int)OfferDecision.Approved);
            command.Parameters.AddWithValue("@GeneratedAtUtc", generatedAtUtc);
            command.Parameters.AddWithValue("@ValidUntilUtc", validUntilUtc);
            command.Parameters.AddWithValue("@ApprovedLoanAmount", approvedAmount);
            command.Parameters.AddWithValue("@InterestRate", interestRate);
            command.Parameters.AddWithValue("@MonthlyPayment", monthlyPayment);
            command.Parameters.AddWithValue("@TermYears", termYears);
            command.Parameters.AddWithValue("@Notes", "Generated by POC batch");
            command.Parameters.AddWithValue("@DocumentFormat", (int)OfferDocumentFormat.Json);
            command.Parameters.AddWithValue("@DocumentBlobKey", blobKey);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private async Task UpdateApplicationStatusAsync(Guid applicationId, ApplicationStatus status, CancellationToken cancellationToken)
        {
            const string sql = @"
UPDATE MortgageApplications
SET Status = @Status, UpdatedAtUtc = @UpdatedAtUtc
WHERE ApplicationId = @ApplicationId;";

            await using var connection = _connectionFactory();
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Status", (int)status);
            command.Parameters.AddWithValue("@UpdatedAtUtc", DateTime.UtcNow);
            command.Parameters.AddWithValue("@ApplicationId", applicationId);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private static decimal CalculateMonthlyPayment(decimal principal, decimal annualRatePercent, int years)
        {
            if (principal <= 0 || years <= 0) return 0m;

            var monthlyRate = (double)annualRatePercent / 100d / 12d;
            var months = years * 12;

            if (monthlyRate <= 0) return principal / months;

            var factor = Math.Pow(1 + monthlyRate, months);
            var payment = (double)principal * monthlyRate * factor / (factor - 1);

            return (decimal)Math.Round(payment, 2);
        }

        private static string BuildApplicantSlug(string? firstName, string? lastName)
        {
            var combined = $"{firstName} {lastName}".Trim();
            var slug = BuildSlug(combined);
            return string.IsNullOrWhiteSpace(slug) ? "applicant" : slug;
        }

        private static string BuildSlug(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            var builder = new StringBuilder(value.Length);
            var lastWasDash = false;

            foreach (var ch in value)
            {
                if (char.IsLetterOrDigit(ch))
                {
                    builder.Append(char.ToLowerInvariant(ch));
                    lastWasDash = false;
                    continue;
                }

                if (builder.Length == 0 || lastWasDash) continue;

                builder.Append('-');
                lastWasDash = true;
            }

            return builder.ToString().Trim('-');
        }

        private sealed class MortgageApplicationRow
        {
            public Guid ApplicationId { get; set; }
            public Guid ApplicantId { get; set; }
            public decimal RequestedLoanAmount { get; set; }
            public int DesiredTermYears { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
        }

    }

    public record OfferGenerationResult(bool Success, bool NotFound, Guid? OfferId, string? DocumentBlobKey, string? AccessToken)
    {
        public static OfferGenerationResult Ok(Guid offerId, string blobKey, string? accessToken) => new(true, false, offerId, blobKey, accessToken);
        public static OfferGenerationResult Missing() => new(false, true, null, null, null);
    }
}
