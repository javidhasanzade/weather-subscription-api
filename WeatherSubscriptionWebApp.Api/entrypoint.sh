#!/bin/bash
set -e

echo "Applying EF Core migrations..."

dotnet ef database update --project WeatherSubscriptionWebApp.Infrastructure --startup-project WeatherSubscriptionWebApp.Api --msbuildprojectextensionspath obj
echo "Migrations applied. Starting the application..."

exec dotnet WeatherSubscriptionWebApp.Api.dll
