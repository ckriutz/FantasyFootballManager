# Use the official image from the .NET SDK
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0.203-jammy AS build
ARG TARGETARCH
WORKDIR /src

# Copy the .csproj and restore as distinct layers
COPY . .
RUN dotnet restore "FantasyFootballManager.DataService.csproj" -a $TARGETARCH

RUN dotnet publish "FantasyFootballManager.DataService.csproj" -a $TARGETARCH --no-restore -c Release -o /app/publish

# Final stage / image
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0.3-jammy AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 3992
ENTRYPOINT ["dotnet", "FantasyFootballManager.DataService.dll"]