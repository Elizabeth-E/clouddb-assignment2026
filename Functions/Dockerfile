FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Functions/Functions.csproj Functions/
COPY Shared/Shared.csproj Shared/
RUN dotnet restore Functions/Functions.csproj

COPY . .
WORKDIR /src/Functions
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0
WORKDIR /home/site/wwwroot
COPY --from=build /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
