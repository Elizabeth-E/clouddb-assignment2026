using System;
using BuyMyHouse.Api.Mappings;
using Shared.Models.Entities;
using Shared.Models.Enums;
using Xunit;

namespace BuyMyHouseApi.Tests
{
    public class MortgageOfferMapperTests
    {
        [Fact]
        public void ToDto_MapsDocumentUrl()
        {
            var offer = new MortgageOfferEntity
            {
                OfferId = Guid.NewGuid(),
                ApplicationId = Guid.NewGuid(),
                GeneratedAtUtc = DateTime.UtcNow,
                ValidUntilUtc = DateTime.UtcNow.AddDays(7),
                Decision = OfferDecision.Approved,
                ApprovedLoanAmount = 250000m,
                InterestRate = 3.5m,
                MonthlyPayment = 1150m,
                TermYears = 30,
                Notes = "Test",
                DocumentBlobKey = "offers/x/y.json"
            };

            var dto = MortgageOfferMapper.ToDto(offer, "http://localhost:10000/devstoreaccount1/mortgage-offers/offers/x/y.json");

            Assert.Equal(offer.OfferId, dto.OfferId);
            Assert.Equal("http://localhost:10000/devstoreaccount1/mortgage-offers/offers/x/y.json", dto.DocumentDownloadUrl);
        }
    }
}
