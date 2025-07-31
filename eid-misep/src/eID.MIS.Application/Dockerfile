FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
ARG EID_NUGET_REPO_URL
ARG EID_NUGET_ORG
COPY "./eID.MIS.sln" "eID.MIS.sln"

COPY ["./eID.MIS.API/eID.MIS.API.csproj", "eID.MIS.API/"]
COPY ["./eID.MIS.Application/eID.MIS.Application.csproj", "eID.MIS.Application/"]
COPY ["./eID.MIS.Service/eID.MIS.Service.csproj", "eID.MIS.Service/"]
COPY ["./eID.MIS.Contracts/eID.MIS.Contracts.csproj", "eID.MIS.Contracts/"]

#Copy Unit tests
COPY ["./tests/unit/eID.MIS.UnitTests/eID.MIS.UnitTests.csproj", "tests/unit/eID.MIS.UnitTests/"]

COPY ["./NuGet.config", "nuget.config"]

RUN dotnet restore "eID.MIS.sln"
COPY . .
WORKDIR "/src/eID.MIS.Application"
RUN dotnet publish "eID.MIS.Application.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sh", "-c", "update-ca-certificates && exec dotnet eID.MIS.Application.dll"]