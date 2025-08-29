# ----------------------------
# Build Stage
# ----------------------------
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src
    
    # Copy solution & project files first (better cache)
    COPY SagaOrchestrator/SagaOrchestrator.sln SagaOrchestrator/
    COPY SagaOrchestrator/SagaOrchestrator.API/SagaOrchestrator.API.csproj SagaOrchestrator/SagaOrchestrator.API/
    COPY SagaOrchestrator/SagaOrchestrator.Application/SagaOrchestrator.Application.csproj SagaOrchestrator/SagaOrchestrator.Application/
    COPY SagaOrchestrator/SagaOrchestrator.Infrastructure/SagaOrchestrator.Infrastructure.csproj SagaOrchestrator/SagaOrchestrator.Infrastructure/
    COPY SagaOrchestrator/SagaOrchestrator.Domain/SagaOrchestrator.Domain.csproj SagaOrchestrator/SagaOrchestrator.Domain/
    COPY Shared/Contracts/Contracts.csproj Shared/Contracts/
    COPY Shared/Shared.Infrastructure/Shared.Infrastructure.csproj Shared/Shared.Infrastructure/
    COPY Shared/Contracts/Protos/ Shared/Contracts/Protos/
    
    # If any ProjectReferences use absolute "/Shared/..." paths, mirror them too
    COPY Shared/Contracts/Contracts.csproj /Shared/Contracts/
    COPY Shared/Shared.Infrastructure/Shared.Infrastructure.csproj /Shared/Shared.Infrastructure/
    
    # Restore
    WORKDIR /src/SagaOrchestrator
    RUN dotnet restore SagaOrchestrator.sln
    
    # Copy full sources
    WORKDIR /src
    COPY SagaOrchestrator/ SagaOrchestrator/
    COPY Shared/ Shared/
    # Mirror Shared to /Shared as well (only needed if absolute refs exist)
    COPY Shared/ /Shared/
    
    # Sanity check for required proto (optional)
    RUN test -f Shared/Contracts/Protos/department.proto || (echo "❌ department.proto missing!" && exit 1)
    
    # Publish (no app host = smaller image)
    RUN dotnet publish SagaOrchestrator/SagaOrchestrator.API/SagaOrchestrator.API.csproj \
        -c Release -o /app/publish /p:UseAppHost=false
    
    # Copy runtime settings if your app loads settings/*.json at runtime
    RUN if [ -d "/src/SagaOrchestrator/SagaOrchestrator.API/settings" ]; then \
          mkdir -p /app/publish/settings && cp -r /src/SagaOrchestrator/SagaOrchestrator.API/settings/* /app/publish/settings/; \
        fi
    
    # ----------------------------
    # Runtime Stage
    # ----------------------------
    FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
    WORKDIR /app
    
    COPY --from=build /app/publish .
    
    # In-cluster we run HTTP; TLS is handled by ingress if needed
    ENV ASPNETCORE_ENVIRONMENT=Container
    
    # (Ports are bound via Kestrel/env in your Deployment; EXPOSE is optional)
    # Example if you know them:
    # EXPOSE 5xxx/tcp   # REST
    # EXPOSE 7xxx/tcp   # gRPC (h2c)
    
    ENTRYPOINT ["dotnet", "SagaOrchestrator.API.dll"]
    