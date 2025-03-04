# Use .NET SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY ["UserManagement/UserManagement.sln", "./"]
COPY ["UserManagement/", "UserManagement/"]
COPY ["Shared/Contracts/", "Shared/Contracts/"]

# Restore dependencies
WORKDIR /app/UserManagement
RUN dotnet restore

# Build and publish the application
RUN dotnet publish -c Release -o /app/publish

# Use a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5174
ENTRYPOINT ["dotnet", "UserManagement.dll"]
