using Data.EF;
using Microsoft.EntityFrameworkCore;
using BuyMyHouse.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// EF Core + SQL Server (connection string in appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IHousesService, HousesService>();
builder.Services.AddScoped<IApplicantsService, ApplicantsService>();
builder.Services.AddScoped<IMortgageApplicationsService, MortgageApplicationsService>();
builder.Services.AddScoped<IMortgageOffersService, MortgageOffersService>();
builder.Services.AddSingleton<IPhotoUrlService, PhotoUrlService>();
builder.Services.AddSingleton<IOfferDocumentUrlService, OfferDocumentUrlService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
