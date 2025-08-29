# ----------------------------
# Build Stage
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files first (better cache)
COPY UserManagement/UserManagement.sln UserManagement/
COPY UserManagement/UserManagement.API/UserManagement.API.csproj UserManagement/UserManagement.API/
COPY UserManagement/UserManagement.Infrastructure/UserManagement.Infrastructure.csproj UserManagement/UserManagement.Infrastructure/
COPY UserManagement/UserManagement.SecurityTests/UserManagement.SecurityTests.csproj UserManagement/UserManagement.SecurityTests/
COPY UserManagement/UserManagement.Tests/UserManagement.Tests.csproj UserManagement/UserManagement.Tests/
COPY UserManagement/Core/Core.Application/Core.Application.csproj UserManagement/Core/Core.Application/
COPY UserManagement/Core/Core.Domain/Core.Domain.csproj UserManagement/Core/Core.Domain/
COPY Shared/Contracts/Contracts.csproj Shared/Contracts/
COPY Shared/Shared.Infrastructure/Shared.Infrastructure.csproj Shared/Shared.Infrastructure/
COPY BackgroundService/BackgroundService.Infrastructure/BackgroundService.Infrastructure.csproj BackgroundService/BackgroundService.Infrastructure/

# (Optional) if your build depends on proto files at restore time
COPY Shared/Contracts/Protos/ Shared/Contracts/Protos/

# Restore
WORKDIR /src/UserManagement
RUN dotnet restore UserManagement.sln

# Copy the full sources
WORKDIR /src
COPY UserManagement/ UserManagement/
COPY Shared/ Shared/
COPY BackgroundService/ BackgroundService/

# Sanity check for a required proto (optional)
RUN test -f Shared/Contracts/Protos/department_maintenance.proto || (echo "❌ Proto file missing!" && exit 1)

# Publish (no app host = smaller image)
RUN dotnet publish UserManagement/UserManagement.API/UserManagement.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Ensure runtime config files are present (you load settings/*.json at runtime)
RUN if [ -d "/src/UserManagement/UserManagement.API/settings" ]; then \
  mkdir -p /app/publish/settings && cp -r /src/UserManagement/UserManagement.API/settings/* /app/publish/settings/; \
  fi

# ----------------------------
# Runtime Stage
# ----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# In-cluster we run HTTP; TLS is handled by ingress if needed
ENV ASPNETCORE_ENVIRONMENT=Container

# REST
EXPOSE 5174/tcp
# gRPC (h2c)
EXPOSE 7031/tcp

ENTRYPOINT ["dotnet", "UserManagement.API.dll"]
