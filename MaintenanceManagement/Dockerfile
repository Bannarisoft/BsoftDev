# ----------------------------
# Build Stage
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution & project files first (better cache)
COPY MaintenanceManagement/MaintenanceManagement.sln MaintenanceManagement/
COPY MaintenanceManagement/MaintenanceManagement.API/MaintenanceManagement.API.csproj MaintenanceManagement/MaintenanceManagement.API/
COPY MaintenanceManagement/MaintenanceManagement.Infrastructure/MaintenanceManagement.Infrastructure.csproj MaintenanceManagement/MaintenanceManagement.Infrastructure/
COPY MaintenanceManagement/MaintenanceManagement.Tests/MaintenanceManagement.Tests.csproj MaintenanceManagement/MaintenanceManagement.Tests/
COPY MaintenanceManagement/Core/Core.Application/Core.Application.csproj MaintenanceManagement/Core/Core.Application/
COPY MaintenanceManagement/Core/Core.Domain/Core.Domain.csproj MaintenanceManagement/Core/Core.Domain/
COPY Shared/Contracts/Contracts.csproj Shared/Contracts/
COPY Shared/Shared.Infrastructure/Shared.Infrastructure.csproj Shared/Shared.Infrastructure/
COPY Shared/Contracts/Protos/ Shared/Contracts/Protos/

# (If some ProjectReferences use absolute "/Shared/..." paths, mirror them)
COPY Shared/Contracts/Contracts.csproj /Shared/Contracts/
COPY Shared/Shared.Infrastructure/Shared.Infrastructure.csproj /Shared/Shared.Infrastructure/

# Restore
WORKDIR /src/MaintenanceManagement
RUN dotnet restore MaintenanceManagement.sln

# Copy full sources
WORKDIR /src
COPY MaintenanceManagement/ MaintenanceManagement/
COPY Shared/ Shared/

# Mirror Shared trees to /Shared so absolute refs compile (if any)
COPY Shared/ /Shared/

# Optional sanity check (proto presence)
RUN test -f Shared/Contracts/Protos/department.proto || (echo "❌ department.proto is missing!" && exit 1)

# Publish (no app host = smaller image)
RUN dotnet publish MaintenanceManagement/MaintenanceManagement.API/MaintenanceManagement.API.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# Copy runtime settings if your app loads settings/*.json at runtime
RUN if [ -d "/src/MaintenanceManagement/MaintenanceManagement.API/settings" ]; then \
    mkdir -p /app/publish/settings && cp -r /src/MaintenanceManagement/MaintenanceManagement.API/settings/* /app/publish/settings/; \
    fi

# ----------------------------
# Runtime Stage
# ----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# In-cluster we run HTTP; TLS is handled by ingress if needed
ENV ASPNETCORE_ENVIRONMENT=Container

# Expose actual ports (match your Kestrel bindings / Deployment)
# REST
EXPOSE 5293/tcp
# gRPC (h2c)
EXPOSE 7243/tcp

ENTRYPOINT ["dotnet", "MaintenanceManagement.API.dll"]
