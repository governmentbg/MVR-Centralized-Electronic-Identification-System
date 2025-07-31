FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG EID_NUGET_REPO_URL
WORKDIR /src

COPY "./eID.Signing.sln" "eID.Signing.sln"

COPY ["./eID.Signing.API/eID.Signing.API.csproj", "eID.Signing.API/"]
COPY ["./eID.Signing.API.Public/eID.Signing.API.Public.csproj", "eID.Signing.API.Public/"]
COPY ["./eID.Signing.Application/eID.Signing.Application.csproj", "eID.Signing.Application/"]
COPY ["./eID.Signing.Service/eID.Signing.Service.csproj", "eID.Signing.Service/"]
COPY ["./eID.Signing.Contracts/eID.Signing.Contracts.csproj", "eID.Signing.Contracts/"]

#Copy Unit tests
COPY ["./tests/unit/eID.Signing.UnitTests/eID.Signing.UnitTests.csproj", "tests/unit/eID.Signing.UnitTests/"]

COPY ["./NuGet.config", "nuget.config"]

RUN dotnet restore "eID.Signing.sln"
COPY . .
WORKDIR "/src/eID.Signing.Application"
RUN dotnet publish "eID.Signing.Application.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sh", "-c", "update-ca-certificates && exec dotnet eID.Signing.Application.dll"]