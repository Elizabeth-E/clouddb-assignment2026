using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data.EF;
using BuyMyHouse.Api.Mappings;
using Shared.Models.Dto;
using Shared.Models.Entities;

namespace BuyMyHouse.Api.Services
{
    public class ApplicantsService : IApplicantsService
    {
        private readonly AppDbContext _db;

        public ApplicantsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResultDto<ApplicantDto>> SearchAsync(string? email, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            IQueryable<ApplicantEntity> query = _db.Applicants.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(a => a.Email == email);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(a => a.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = items.Select(ApplicantMapper.ToDto).ToList();

            return new PagedResultDto<ApplicantDto>
            {
                Items = dtos,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ApplicantDto?> GetByIdAsync(Guid applicantId)
        {
            var applicant = await _db.Applicants.AsNoTracking()
                .FirstOrDefaultAsync(a => a.ApplicantId == applicantId);

            if (applicant is null) return null;

            return ApplicantMapper.ToDto(applicant);
        }

        public async Task<ApplicantsCreateResult> CreateAsync(CreateApplicantRequestDto request)
        {
            var exists = await _db.Applicants.AsNoTracking()
                .AnyAsync(a => a.Email == request.Email);

            if (exists)
            {
                return new ApplicantsCreateResult(ApplicantsCreateStatus.Conflict, null);
            }

            var now = DateTime.UtcNow;
            var applicantId = Guid.NewGuid();

            var entity = ApplicantMapper.ToEntity(request, applicantId, now);

            _db.Applicants.Add(entity);
            await _db.SaveChangesAsync();

            return new ApplicantsCreateResult(ApplicantsCreateStatus.Ok, ApplicantMapper.ToDto(entity));
        }
    }
}
