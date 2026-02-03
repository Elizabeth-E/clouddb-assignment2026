using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data.EF;
using BuyMyHouse.Api.Mappings;
using Shared.Models.Dto;
using Shared.Models.Entities;
using Shared.Models.Enums;

namespace BuyMyHouse.Api.Services
{
    public class MortgageApplicationsService : IMortgageApplicationsService
    {
        private readonly AppDbContext _db;

        public MortgageApplicationsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResultDto<MortgageApplicationDto>> SearchAsync(
            Guid? applicantId,
            ApplicationStatus? status,
            int page,
            int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            IQueryable<MortgageApplicationEntity> query = _db.MortgageApplications.AsNoTracking();

            if (applicantId.HasValue) query = query.Where(a => a.ApplicantId == applicantId.Value);
            if (status.HasValue) query = query.Where(a => a.Status == status.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = items.Select(MortgageApplicationMapper.ToDto).ToList();

            return new PagedResultDto<MortgageApplicationDto>
            {
                Items = dtos,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<MortgageApplicationDto?> GetByIdAsync(Guid applicationId)
        {
            var application = await _db.MortgageApplications.AsNoTracking()
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application is null) return null;

            return MortgageApplicationMapper.ToDto(application);
        }

        public async Task<MortgageApplicationsCreateResult> CreateAsync(CreateMortgageApplicationRequestDto request)
        {
            var applicantExists = await _db.Applicants.AsNoTracking()
                .AnyAsync(a => a.ApplicantId == request.ApplicantId);

            if (!applicantExists)
            {
                return new MortgageApplicationsCreateResult(MortgageApplicationsCreateStatus.ApplicantNotFound, null);
            }

            if (request.HouseId.HasValue)
            {
                var houseExists = await _db.Houses.AsNoTracking()
                    .AnyAsync(h => h.HouseId == request.HouseId.Value);

                if (!houseExists)
                {
                    return new MortgageApplicationsCreateResult(MortgageApplicationsCreateStatus.HouseNotFound, null);
                }
            }

            var now = DateTime.UtcNow;
            var incomeRecordId = Guid.NewGuid();
            var applicationId = Guid.NewGuid();

            var income = IncomeRecordMapper.ToEntity(request.Income, incomeRecordId, request.ApplicantId, now);
            var application = MortgageApplicationMapper.ToEntity(request, applicationId, incomeRecordId, now);

            _db.IncomeRecords.Add(income);
            _db.MortgageApplications.Add(application);

            await _db.SaveChangesAsync();

            return new MortgageApplicationsCreateResult(
                MortgageApplicationsCreateStatus.Ok,
                MortgageApplicationMapper.ToDto(application));
        }

        public async Task<MortgageApplicationsUpdateResult> UpdateAsync(Guid applicationId, UpdateMortgageApplicationRequestDto request)
        {
            var application = await _db.MortgageApplications
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application is null)
            {
                return new MortgageApplicationsUpdateResult(MortgageApplicationsUpdateStatus.NotFound, null);
            }

            MortgageApplicationMapper.ApplyUpdate(application, request, DateTime.UtcNow);
            await _db.SaveChangesAsync();

            return new MortgageApplicationsUpdateResult(
                MortgageApplicationsUpdateStatus.Ok,
                MortgageApplicationMapper.ToDto(application));
        }

        public async Task<MortgageApplicationsUpdateStatusResult> UpdateStatusAsync(
            Guid applicationId,
            UpdateMortgageApplicationStatusRequestDto request)
        {
            var application = await _db.MortgageApplications
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application is null)
            {
                return new MortgageApplicationsUpdateStatusResult(MortgageApplicationsStatusUpdateStatus.NotFound, null);
            }

            application.Status = request.Status;
            application.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return new MortgageApplicationsUpdateStatusResult(
                MortgageApplicationsStatusUpdateStatus.Ok,
                MortgageApplicationMapper.ToDto(application));
        }

        public async Task<MortgageApplicationsSubmitResult> SubmitAsync(Guid applicationId)
        {
            var application = await _db.MortgageApplications
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application is null)
            {
                return new MortgageApplicationsSubmitResult(MortgageApplicationsSubmitStatus.NotFound, null);
            }

            application.Status = ApplicationStatus.Submitted;
            application.SubmittedAtUtc = DateTime.UtcNow;
            application.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return new MortgageApplicationsSubmitResult(
                MortgageApplicationsSubmitStatus.Ok,
                MortgageApplicationMapper.ToDto(application));
        }

        public async Task<MortgageApplicationsIncomeResult> GetIncomeAsync(Guid applicationId)
        {
            var application = await _db.MortgageApplications.AsNoTracking()
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application is null)
            {
                return new MortgageApplicationsIncomeResult(MortgageApplicationsIncomeStatus.ApplicationNotFound, null);
            }

            var income = await _db.IncomeRecords.AsNoTracking()
                .FirstOrDefaultAsync(i => i.IncomeRecordId == application.IncomeRecordId);

            if (income is null)
            {
                return new MortgageApplicationsIncomeResult(MortgageApplicationsIncomeStatus.IncomeNotFound, null);
            }

            return new MortgageApplicationsIncomeResult(
                MortgageApplicationsIncomeStatus.Ok,
                IncomeRecordMapper.ToDto(income));
        }

        public async Task<MortgageApplicationsDeleteStatus> DeleteAsync(Guid applicationId)
        {
            var application = await _db.MortgageApplications
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application is null) return MortgageApplicationsDeleteStatus.NotFound;

            _db.MortgageApplications.Remove(application);
            await _db.SaveChangesAsync();

            return MortgageApplicationsDeleteStatus.Ok;
        }
    }
}
