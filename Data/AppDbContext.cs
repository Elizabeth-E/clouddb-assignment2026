

using Microsoft.EntityFrameworkCore;
using Shared.Models.Entities;

namespace Data.EF
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<HouseEntity> Houses => Set<HouseEntity>();
        public DbSet<HousePhotoEntity> HousePhotos => Set<HousePhotoEntity>();
        public DbSet<ApplicantEntity> Applicants => Set<ApplicantEntity>();
        public DbSet<IncomeRecordEntity> IncomeRecords => Set<IncomeRecordEntity>();
        public DbSet<MortgageApplicationEntity> MortgageApplications => Set<MortgageApplicationEntity>();
        public DbSet<MortgageOfferEntity> MortgageOffers => Set<MortgageOfferEntity>();
        public DbSet<OfferAccessTokenEntity> OfferAccessTokens => Set<OfferAccessTokenEntity>();
        public DbSet<BatchRunEntity> BatchRuns => Set<BatchRunEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // House
            modelBuilder.Entity<HouseEntity>(entity =>
            {
                entity.HasKey(h => h.HouseId);

                entity.Property(h => h.AddressLine1).HasMaxLength(200).IsRequired();
                entity.Property(h => h.PostalCode).HasMaxLength(20).IsRequired();
                entity.Property(h => h.City).HasMaxLength(100).IsRequired();

                entity.Property(h => h.AskingPrice).HasColumnType("decimal(18,2)");

                entity.Property(h => h.EnergyLabel).HasMaxLength(10);
            });

            // HousePhoto
            modelBuilder.Entity<HousePhotoEntity>(entity =>
            {
                entity.HasKey(p => p.PhotoId);

                entity.Property(p => p.FileName).HasMaxLength(260).IsRequired();
                entity.Property(p => p.ContentType).HasMaxLength(100).IsRequired();
                entity.Property(p => p.BlobStorageKey).HasMaxLength(500).IsRequired();

                // Useful index for house photo lookups
                entity.HasIndex(p => p.HouseId);

                entity.HasOne<HouseEntity>()
                    .WithMany()
                    .HasForeignKey(p => p.HouseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Applicant
            modelBuilder.Entity<ApplicantEntity>(entity =>
            {
                entity.HasKey(a => a.ApplicantId);

                entity.Property(a => a.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(a => a.LastName).HasMaxLength(100).IsRequired();
                entity.Property(a => a.Email).HasMaxLength(320).IsRequired();
                entity.Property(a => a.Phone).HasMaxLength(30);

                entity.HasIndex(a => a.Email).IsUnique();
            });

            // IncomeRecord
            modelBuilder.Entity<IncomeRecordEntity>(entity =>
            {
                entity.HasKey(i => i.IncomeRecordId);

                entity.Property(i => i.EmployerName).HasMaxLength(200);

                entity.Property(i => i.GrossAnnualIncome).HasColumnType("decimal(18,2)");
                entity.Property(i => i.NetMonthlyIncome).HasColumnType("decimal(18,2)");
                entity.Property(i => i.OtherIncomeAnnual).HasColumnType("decimal(18,2)");
                entity.Property(i => i.MonthlyDebtPayments).HasColumnType("decimal(18,2)");

                entity.HasIndex(i => i.ApplicantId);

                entity.HasOne<ApplicantEntity>()
                    .WithMany()
                    .HasForeignKey(i => i.ApplicantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MortgageApplication
            modelBuilder.Entity<MortgageApplicationEntity>(entity =>
            {
                entity.HasKey(a => a.ApplicationId);

                entity.Property(a => a.RequestedLoanAmount).HasColumnType("decimal(18,2)");
                entity.Property(a => a.Interest).HasColumnType("decimal(9,4)");
                entity.Property(a => a.DownPayment).HasColumnType("decimal(18,2)");
                entity.Property(a => a.PartnerIncomeAnnual).HasColumnType("decimal(18,2)");
                entity.Property(a => a.CurrentRentOrMortgageMonthly).HasColumnType("decimal(18,2)");

                entity.HasIndex(a => a.ApplicantId);
                entity.HasIndex(a => a.HouseId);

                entity.HasOne<ApplicantEntity>()
                    .WithMany()
                    .HasForeignKey(a => a.ApplicantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<HouseEntity>()
                    .WithMany()
                    .HasForeignKey(a => a.HouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<IncomeRecordEntity>()
                    .WithMany()
                    .HasForeignKey(a => a.IncomeRecordId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // MortgageOffer
            modelBuilder.Entity<MortgageOfferEntity>(entity =>
            {
                entity.HasKey(o => o.OfferId);

                entity.Property(o => o.ApprovedLoanAmount).HasColumnType("decimal(18,2)");
                entity.Property(o => o.InterestRate).HasColumnType("decimal(9,4)");
                entity.Property(o => o.MonthlyPayment).HasColumnType("decimal(18,2)");
                entity.Property(o => o.Notes).HasMaxLength(2000);
                entity.Property(o => o.DocumentBlobKey).HasMaxLength(500).IsRequired();

                entity.HasIndex(o => o.ApplicationId);
                entity.HasIndex(o => o.ApplicantId);

                entity.HasOne<MortgageApplicationEntity>()
                    .WithMany()
                    .HasForeignKey(o => o.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<ApplicantEntity>()
                    .WithMany()
                    .HasForeignKey(o => o.ApplicantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OfferAccessToken
            modelBuilder.Entity<OfferAccessTokenEntity>(entity =>
            {
                entity.HasKey(t => t.TokenId);

                entity.Property(t => t.TokenHash).HasMaxLength(200).IsRequired();

                entity.HasIndex(t => t.OfferId);

                entity.HasOne<MortgageOfferEntity>()
                    .WithMany()
                    .HasForeignKey(t => t.OfferId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // BatchRun
            modelBuilder.Entity<BatchRunEntity>(entity =>
            {
                entity.HasKey(b => b.BatchRunId);

                entity.Property(b => b.ErrorMessage).HasMaxLength(2000);
            });
        }
    }
}
