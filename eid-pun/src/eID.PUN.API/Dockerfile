FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
ARG EID_NUGET_REPO_URL
ARG EID_NPM_REPO_URL

COPY "./eID.PUN.sln" "eID.PUN.sln"

COPY ["./eID.PUN.API/eID.PUN.API.csproj", "eID.PUN.API/"]
COPY ["./eID.PUN.Application/eID.PUN.Application.csproj", "eID.PUN.Application/"]
COPY ["./eID.PUN.Service/eID.PUN.Service.csproj", "eID.PUN.Service/"]
COPY ["./eID.PUN.Contracts/eID.PUN.Contracts.csproj", "eID.PUN.Contracts/"]

#Copy Unit tests
COPY ["./tests/unit/eID.PUN.UnitTests/eID.PUN.UnitTests.csproj", "tests/unit/eID.PUN.UnitTests/"]

COPY ["./NuGet.config", "nuget.config"]

RUN dotnet restore "eID.PUN.sln"
COPY . .
WORKDIR "/src/eID.PUN.API"
RUN dotnet publish "eID.PUN.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sh", "-c", "update-ca-certificates && exec dotnet eID.PUN.API.dll"]