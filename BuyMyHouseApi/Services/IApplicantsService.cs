using System;
using System.Threading.Tasks;
using Shared.Models.Dto;

namespace BuyMyHouse.Api.Services
{
    public interface IApplicantsService
    {
        Task<PagedResultDto<ApplicantDto>> SearchAsync(string? email, int page, int pageSize);
        Task<ApplicantDto?> GetByIdAsync(Guid applicantId);
        Task<ApplicantsCreateResult> CreateAsync(CreateApplicantRequestDto request);
    }

    public enum ApplicantsCreateStatus
    {
        Ok = 0,
        Conflict = 1
    }

    public record ApplicantsCreateResult(ApplicantsCreateStatus Status, ApplicantDto? Applicant);
}
