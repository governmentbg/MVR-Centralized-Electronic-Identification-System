FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

ARG EID_NUGET_REPO_URL
ARG EID_NPM_REPO_URL
COPY "./eID.PJS.LocalLogsSearch.sln" "eID.PJS.LocalLogsSearch.sln"
COPY ["./eID.PJS.LocalLogsSearch.API/eID.PJS.LocalLogsSearch.API.csproj", "eID.PJS.LocalLogsSearch.API/"]
COPY ["./eID.PJS.LocalLogsSearch.Service/eID.PJS.LocalLogsSearch.Service.csproj", "eID.PJS.LocalLogsSearch.Service/"]
#Copy Unit tests
COPY ["./eID.PJS.LocalLogsSearch.Tests/eID.PJS.LocalLogsSearch.Tests.csproj", "eID.PJS.LocalLogsSearch.Tests/"]

RUN dotnet restore "eID.PJS.LocalLogsSearch.sln"
COPY . .
WORKDIR "/src/eID.PJS.LocalLogsSearch.API"
RUN dotnet publish "eID.PJS.LocalLogsSearch.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eID.PJS.LocalLogsSearch.API.dll"]