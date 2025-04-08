FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WeatherSubscriptionWebApp.Api/WeatherSubscriptionWebApp.Api.csproj", "WeatherSubscriptionWebApp.Api/"]
COPY ["WeatherSubscriptionWebApp.Application/WeatherSubscriptionWebApp.Application.csproj", "WeatherSubscriptionWebApp.Application/"]
COPY ["WeatherSubscriptionWebApp.Domain/WeatherSubscriptionWebApp.Domain.csproj", "WeatherSubscriptionWebApp.Domain/"]
COPY ["WeatherSubscriptionWebApp.Infrastructure/WeatherSubscriptionWebApp.Infrastructure.csproj", "WeatherSubscriptionWebApp.Infrastructure/"]
RUN dotnet restore "WeatherSubscriptionWebApp.Api/WeatherSubscriptionWebApp.Api.csproj"
COPY . .
WORKDIR "/src/WeatherSubscriptionWebApp.Api"
RUN dotnet build "WeatherSubscriptionWebApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WeatherSubscriptionWebApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeatherSubscriptionWebApp.Api.dll"]
