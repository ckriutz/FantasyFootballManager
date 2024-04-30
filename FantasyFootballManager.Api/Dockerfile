# Use the official image as a parent image.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 3992
ARG TARGETARCH

# Use the official image from the .NET SDK
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0.203-cbl-mariner2.0 AS build
WORKDIR /src
ARG TARGETARCH

# Copy the .csproj and restore as distinct layers
COPY ["FantasyFootballManager.Api.csproj", "api/"]
RUN dotnet restore "api/FantasyFootballManager.Api.csproj" -a $TARGETARCH

# Copy everything else and build the project
COPY . .
#WORKDIR "/src/api"
RUN dotnet build "FantasyFootballManager.Api.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FantasyFootballManager.Api.csproj" -a $TARGETARCH -c Release -o /app/publish

# Final stage / image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FantasyFootballManager.Api.dll"]