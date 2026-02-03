using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BuyMyHouse.Api.Services;
using Shared.Models.Dto;

namespace BuyMyHouse.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicantsController : ControllerBase
    {
        private readonly IApplicantsService _service;

        public ApplicantsController(IApplicantsService service)
        {
            _service = service;
        }

        // GET /api/applicants?email=...&page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<ApplicantDto>>> Search(
            [FromQuery] string? email,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.SearchAsync(email, page, pageSize);
            return Ok(result);
        }

        // GET /api/applicants/{applicantId}
        [HttpGet("{applicantId:guid}")]
        public async Task<ActionResult<ApplicantDto>> GetById([FromRoute] Guid applicantId)
        {
            var result = await _service.GetByIdAsync(applicantId);
            if (result is null) return NotFound();

            return Ok(result);
        }

        // POST /api/applicants
        [HttpPost]
        public async Task<ActionResult<ApplicantDto>> Create([FromBody] CreateApplicantRequestDto request)
        {
            var result = await _service.CreateAsync(request);
            if (result.Status == ApplicantsCreateStatus.Conflict) return Conflict("Applicant with this email already exists.");

            return CreatedAtAction(nameof(GetById), new { applicantId = result.Applicant!.ApplicantId }, result.Applicant);
        }
    }
}
