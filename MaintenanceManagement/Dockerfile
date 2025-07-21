# -----------------------
# Stage 1: Build
# -----------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-ltsc2022 AS build
WORKDIR /src

# Copy everything
COPY . .

# Publish the application
RUN dotnet publish ./MaintenanceManagement.API/MaintenanceManagement.API.csproj -c Release -o /app/publish

# -----------------------
# Stage 2: Runtime
# -----------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-ltsc2022
WORKDIR /app

# Copy published files from build
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5002
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Expose port for gRPC or HTTP
EXPOSE 5002

# Launch the app
ENTRYPOINT ["dotnet", "MaintenanceManagement.API.dll"]
