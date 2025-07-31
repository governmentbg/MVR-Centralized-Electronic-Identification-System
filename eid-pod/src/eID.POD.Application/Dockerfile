FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
ARG EID_NPM_REPO_URL
ARG EID_NUGET_REPO_URL
COPY "./eID.POD.sln" "eID.POD.sln"
COPY ["./eID.POD.API/eID.POD.API.csproj", "eID.POD.API/"]
COPY ["./eID.POD.Application/eID.POD.Application.csproj", "eID.POD.Application/"]
COPY ["./eID.POD.Service/eID.POD.Service.csproj", "eID.POD.Service/"]
COPY ["./eID.POD.Contracts/eID.POD.Contracts.csproj", "eID.POD.Contracts/"]

#Copy Unit tests
COPY ["./tests/unit/eID.POD.UnitTests/eID.POD.UnitTests.csproj", "tests/unit/eID.POD.UnitTests/"]

COPY ["./NuGet.config", "nuget.config"]

RUN dotnet restore "eID.POD.sln"
COPY . .
WORKDIR "/src/eID.POD.Application"
RUN dotnet publish "eID.POD.Application.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#RUN echo 'extra/management_ca.pem' >> /etc/ca-certificates.conf
ENTRYPOINT ["sh", "-c", "update-ca-certificates && exec dotnet eID.POD.Application.dll"]