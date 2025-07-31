FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG EID_NUGET_REPO_URL
ARG EID_NUGET_ORG
WORKDIR /src

COPY "./eID.PAN.sln" "eID.PAN.sln"

COPY ["./eID.PAN.API/eID.PAN.API.csproj", "eID.PAN.API/"]
COPY ["./eID.PAN.API.Public/eID.PAN.API.Public.csproj", "eID.PAN.API.Public/"]
COPY ["./eID.PAN.Application/eID.PAN.Application.csproj", "eID.PAN.Application/"]
COPY ["./eID.PAN.Service/eID.PAN.Service.csproj", "eID.PAN.Service/"]
COPY ["./eID.PAN.Contracts/eID.PAN.Contracts.csproj", "eID.PAN.Contracts/"]

#Copy Unit tests
COPY ["./tests/unit/eID.PAN.UnitTests/eID.PAN.UnitTests.csproj", "tests/unit/eID.PAN.UnitTests/"]

COPY ["./NuGet.config", "nuget.config"]

RUN dotnet restore "eID.PAN.sln"
COPY . .
WORKDIR "/src/eID.PAN.API.Public"
RUN dotnet publish "eID.PAN.API.Public.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sh", "-c", "update-ca-certificates && exec dotnet eID.PAN.API.Public.dll"]