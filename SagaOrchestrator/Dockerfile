# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-ltsc2022 AS build
WORKDIR /src
COPY . .
RUN dotnet publish ./SagaOrchestrator.API/SagaOrchestrator.API.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-ltsc2022
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5004
ENV DOTNET_RUNNING_IN_CONTAINER=true
EXPOSE 5004
ENTRYPOINT ["dotnet", "SagaOrchestrator.API.dll"]
