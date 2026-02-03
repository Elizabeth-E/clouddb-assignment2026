using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BuyMyHouse.Api.Services;
using Shared.Models.Dto;

namespace BuyMyHouse.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HousesController : ControllerBase
    {
        private readonly IHousesService _service;

        public HousesController(IHousesService service)
        {
            _service = service;
        }

        // GET /api/houses?minPrice=...&maxPrice=...&city=...&page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<HouseSummaryDto>>> Search(
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? city,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.SearchAsync(minPrice, maxPrice, city, page, pageSize);
            return Ok(result);
        }

        // GET /api/houses/{houseId}
        [HttpGet("{houseId:guid}")]
        public async Task<ActionResult<HouseDetailDto>> GetById([FromRoute] Guid houseId)
        {
            var result = await _service.GetByIdAsync(houseId);
            if (result is null) return NotFound();

            return Ok(result);
        }

        // POST /api/houses
        [HttpPost]
        public async Task<ActionResult<HouseDetailDto>> Create([FromBody] CreateHouseRequestDto request)
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { houseId = result.HouseId }, result);
        }

        // POST /api/houses/seed
        [HttpPost("seed")]
        public async Task<ActionResult> Seed()
        {
            var result = await _service.SeedAsync();
            return Ok(result);
        }

        // PUT /api/houses/{houseId}
        [HttpPut("{houseId:guid}")]
        public async Task<ActionResult<HouseDetailDto>> Update(
            [FromRoute] Guid houseId,
            [FromBody] UpdateHouseRequestDto request)
        {
            var result = await _service.UpdateAsync(houseId, request);
            if (result is null) return NotFound();

            return Ok(result);
        }

        // DELETE /api/houses/{houseId}
        [HttpDelete("{houseId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid houseId)
        {
            var deleted = await _service.DeleteAsync(houseId);
            if (!deleted) return NotFound();

            return NoContent();
        }

        // GET /api/houses/{houseId}/photos
        [HttpGet("{houseId:guid}/photos")]
        public async Task<ActionResult> GetPhotos([FromRoute] Guid houseId)
        {
            var photos = await _service.GetPhotosAsync(houseId);
            if (photos is null) return NotFound();

            return Ok(photos);
        }

        // GET /api/houses/{houseId}/photos/{photoId}
        // (Optional: if you want your API to stream/proxy the image instead of returning SAS URLs)
        // This can return a redirect to SAS URL or stream content.
        [HttpGet("{houseId:guid}/photos/{photoId:guid}")]
        public ActionResult GetPhoto([FromRoute] Guid houseId, [FromRoute] Guid photoId)
        {
            var url = _service.GetPhotoUrl(houseId, photoId);
            return Redirect(url);
        }
    }
}
