FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first so the package layer is cached until the .csproj changes.
COPY src/Portfolio.Api/Portfolio.Api.csproj src/Portfolio.Api/
RUN dotnet restore src/Portfolio.Api/Portfolio.Api.csproj

COPY src/ src/
RUN dotnet publish src/Portfolio.Api/Portfolio.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Data Protection keys (auth cookie encryption) and uploaded images are persisted in volumes mounted here.
# Creating the folders with the right owner lets the non-root user write to those volumes.
RUN mkdir -p /home/app/.aspnet/DataProtection-Keys /data/uploads && chown -R $APP_UID /home/app/.aspnet /data/uploads

COPY --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Portfolio.Api.dll"]
