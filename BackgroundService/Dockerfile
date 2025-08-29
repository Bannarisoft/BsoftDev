# ----------------------------
# Build Stage
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src/BackgroundService

# Solution + project files (cache-friendly restore)
COPY BackgroundService/BackgroundService.sln ./
COPY BackgroundService/BackgroundService.API/BackgroundService.API.csproj ./BackgroundService.API/
COPY BackgroundService/BackgroundService.Application/BackgroundService.Application.csproj ./BackgroundService.Application/
COPY BackgroundService/BackgroundService.Infrastructure/BackgroundService.Infrastructure.csproj ./BackgroundService.Infrastructure/
COPY BackgroundService/BackgroundService.Domain/BackgroundService.Domain.csproj ./BackgroundService.Domain/

# Shared projects
COPY Shared/Contracts/Contracts.csproj ../Shared/Contracts/
COPY Shared/Shared.Infrastructure/Shared.Infrastructure.csproj ../Shared/Shared.Infrastructure/
COPY Shared/Contracts/Protos/ ../Shared/Contracts/Protos/

# 🔹 UserManagement deps
COPY UserManagement/Core/Core.Application/Core.Application.csproj ../UserManagement/Core/Core.Application/
# 🔹 MISSING BEFORE: Core.Domain (needed by Core.Application)
COPY UserManagement/Core/Core.Domain/Core.Domain.csproj ../UserManagement/Core/Core.Domain/

# Restore
RUN dotnet restore "BackgroundService.sln"

# Copy full source AFTER restore
COPY BackgroundService/ ./
COPY Shared/ ../Shared/
COPY UserManagement/Core/Core.Application/ ../UserManagement/Core/Core.Application/
# 🔹 Copy Core.Domain sources too
COPY UserManagement/Core/Core.Domain/ ../UserManagement/Core/Core.Domain/

# Optional sanity check
RUN test -f ../Shared/Contracts/Protos/department.proto || (echo "❌ department.proto missing!" && exit 1)

# Publish
RUN dotnet publish "BackgroundService.API/BackgroundService.API.csproj" -c Release -o /app/publish

# ----------------------------
# Runtime Stage
# ----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Container
ENV ASPNETCORE_URLS="http://+:5011"

EXPOSE 5011
# EXPOSE 7111  # only if you run HTTPS locally with that port

ENTRYPOINT ["dotnet", "BackgroundService.API.dll"]
