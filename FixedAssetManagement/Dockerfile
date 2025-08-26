# ----------------------------
# Build Stage
# ----------------------------
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src
    
    # Copy solution & project files first (good cache)
    COPY FixedAssetManagement/FixedAssetManagement.sln FixedAssetManagement/
    COPY FixedAssetManagement/FAM.API/FAM.API.csproj FixedAssetManagement/FAM.API/
    COPY FixedAssetManagement/FAM.Infrastructure/FAM.Infrastructure.csproj FixedAssetManagement/FAM.Infrastructure/
    COPY FixedAssetManagement/Core/Core.Domain/Core.Domain.csproj FixedAssetManagement/Core/Core.Domain/
    COPY FixedAssetManagement/Core/Core.Application/Core.Application.csproj FixedAssetManagement/Core/Core.Application/
    COPY Shared/Contracts/Contracts.csproj Shared/Contracts/
    COPY Shared/Shared.Infrastructure/Shared.Infrastructure.csproj Shared/Shared.Infrastructure/
    COPY Shared/Contracts/Protos/ Shared/Contracts/Protos/
    
    # (If any ProjectReferences use absolute "/Shared/..." paths, mirror them)
    COPY Shared/Contracts/Contracts.csproj /Shared/Contracts/
    COPY Shared/Shared.Infrastructure/Shared.Infrastructure.csproj /Shared/Shared.Infrastructure/
    
    # Restore
    WORKDIR /src/FixedAssetManagement
    RUN dotnet restore FixedAssetManagement.sln
    
    # Copy full sources
    WORKDIR /src
    COPY FixedAssetManagement/ FixedAssetManagement/
    COPY Shared/ Shared/
    # Mirror Shared to /Shared too (only needed if absolute refs exist)
    COPY Shared/ /Shared/
    
    # Sanity check for required proto
    RUN test -f Shared/Contracts/Protos/department.proto || (echo "❌ department.proto missing!" && exit 1)
    
    # Publish (no app host = smaller image)
    RUN dotnet publish FixedAssetManagement/FAM.API/FAM.API.csproj -c Release -o /app/publish /p:UseAppHost=false
    
    # Copy runtime settings if you load settings/*.json at runtime
    RUN if [ -d "/src/FixedAssetManagement/FAM.API/settings" ]; then \
          mkdir -p /app/publish/settings && cp -r /src/FixedAssetManagement/FAM.API/settings/* /app/publish/settings/; \
        fi
    
    # ----------------------------
    # Runtime Stage
    # ----------------------------
    FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
    WORKDIR /app
    
    COPY --from=build /app/publish .
    
    # In-cluster we run HTTP; TLS is handled by ingress if needed
    ENV ASPNETCORE_ENVIRONMENT=Container
    
    # REST
    EXPOSE 5194/tcp
    # gRPC (h2c)
    EXPOSE 7039/tcp
    
    ENTRYPOINT ["dotnet", "FAM.API.dll"]
    