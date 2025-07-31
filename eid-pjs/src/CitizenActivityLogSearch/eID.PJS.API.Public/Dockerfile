FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
ARG EID_NUGET_REPO_URL
ARG EID_NPM_REPO_URL
COPY "./eID.PJS.sln" "eID.PJS.sln"
COPY ["./eID.PJS.API/eID.PJS.API.csproj", "eID.PJS.API/"]
COPY ["./eID.PJS.API.Public/eID.PJS.API.Public.csproj", "eID.PJS.API.Public/"]
COPY ["./eID.PJS.Application/eID.PJS.Application.csproj", "eID.PJS.Application/"]
COPY ["./eID.PJS.Service/eID.PJS.Service.csproj", "eID.PJS.Service/"]
COPY ["./eID.PJS.Contracts/eID.PJS.Contracts.csproj", "eID.PJS.Contracts/"]
#Copy Unit tests
COPY ["./tests/unit/eID.PJS.UnitTests/eID.PJS.UnitTests.csproj", "tests/unit/eID.PJS.UnitTests/"]
COPY ["./NuGet.config", "nuget.config"]

RUN dotnet restore "eID.PJS.sln"
COPY . .
WORKDIR "/src/eID.PJS.API.Public"
RUN dotnet publish "eID.PJS.API.Public.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sh", "-c", "update-ca-certificates && exec dotnet eID.PJS.API.Public.dll"]