FROM mcr.microsoft.com/dotnet/aspnet:8.0-cbl-mariner2.0 AS base
WORKDIR /app
EXPOSE 5288

ENV ASPNETCORE_URLS=http://+:5288

FROM mcr.microsoft.com/dotnet/sdk:8.0-cbl-mariner2.0 AS build
WORKDIR /src
COPY ["FantasyFootballManager.Api.csproj", "./"]
RUN dotnet restore "FantasyFootballManager.Api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "FantasyFootballManager.Api.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "FantasyFootballManager.Api.csproj" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FantasyFootballManager.Api.dll"]
