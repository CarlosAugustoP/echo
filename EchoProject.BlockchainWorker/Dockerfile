FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY EchoProject.sln ./
COPY EchoProject.BlockchainWorker/EchoProject.BlockchainWorker.csproj EchoProject.BlockchainWorker/
COPY EchoProject.Application/EchoProject.Application.csproj EchoProject.Application/
COPY EchoProject.Domain/EchoProject.Domain.csproj EchoProject.Domain/
COPY EchoProject.Infrastructure/EchoProject.Infrastructure.csproj EchoProject.Infrastructure/
RUN dotnet restore EchoProject.BlockchainWorker/EchoProject.BlockchainWorker.csproj

COPY . .
RUN dotnet publish EchoProject.BlockchainWorker/EchoProject.BlockchainWorker.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EchoProject.BlockchainWorker.dll"]
