# Use the official image as a parent image.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 3992

# Use the official image from the .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0.203-cbl-mariner2.0 AS build
WORKDIR /src

# Copy the .csproj and restore as distinct layers
COPY ["FantasyFootballManager.Api.csproj", "api/"]
RUN dotnet restore "api/FantasyFootballManager.Api.csproj"

# Copy everything else and build the project
COPY . .
#WORKDIR "/src/api"
RUN dotnet build "FantasyFootballManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FantasyFootballManager.Api.csproj" -c Release -o /app/publish

# Final stage / image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FantasyFootballManager.Api.dll"]