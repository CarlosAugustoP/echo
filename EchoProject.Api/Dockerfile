FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY EchoProject.sln ./
COPY EchoProject.Api/EchoProject.Api.csproj EchoProject.Api/
COPY EchoProject.Application/EchoProject.Application.csproj EchoProject.Application/
COPY EchoProject.Domain/EchoProject.Domain.csproj EchoProject.Domain/
COPY EchoProject.Infrastructure/EchoProject.Infrastructure.csproj EchoProject.Infrastructure/
RUN dotnet restore EchoProject.Api/EchoProject.Api.csproj

COPY . .
RUN dotnet publish EchoProject.Api/EchoProject.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EchoProject.Api.dll"]
