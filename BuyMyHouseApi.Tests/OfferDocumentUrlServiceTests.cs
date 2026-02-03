using System.Collections.Generic;
using BuyMyHouse.Api.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BuyMyHouseApi.Tests
{
    public class OfferDocumentUrlServiceTests
    {
        [Fact]
        public void GetOfferDocumentUrl_UsesBaseUrlAndContainer()
        {
            var settings = new Dictionary<string, string?>
            {
                ["Blob:BaseUrl"] = "http://localhost:10000/devstoreaccount1",
                ["Blob:OfferDocumentsContainer"] = "mortgage-offers"
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var service = new OfferDocumentUrlService(configuration);

            var url = service.GetOfferDocumentUrl("offers/applicant/abc.json");

            Assert.Equal("http://localhost:10000/devstoreaccount1/mortgage-offers/offers/applicant/abc.json", url);
        }
    }
}
