FROM mcr.microsoft.com/dotnet/aspnet:6.0

ARG APP_DIR=/opt/application

# Copy all binaries
COPY ./application ${APP_DIR}

# Set working directory and execution point for image
WORKDIR ${APP_DIR}

ENTRYPOINT ["dotnet", "eID.PJS.LocalLogsSearch.API.dll"]