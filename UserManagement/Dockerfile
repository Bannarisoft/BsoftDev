# -----------------------
# Stage 1: Build
# -----------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-ltsc2022 AS build
WORKDIR /src

# Copy everything and publish
COPY . .
RUN dotnet publish ./UserManagement.API/UserManagement.API.csproj -c Release -o /app/publish

# -----------------------
# Stage 2: Runtime
# -----------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-ltsc2022
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Set env vars
ENV ASPNETCORE_URLS=http://+:5001
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Expose gRPC port
EXPOSE 5001

# Run the application
ENTRYPOINT ["dotnet", "UserManagement.API.dll"]
