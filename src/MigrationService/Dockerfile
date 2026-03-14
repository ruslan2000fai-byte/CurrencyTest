FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/MigrationService/MigrationService.csproj src/MigrationService/
RUN dotnet restore src/MigrationService/MigrationService.csproj
COPY src/MigrationService/ src/MigrationService/
RUN dotnet publish src/MigrationService/MigrationService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MigrationService.dll"]
