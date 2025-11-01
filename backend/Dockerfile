# Backend Dockerfile for .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and project files
COPY ["backend/backend.csproj", "backend/"]
COPY ["Shop.Shared/Shop.Shared.csproj", "Shop.Shared/"]

# Restore dependencies
RUN dotnet restore "backend/backend.csproj"

# Copy everything else
COPY backend/ backend/
COPY Shop.Shared/ Shop.Shared/

# Build the project
WORKDIR "/src/backend"
RUN dotnet build "backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "backend.dll"]
