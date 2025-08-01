FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
ARG EID_NUGET_REPO_URL

COPY "./eID.PDEAU.sln" "eID.PDEAU.sln"

COPY ["./eID.PDEAU.API/eID.PDEAU.API.csproj", "eID.PDEAU.API/"]
COPY ["./eID.PDEAU.API.Public/eID.PDEAU.API.Public.csproj", "eID.PDEAU.API.Public/"]
COPY ["./eID.PDEAU.API.Public.Admin/eID.PDEAU.API.Public.Admin.csproj", "eID.PDEAU.API.Public.Admin/"]
COPY ["./eID.PDEAU.Application/eID.PDEAU.Application.csproj", "eID.PDEAU.Application/"]
COPY ["./eID.PDEAU.Service/eID.PDEAU.Service.csproj", "eID.PDEAU.Service/"]
COPY ["./eID.PDEAU.Contracts/eID.PDEAU.Contracts.csproj", "eID.PDEAU.Contracts/"]

#Copy Unit tests
COPY ["./tests/unit/eID.PDEAU.UnitTests/eID.PDEAU.UnitTests.csproj", "tests/unit/eID.PDEAU.UnitTests/"]

COPY ["./NuGet.config", "nuget.config"]

RUN dotnet restore "eID.PDEAU.sln"
COPY . .
WORKDIR "/src/eID.PDEAU.API.Public"
RUN dotnet publish "eID.PDEAU.API.Public.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sh", "-c", "update-ca-certificates && exec dotnet eID.PDEAU.API.Public.dll"]