FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/FinanceService/FinanceService.Domain/FinanceService.Domain.csproj             src/FinanceService/FinanceService.Domain/
COPY src/FinanceService/FinanceService.Application/FinanceService.Application.csproj   src/FinanceService/FinanceService.Application/
COPY src/FinanceService/FinanceService.Infrastructure/FinanceService.Infrastructure.csproj src/FinanceService/FinanceService.Infrastructure/
COPY src/FinanceService/FinanceService.API/FinanceService.API.csproj                   src/FinanceService/FinanceService.API/
RUN dotnet restore src/FinanceService/FinanceService.API/FinanceService.API.csproj
COPY src/FinanceService/ src/FinanceService/
RUN dotnet publish src/FinanceService/FinanceService.API/FinanceService.API.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "FinanceService.API.dll"]
