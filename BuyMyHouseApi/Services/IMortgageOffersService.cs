using System;
using System.Threading.Tasks;
using Shared.Models.Dto;

namespace BuyMyHouse.Api.Services
{
    public interface IMortgageOffersService
    {
        Task<PagedResultDto<MortgageOfferDto>> SearchAsync(Guid? applicationId, Guid? applicantId, int page, int pageSize);
        Task<MortgageOfferDto?> GetByIdAsync(Guid offerId);
        Task<OfferViewDto?> GetViewAsync(Guid offerId);
    }
}
