FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG EID_NUGET_REPO_URL
WORKDIR /src

COPY "./eID.PIVR.sln" "eID.PIVR.sln"

COPY ["./eID.PIVR.API/eID.PIVR.API.csproj", "eID.PIVR.API/"]
COPY ["./eID.PIVR.Application/eID.PIVR.Application.csproj", "eID.PIVR.Application/"]
COPY ["./eID.PIVR.Service/eID.PIVR.Service.csproj", "eID.PIVR.Service/"]
COPY ["./eID.PIVR.Contracts/eID.PIVR.Contracts.csproj", "eID.PIVR.Contracts/"]

#Copy Unit tests
COPY ["./tests/unit/eID.PIVR.UnitTests/eID.PIVR.UnitTests.csproj", "tests/unit/eID.PIVR.UnitTests/"]

COPY ["./NuGet.config", "nuget.config"]

RUN dotnet restore "eID.PIVR.sln"
COPY . .
WORKDIR "/src/eID.PIVR.API"
RUN dotnet publish "eID.PIVR.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sh", "-c", "update-ca-certificates && exec dotnet eID.PIVR.API.dll"]