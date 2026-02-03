using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMortgageEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applicants",
                columns: table => new
                {
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicants", x => x.ApplicantId);
                });

            migrationBuilder.CreateTable(
                name: "BatchRuns",
                columns: table => new
                {
                    BatchRunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RunDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApplicationsProcessed = table.Column<int>(type: "int", nullable: false),
                    OffersGenerated = table.Column<int>(type: "int", nullable: false),
                    EmailsQueued = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchRuns", x => x.BatchRunId);
                });

            migrationBuilder.CreateTable(
                name: "IncomeRecords",
                columns: table => new
                {
                    IncomeRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GrossAnnualIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetMonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OtherIncomeAnnual = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MonthlyDebtPayments = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RecordedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeRecords", x => x.IncomeRecordId);
                    table.ForeignKey(
                        name: "FK_IncomeRecords_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Applicants",
                        principalColumn: "ApplicantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MortgageApplications",
                columns: table => new
                {
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IncomeRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DesiredTermYears = table.Column<int>(type: "int", nullable: false),
                    Interest = table.Column<decimal>(type: "decimal(9,4)", nullable: true),
                    DownPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HasPartner = table.Column<bool>(type: "bit", nullable: false),
                    PartnerIncomeAnnual = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentRentOrMortgageMonthly = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MortgageApplications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_MortgageApplications_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Applicants",
                        principalColumn: "ApplicantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MortgageApplications_Houses_HouseId",
                        column: x => x.HouseId,
                        principalTable: "Houses",
                        principalColumn: "HouseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MortgageApplications_IncomeRecords_IncomeRecordId",
                        column: x => x.IncomeRecordId,
                        principalTable: "IncomeRecords",
                        principalColumn: "IncomeRecordId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MortgageOffers",
                columns: table => new
                {
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Decision = table.Column<int>(type: "int", nullable: false),
                    GeneratedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidUntilUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InterestRate = table.Column<decimal>(type: "decimal(9,4)", nullable: true),
                    MonthlyPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TermYears = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DocumentFormat = table.Column<int>(type: "int", nullable: false),
                    DocumentBlobKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MortgageOffers", x => x.OfferId);
                    table.ForeignKey(
                        name: "FK_MortgageOffers_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Applicants",
                        principalColumn: "ApplicantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MortgageOffers_MortgageApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "MortgageApplications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfferAccessTokens",
                columns: table => new
                {
                    TokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferAccessTokens", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_OfferAccessTokens_MortgageOffers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "MortgageOffers",
                        principalColumn: "OfferId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_Email",
                table: "Applicants",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncomeRecords_ApplicantId",
                table: "IncomeRecords",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_MortgageApplications_ApplicantId",
                table: "MortgageApplications",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_MortgageApplications_HouseId",
                table: "MortgageApplications",
                column: "HouseId");

            migrationBuilder.CreateIndex(
                name: "IX_MortgageApplications_IncomeRecordId",
                table: "MortgageApplications",
                column: "IncomeRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_MortgageOffers_ApplicantId",
                table: "MortgageOffers",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_MortgageOffers_ApplicationId",
                table: "MortgageOffers",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferAccessTokens_OfferId",
                table: "OfferAccessTokens",
                column: "OfferId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatchRuns");

            migrationBuilder.DropTable(
                name: "OfferAccessTokens");

            migrationBuilder.DropTable(
                name: "MortgageOffers");

            migrationBuilder.DropTable(
                name: "MortgageApplications");

            migrationBuilder.DropTable(
                name: "IncomeRecords");

            migrationBuilder.DropTable(
                name: "Applicants");
        }
    }
}
