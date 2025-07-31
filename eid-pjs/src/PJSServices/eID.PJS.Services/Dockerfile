FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

ARG EID_NUGET_REPO_URL
ARG EID_NPM_REPO_URL
COPY "./eID.PJS.Services.sln" "eID.PJS.Services.sln"
COPY ["./eID.PJS.Services/eID.PJS.Services.csproj", "eID.PJS.Services/"]
COPY ["./AuditLogGenerator/AuditLogGenerator.csproj", "AuditLogGenerator/"]
#Copy Unit tests
COPY ["./eID.PJS.Services.Tests/eID.PJS.Services.Tests.csproj", "eID.PJS.Services.Tests/"]
COPY ["./NuGet.config", "nuget.config"]

RUN dotnet restore "eID.PJS.Services.sln"
COPY . .
WORKDIR "/src/eID.PJS.Services"
RUN dotnet publish "eID.PJS.Services.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eID.PJS.Services.dll"]