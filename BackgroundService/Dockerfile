# Use .NET SDK image to build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the solution file first
COPY BackgroundService.sln ./

# ✅ Use "Core/" instead of "CORE/"
COPY Core/BackgroundService.Application/BackgroundService.Application.csproj ./Core/BackgroundService.Application/
COPY Core/BackgroundService.Domain/BackgroundService.Domain.csproj ./Core/BackgroundService.Domain/
COPY BackgroundService.Infrastructure/BackgroundService.Infrastructure.csproj ./BackgroundService.Infrastructure/
COPY BackgroundService.API/BackgroundService.API.csproj ./BackgroundService.API/

# Run restore **only on csproj files**
RUN dotnet restore

# Copy all source code and build
COPY . .
RUN dotnet publish -c Release -o out

# Use runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 5002
CMD ["dotnet", "BackgroundService.API.dll"]
