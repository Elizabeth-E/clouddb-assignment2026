using Azure.Storage.Blobs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Functions.Options;
using Functions.Services;

static string MustGet(string name) =>
    Environment.GetEnvironmentVariable(name)
    ?? throw new InvalidOperationException($"Missing environment variable: {name}");

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.Configure<FunctionsOptions>(options =>
        {
            options.OfferDocumentsContainer = MustGet("OfferDocumentsContainer");
        });

        // Blob
        services.AddSingleton(_ => new BlobServiceClient(MustGet("BlobConnection")));

        // SQL factory
        services.AddSingleton<Func<SqlConnection>>(_ =>
            () => new SqlConnection(MustGet("SqlConnection")));

        services.AddSingleton<OfferGenerationService>();
        services.AddSingleton<BatchProcessingService>();
    })
    .Build();

host.Run();
