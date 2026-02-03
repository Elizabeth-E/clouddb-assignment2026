using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BuyMyHouse.Api.Services;
using Shared.Models.Dto;

namespace BuyMyHouse.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MortgageOffersController : ControllerBase
    {
        private readonly IMortgageOffersService _service;

        public MortgageOffersController(IMortgageOffersService service)
        {
            _service = service;
        }

        // GET /api/mortgageoffers?applicationId=...&applicantId=...&page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<MortgageOfferDto>>> Search(
            [FromQuery] Guid? applicationId,
            [FromQuery] Guid? applicantId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.SearchAsync(applicationId, applicantId, page, pageSize);
            return Ok(result);
        }

        // GET /api/mortgageoffers/{offerId}
        [HttpGet("{offerId:guid}")]
        public async Task<ActionResult<MortgageOfferDto>> GetById([FromRoute] Guid offerId)
        {
            var result = await _service.GetByIdAsync(offerId);
            if (result is null) return NotFound();

            return Ok(result);
        }

        // GET /api/mortgageoffers/{offerId}/view
        [HttpGet("{offerId:guid}/view")]
        public async Task<ActionResult<OfferViewDto>> GetView([FromRoute] Guid offerId)
        {
            var result = await _service.GetViewAsync(offerId);
            if (result is null) return NotFound();

            return Ok(result);
        }
    }
}
