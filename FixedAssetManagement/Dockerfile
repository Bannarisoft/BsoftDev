FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ["FixedAssetManagement/FixedAssetManagement.sln", "./"]
COPY ["FixedAssetManagement/", "FixedAssetManagement/"]
COPY ["Shared/Contracts/", "Shared/Contracts/"]

WORKDIR /app/FixedAssetManagement
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5194
ENTRYPOINT ["dotnet", "FixedAssetManagement.dll"]
