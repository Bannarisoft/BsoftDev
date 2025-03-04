FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ["SagaOrchestrator/SagaOrchestrator.sln", "./"]
COPY ["SagaOrchestrator/", "SagaOrchestrator/"]
COPY ["Shared/Contracts/", "Shared/Contracts/"]

WORKDIR /app/SagaOrchestrator
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5192
ENTRYPOINT ["dotnet", "SagaOrchestrator.dll"]
