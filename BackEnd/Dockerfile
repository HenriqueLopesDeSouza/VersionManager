FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copia todos os projetos
COPY ./src ./src

# restore apontando direto pro csproj
RUN dotnet restore "./src/VersionManager.Api/VersionManager.Api.csproj"

# publish apontando direto pro csproj
RUN dotnet publish "./src/VersionManager.Api/VersionManager.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "VersionManager.Api.dll"]
