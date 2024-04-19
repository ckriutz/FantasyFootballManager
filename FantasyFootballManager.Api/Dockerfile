FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS base
WORKDIR /app
EXPOSE 5288
ENV ASPNETCORE_URLS=http://+:5288

FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /src
COPY ["FantasyFootballManager.Api.csproj", "./"]
RUN dotnet restore "FantasyFootballManager.Api.csproj"
COPY . .
RUN dotnet publish "FantasyFootballManager.Api.csproj" --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FantasyFootballManager.Api.dll"]
