using System;
using System.Threading.Tasks;
using Shared.Models.Dto;
using Shared.Models.Enums;

namespace BuyMyHouse.Api.Services
{
    public interface IMortgageApplicationsService
    {
        Task<PagedResultDto<MortgageApplicationDto>> SearchAsync(Guid? applicantId, ApplicationStatus? status, int page, int pageSize);
        Task<MortgageApplicationDto?> GetByIdAsync(Guid applicationId);
        Task<MortgageApplicationsCreateResult> CreateAsync(CreateMortgageApplicationRequestDto request);
        Task<MortgageApplicationsUpdateResult> UpdateAsync(Guid applicationId, UpdateMortgageApplicationRequestDto request);
        Task<MortgageApplicationsUpdateStatusResult> UpdateStatusAsync(Guid applicationId, UpdateMortgageApplicationStatusRequestDto request);
        Task<MortgageApplicationsSubmitResult> SubmitAsync(Guid applicationId);
        Task<MortgageApplicationsIncomeResult> GetIncomeAsync(Guid applicationId);
        Task<MortgageApplicationsDeleteStatus> DeleteAsync(Guid applicationId);
    }

    public enum MortgageApplicationsCreateStatus
    {
        Ok = 0,
        ApplicantNotFound = 1,
        HouseNotFound = 2
    }

    public record MortgageApplicationsCreateResult(MortgageApplicationsCreateStatus Status, MortgageApplicationDto? Application);

    public enum MortgageApplicationsUpdateStatus
    {
        Ok = 0,
        NotFound = 1
    }

    public record MortgageApplicationsUpdateResult(MortgageApplicationsUpdateStatus Status, MortgageApplicationDto? Application);

    public enum MortgageApplicationsStatusUpdateStatus
    {
        Ok = 0,
        NotFound = 1
    }

    public record MortgageApplicationsUpdateStatusResult(MortgageApplicationsStatusUpdateStatus Status, MortgageApplicationDto? Application);

    public enum MortgageApplicationsIncomeStatus
    {
        Ok = 0,
        ApplicationNotFound = 1,
        IncomeNotFound = 2
    }

    public record MortgageApplicationsIncomeResult(MortgageApplicationsIncomeStatus Status, IncomeRecordDto? Income);

    public enum MortgageApplicationsDeleteStatus
    {
        Ok = 0,
        NotFound = 1
    }

    public enum MortgageApplicationsSubmitStatus
    {
        Ok = 0,
        NotFound = 1
    }

    public record MortgageApplicationsSubmitResult(MortgageApplicationsSubmitStatus Status, MortgageApplicationDto? Application);
}
