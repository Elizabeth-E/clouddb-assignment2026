using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models.Dto;

namespace BuyMyHouse.Api.Services
{
    public interface IHousesService
    {
        Task<PagedResultDto<HouseSummaryDto>> SearchAsync(decimal? minPrice, decimal? maxPrice, string? city, int page, int pageSize);
        Task<HouseDetailDto?> GetByIdAsync(Guid houseId);
        Task<IReadOnlyList<HousePhotoDto>?> GetPhotosAsync(Guid houseId);
        Task<HouseDetailDto> CreateAsync(CreateHouseRequestDto request);
        Task<HouseDetailDto?> UpdateAsync(Guid houseId, UpdateHouseRequestDto request);
        Task<bool> DeleteAsync(Guid houseId);
        Task<IReadOnlyList<HouseDetailDto>> SeedAsync();
        string GetPhotoUrl(Guid houseId, Guid photoId);
    }
}
