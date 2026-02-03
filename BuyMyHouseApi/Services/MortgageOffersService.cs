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
    public class MortgageOffersService : IMortgageOffersService
    {
        private readonly AppDbContext _db;
        private readonly IOfferDocumentUrlService _offerDocumentUrlService;

        public MortgageOffersService(AppDbContext db, IOfferDocumentUrlService offerDocumentUrlService)
        {
            _db = db;
            _offerDocumentUrlService = offerDocumentUrlService;
        }

        public async Task<PagedResultDto<MortgageOfferDto>> SearchAsync(
            Guid? applicationId,
            Guid? applicantId,
            int page,
            int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            IQueryable<MortgageOfferEntity> query = _db.MortgageOffers.AsNoTracking();

            if (applicationId.HasValue)
            {
                query = query.Where(o => o.ApplicationId == applicationId.Value);
            }

            if (applicantId.HasValue)
            {
                query = query.Where(o => o.ApplicantId == applicantId.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.GeneratedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = items
                .Select(o => MortgageOfferMapper.ToDto(o, _offerDocumentUrlService.GetOfferDocumentUrl(o.DocumentBlobKey)))
                .ToList();

            return new PagedResultDto<MortgageOfferDto>
            {
                Items = dtos,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<MortgageOfferDto?> GetByIdAsync(Guid offerId)
        {
            var offer = await _db.MortgageOffers.AsNoTracking()
                .FirstOrDefaultAsync(o => o.OfferId == offerId);

            if (offer is null) return null;

            return MortgageOfferMapper.ToDto(offer, _offerDocumentUrlService.GetOfferDocumentUrl(offer.DocumentBlobKey));
        }

        public async Task<OfferViewDto?> GetViewAsync(Guid offerId)
        {
            var offer = await _db.MortgageOffers.AsNoTracking()
                .FirstOrDefaultAsync(o => o.OfferId == offerId);

            if (offer is null) return null;

            return MortgageOfferMapper.ToViewDto(offer, _offerDocumentUrlService.GetOfferDocumentUrl(offer.DocumentBlobKey));
        }
    }
}
