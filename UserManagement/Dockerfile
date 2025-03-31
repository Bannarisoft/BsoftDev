# Use .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the solution file first
COPY UserManagement.sln ./

# Copy the project files before restoring dependencies
COPY Core/Core.Application/*.csproj ./Core/Core.Application/
COPY UserManagement.Infrastructure/*.csproj ./UserManagement.Infrastructure/
COPY UserManagement.API/*.csproj ./UserManagement.API/
COPY Core/Core.Domain/*.csproj ./Core/Core.Domain/

# Run restore **only on csproj files**
RUN dotnet restore

# Copy all other source code and build
COPY . .
RUN dotnet publish -c Release -o out

# Use runtime image for execution
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 5001
CMD ["dotnet", "UserManagement.API.dll"]
