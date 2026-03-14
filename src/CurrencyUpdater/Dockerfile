FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/CurrencyUpdater/CurrencyUpdater.csproj src/CurrencyUpdater/
RUN dotnet restore src/CurrencyUpdater/CurrencyUpdater.csproj
COPY src/CurrencyUpdater/ src/CurrencyUpdater/
RUN dotnet publish src/CurrencyUpdater/CurrencyUpdater.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "CurrencyUpdater.dll"]
