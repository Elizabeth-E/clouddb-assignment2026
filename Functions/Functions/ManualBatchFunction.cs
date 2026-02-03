using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Functions.Services;

namespace Functions.Functions
{
    public class ManualBatchFunction
    {
        private readonly BatchProcessingService _batchProcessingService;

        public ManualBatchFunction(BatchProcessingService batchProcessingService)
        {
            _batchProcessingService = batchProcessingService;
        }

        [Function("ManualBatch")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "batch/run")] HttpRequestData req,
            FunctionContext executionContext,
            CancellationToken cancellationToken)
        {
            var summary = await _batchProcessingService.RunAsync(cancellationToken);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(summary, cancellationToken);

            return response;
        }
    }
}
