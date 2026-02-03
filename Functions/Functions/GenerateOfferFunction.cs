using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Functions.Services;

namespace Functions.Functions
{
    public class GenerateOfferFunction
    {
        private readonly OfferGenerationService _offerGenerationService;

        public GenerateOfferFunction(OfferGenerationService offerGenerationService)
        {
            _offerGenerationService = offerGenerationService;
        }

        [Function("GenerateOffer")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "offers/{applicationId:guid}/generate")] HttpRequestData req,
            Guid applicationId,
            FunctionContext executionContext,
            CancellationToken cancellationToken)
        {
            var result = await _offerGenerationService.GenerateAsync(applicationId, cancellationToken);

            var response = req.CreateResponse(result.NotFound ? HttpStatusCode.NotFound : HttpStatusCode.OK);

            if (!result.NotFound)
            {
                await response.WriteAsJsonAsync(new
                {
                    result.OfferId,
                    result.DocumentBlobKey
                }, cancellationToken);
            }

            return response;
        }
    }
}
