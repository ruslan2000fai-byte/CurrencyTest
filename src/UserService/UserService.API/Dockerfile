FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/UserService/UserService.Domain/UserService.Domain.csproj         src/UserService/UserService.Domain/
COPY src/UserService/UserService.Application/UserService.Application.csproj src/UserService/UserService.Application/
COPY src/UserService/UserService.Infrastructure/UserService.Infrastructure.csproj src/UserService/UserService.Infrastructure/
COPY src/UserService/UserService.API/UserService.API.csproj               src/UserService/UserService.API/
RUN dotnet restore src/UserService/UserService.API/UserService.API.csproj
COPY src/UserService/ src/UserService/
RUN dotnet publish src/UserService/UserService.API/UserService.API.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "UserService.API.dll"]
