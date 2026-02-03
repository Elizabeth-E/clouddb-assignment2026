using System;
using System.Collections.Generic;
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
    public class HousesService : IHousesService
    {
        private readonly AppDbContext _db;
        private readonly IPhotoUrlService _photoUrlService;

        public HousesService(AppDbContext db, IPhotoUrlService photoUrlService)
        {
            _db = db;
            _photoUrlService = photoUrlService;
        }

        public async Task<PagedResultDto<HouseSummaryDto>> SearchAsync(
            decimal? minPrice,
            decimal? maxPrice,
            string? city,
            int page,
            int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            IQueryable<HouseEntity> query = _db.Houses.AsNoTracking();

            if (minPrice.HasValue) query = query.Where(h => h.AskingPrice >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(h => h.AskingPrice <= maxPrice.Value);
            if (!string.IsNullOrWhiteSpace(city)) query = query.Where(h => h.City == city);

            var totalCount = await query.CountAsync();

            var houses = await query
                .OrderBy(h => h.AskingPrice)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var houseIds = houses.Select(h => h.HouseId).ToList();

            var latestPhotos = await _db.HousePhotos.AsNoTracking()
                .Where(p => houseIds.Contains(p.HouseId))
                .OrderByDescending(p => p.UploadedAtUtc)
                .ToListAsync();

            var photoLookup = latestPhotos
                .GroupBy(p => p.HouseId)
                .ToDictionary(g => g.Key, g => g.First().PhotoId);

            var items = houses.Select(h =>
            {
                string? primaryUrl = null;
                if (photoLookup.TryGetValue(h.HouseId, out var photoId))
                {
                    primaryUrl = _photoUrlService.GetHousePhotoUrl(h.HouseId, photoId);
                }

                return HouseMapper.ToSummaryDto(h, primaryUrl);
            }).ToList();

            return new PagedResultDto<HouseSummaryDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<HouseDetailDto?> GetByIdAsync(Guid houseId)
        {
            var house = await _db.Houses.AsNoTracking()
                .FirstOrDefaultAsync(h => h.HouseId == houseId);

            if (house is null) return null;

            var photos = await _db.HousePhotos.AsNoTracking()
                .Where(p => p.HouseId == houseId)
                .OrderByDescending(p => p.UploadedAtUtc)
                .Select(p => new HousePhotoDto
                {
                    PhotoId = p.PhotoId,
                    Url = _photoUrlService.GetHousePhotoUrl(p.HouseId, p.PhotoId)
                })
                .ToListAsync();

            return HouseMapper.ToDetailDto(house, photos);
        }

        public async Task<IReadOnlyList<HousePhotoDto>?> GetPhotosAsync(Guid houseId)
        {
            var exists = await _db.Houses.AsNoTracking().AnyAsync(h => h.HouseId == houseId);
            if (!exists) return null;

            var photos = await _db.HousePhotos.AsNoTracking()
                .Where(p => p.HouseId == houseId)
                .OrderByDescending(p => p.UploadedAtUtc)
                .Select(p => new HousePhotoDto
                {
                    PhotoId = p.PhotoId,
                    Url = _photoUrlService.GetHousePhotoUrl(p.HouseId, p.PhotoId)
                })
                .ToListAsync();

            return photos;
        }

        public async Task<HouseDetailDto> CreateAsync(CreateHouseRequestDto request)
        {
            var now = DateTime.UtcNow;
            var entity = new HouseEntity
            {
                HouseId = Guid.NewGuid(),
                AddressLine1 = request.AddressLine1,
                PostalCode = request.PostalCode,
                City = request.City,
                AskingPrice = request.AskingPrice,
                Bedrooms = request.Bedrooms,
                Bathrooms = request.Bathrooms,
                LivingAreaM2 = request.LivingAreaM2,
                PlotSizeM2 = request.PlotSizeM2,
                YearBuilt = request.YearBuilt,
                EnergyLabel = request.EnergyLabel,
                Description = request.Description,
                Status = request.Status,
                CreatedAtUtc = now
            };

            _db.Houses.Add(entity);
            await _db.SaveChangesAsync();

            return HouseMapper.ToDetailDto(entity, new List<HousePhotoDto>());
        }

        public async Task<HouseDetailDto?> UpdateAsync(Guid houseId, UpdateHouseRequestDto request)
        {
            var entity = await _db.Houses.FirstOrDefaultAsync(h => h.HouseId == houseId);
            if (entity is null) return null;

            entity.AddressLine1 = request.AddressLine1;
            entity.PostalCode = request.PostalCode;
            entity.City = request.City;
            entity.AskingPrice = request.AskingPrice;
            entity.Bedrooms = request.Bedrooms;
            entity.Bathrooms = request.Bathrooms;
            entity.LivingAreaM2 = request.LivingAreaM2;
            entity.PlotSizeM2 = request.PlotSizeM2;
            entity.YearBuilt = request.YearBuilt;
            entity.EnergyLabel = request.EnergyLabel;
            entity.Description = request.Description;
            entity.Status = request.Status;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            var photos = await _db.HousePhotos.AsNoTracking()
                .Where(p => p.HouseId == houseId)
                .OrderByDescending(p => p.UploadedAtUtc)
                .Select(p => new HousePhotoDto
                {
                    PhotoId = p.PhotoId,
                    Url = _photoUrlService.GetHousePhotoUrl(p.HouseId, p.PhotoId)
                })
                .ToListAsync();

            return HouseMapper.ToDetailDto(entity, photos);
        }

        public async Task<bool> DeleteAsync(Guid houseId)
        {
            var entity = await _db.Houses.FirstOrDefaultAsync(h => h.HouseId == houseId);
            if (entity is null) return false;

            _db.Houses.Remove(entity);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<HouseDetailDto>> SeedAsync()
        {
            var now = DateTime.UtcNow;
            var houses = new List<HouseEntity>
            {
                new()
                {
                    HouseId = Guid.NewGuid(),
                    AddressLine1 = "12 Canal View",
                    PostalCode = "1012 AB",
                    City = "Amsterdam",
                    AskingPrice = 425000m,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    LivingAreaM2 = 68,
                    PlotSizeM2 = null,
                    YearBuilt = 1890,
                    EnergyLabel = "C",
                    Description = "Bright canal-side apartment with updated kitchen.",
                    Status = HouseStatus.ForSale,
                    CreatedAtUtc = now
                },
                new()
                {
                    HouseId = Guid.NewGuid(),
                    AddressLine1 = "54 Parklaan",
                    PostalCode = "3016 BB",
                    City = "Rotterdam",
                    AskingPrice = 575000m,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    LivingAreaM2 = 110,
                    PlotSizeM2 = 160,
                    YearBuilt = 1998,
                    EnergyLabel = "B",
                    Description = "Family home near the park with a private garden.",
                    Status = HouseStatus.ForSale,
                    CreatedAtUtc = now
                },
                new()
                {
                    HouseId = Guid.NewGuid(),
                    AddressLine1 = "8 Lindenstraat",
                    PostalCode = "3511 CE",
                    City = "Utrecht",
                    AskingPrice = 489000m,
                    Bedrooms = 3,
                    Bathrooms = 1,
                    LivingAreaM2 = 92,
                    PlotSizeM2 = 140,
                    YearBuilt = 1935,
                    EnergyLabel = "D",
                    Description = "Cozy townhouse with renovated attic and patio.",
                    Status = HouseStatus.ForSale,
                    CreatedAtUtc = now
                },
                new()
                {
                    HouseId = Guid.NewGuid(),
                    AddressLine1 = "221 Havenweg",
                    PostalCode = "9711 AX",
                    City = "Groningen",
                    AskingPrice = 365000m,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    LivingAreaM2 = 75,
                    PlotSizeM2 = 120,
                    YearBuilt = 1978,
                    EnergyLabel = "C",
                    Description = "Renovated corner house with extra storage.",
                    Status = HouseStatus.ForSale,
                    CreatedAtUtc = now
                },
                new()
                {
                    HouseId = Guid.NewGuid(),
                    AddressLine1 = "5 Duinweg",
                    PostalCode = "2011 DE",
                    City = "Haarlem",
                    AskingPrice = 645000m,
                    Bedrooms = 4,
                    Bathrooms = 2,
                    LivingAreaM2 = 135,
                    PlotSizeM2 = 210,
                    YearBuilt = 2006,
                    EnergyLabel = "A",
                    Description = "Modern family home close to dunes and schools.",
                    Status = HouseStatus.ForSale,
                    CreatedAtUtc = now
                }
            };

            _db.Houses.AddRange(houses);
            await _db.SaveChangesAsync();

            return houses.Select(h => HouseMapper.ToDetailDto(h, new List<HousePhotoDto>())).ToList();
        }

        public string GetPhotoUrl(Guid houseId, Guid photoId)
        {
            return _photoUrlService.GetHousePhotoUrl(houseId, photoId);
        }
    }
}
