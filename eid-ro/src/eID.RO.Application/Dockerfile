FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
ARG EID_NPM_REPO_URL
ARG EID_NUGET_REPO_URL
COPY "./eID.RO.sln" "eID.RO.sln"

COPY ["./eID.RO.API/eID.RO.API.csproj", "eID.RO.API/"]
COPY ["./eID.RO.API.Public/eID.RO.API.Public.csproj", "eID.RO.API.Public/"]
COPY ["./eID.RO.Application/eID.RO.Application.csproj", "eID.RO.Application/"]
COPY ["./eID.RO.Service/eID.RO.Service.csproj", "eID.RO.Service/"]
COPY ["./eID.RO.Contracts/eID.RO.Contracts.csproj", "eID.RO.Contracts/"]

#Copy Unit tests
COPY ["./tests/unit/eID.RO.UnitTests/eID.RO.UnitTests.csproj", "tests/unit/eID.RO.UnitTests/"]

COPY ["./NuGet.config", "nuget.config"]

RUN dotnet restore "eID.RO.sln"
COPY . .
WORKDIR "/src/eID.RO.Application"
RUN dotnet publish "eID.RO.Application.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sh", "-c", "update-ca-certificates && exec dotnet eID.RO.Application.dll"]