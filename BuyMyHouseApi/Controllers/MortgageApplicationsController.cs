using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BuyMyHouse.Api.Services;
using Shared.Models.Dto;
using Shared.Models.Enums;

namespace BuyMyHouse.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MortgageApplicationsController : ControllerBase
    {
        private readonly IMortgageApplicationsService _service;

        public MortgageApplicationsController(IMortgageApplicationsService service)
        {
            _service = service;
        }

        // GET /api/mortgageapplications?applicantId=...&status=...&page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<MortgageApplicationDto>>> Search(
            [FromQuery] Guid? applicantId,
            [FromQuery] ApplicationStatus? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.SearchAsync(applicantId, status, page, pageSize);
            return Ok(result);
        }

        // GET /api/mortgageapplications/{applicationId}
        [HttpGet("{applicationId:guid}")]
        public async Task<ActionResult<MortgageApplicationDto>> GetById([FromRoute] Guid applicationId)
        {
            var result = await _service.GetByIdAsync(applicationId);
            if (result is null) return NotFound();

            return Ok(result);
        }

        // POST /api/mortgageapplications
        [HttpPost]
        public async Task<ActionResult<MortgageApplicationDto>> Create(
            [FromBody] CreateMortgageApplicationRequestDto request)
        {
            var result = await _service.CreateAsync(request);

            if (result.Status == MortgageApplicationsCreateStatus.Ok)
            {
                var submitResult = await _service.SubmitAsync(result.Application!.ApplicationId);
                if (submitResult.Status == MortgageApplicationsSubmitStatus.Ok)
                {
                    return CreatedAtAction(nameof(GetById), new { applicationId = submitResult.Application!.ApplicationId }, submitResult.Application);
                }
            }

            return result.Status switch
            {
                MortgageApplicationsCreateStatus.HouseNotFound => NotFound(),
                MortgageApplicationsCreateStatus.ApplicantNotFound => NotFound(),
                _ => NotFound()
            };
        }

        // PUT /api/mortgageapplications/{applicationId}
        [HttpPut("{applicationId:guid}")]
        public async Task<ActionResult<MortgageApplicationDto>> Update(
            [FromRoute] Guid applicationId,
            [FromBody] UpdateMortgageApplicationRequestDto request)
        {
            var result = await _service.UpdateAsync(applicationId, request);
            if (result.Status == MortgageApplicationsUpdateStatus.NotFound) return NotFound();

            return Ok(result.Application);
        }

        // PATCH /api/mortgageapplications/{applicationId}/status
        [HttpPatch("{applicationId:guid}/status")]
        public async Task<ActionResult<MortgageApplicationDto>> UpdateStatus(
            [FromRoute] Guid applicationId,
            [FromBody] UpdateMortgageApplicationStatusRequestDto request)
        {
            var result = await _service.UpdateStatusAsync(applicationId, request);
            if (result.Status == MortgageApplicationsStatusUpdateStatus.NotFound) return NotFound();

            return Ok(result.Application);
        }

        // GET /api/mortgageapplications/{applicationId}/income
        [HttpGet("{applicationId:guid}/income")]
        public async Task<ActionResult<IncomeRecordDto>> GetIncome([FromRoute] Guid applicationId)
        {
            var result = await _service.GetIncomeAsync(applicationId);

            return result.Status switch
            {
                MortgageApplicationsIncomeStatus.Ok => Ok(result.Income),
                MortgageApplicationsIncomeStatus.ApplicationNotFound => NotFound(),
                MortgageApplicationsIncomeStatus.IncomeNotFound => NotFound(),
                _ => NotFound()
            };
        }

        // DELETE /api/mortgageapplications/{applicationId}
        [HttpDelete("{applicationId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid applicationId)
        {
            var result = await _service.DeleteAsync(applicationId);
            if (result == MortgageApplicationsDeleteStatus.NotFound) return NotFound();

            return NoContent();
        }
    }
}
