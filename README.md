# Weather Subscription API

A production‑ready RESTful web API built using ASP.NET Core that allows users to subscribe for weather updates and retrieve current weather data from OpenWeatherMap. The project demonstrates best practices with a layered architecture, SOLID principles, advanced logging and error handling, caching, CQRS/MediatR pattern, advanced validation, containerization, and thorough unit testing.

## Overview

The Weather Subscription API enables users to create subscriptions using their email address and location details (including full country name, city, and optionally ZIP code with an ISO country code). Once subscribed, users can log in (using only their email) and retrieve current weather data for their saved location. The API integrates with the OpenWeatherMap API using geocoding to convert location data into coordinates before querying weather information.

## Key Features

- **Subscription Management:**  
  - Create and retrieve user subscriptions.
  - Store subscription data in a lightweight database (SQLite).
  - Enforced uniqueness by email.

- **Weather Data Retrieval:**  
  - Login using email.
  - Retrieve current weather data (weather description, temperature, pressure, humidity, wind speed, cloudiness, sunrise and sunset times).
  - Uses both ZIP-based geocoding (if ZIP code and ISO country code are provided) or city/country-based geocoding.

- **Advanced Validation:**  
  - Uses FluentValidation for strong, centralized validation of incoming DTOs.

- **CQRS / Mediator Pattern:**  
  - Implements command and query separation using MediatR for cleaner business logic separation.

- **Caching:**  
  - Implements in‑memory caching for weather data to reduce external API calls.
  - Easily switchable to a distributed caching solution for horizontal scaling.

- **Logging & Error Handling:**  
  - Global error handling using custom middleware and RFC 7807 ProblemDetails.
  - Structured logging using Serilog with enrichment and correlation IDs.
  
- **Unit Testing:**  
  - Thorough unit tests with xUnit, Moq, and FluentAssertions across all layers (Domain, Application, Infrastructure, API).

- **Containerization:**  
  - Containerized using Docker with a multi‑stage Dockerfile.
  - Docker‑Compose configuration to integrate with external dependencies like Redis if needed.

## Architecture

The solution is organized into multiple layers for a clean separation of concerns:

- **Domain:**  
  - Contains core business entities (e.g., Subscription), shared DTOs, and external service interfaces (e.g., IWeatherClient, IGeoCodingService, IWeatherResponseParser).

- **Application:**  
  - Implements business logic and orchestration using services and the CQRS pattern via MediatR.
  - Handles commands (e.g., CreateSubscriptionCommand) and queries (e.g., GetSubscriptionByEmailQuery) through dedicated handlers.

- **Infrastructure:**  
  - Provides concrete implementations for external integrations, including:
    - Entity Framework Core for data persistence with SQLite.
    - OpenWeatherMapClient for weather data retrieval.
    - OpenWeatherMapGeoCodingService and OpenWeatherMapResponseParser.
  - Implements caching (IMemoryCache) and logging integration.

- **API:**  
  - Exposes RESTful endpoints through controllers.
  - Implements global error handling middleware.
  - Configures advanced validation (FluentValidation).
  - Integrates AutoMapper for clean DTO-to-domain mapping.
  - Configures dependency injection, Serilog, Swagger/OpenAPI, and containerization.


## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started) (for containerization)
- A code editor like Visual Studio, Visual Studio Code, or JetBrains Rider

## Setup and Installation

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/javidhasanzade/weather-subscription-api
   cd WeatherSubscriptionWebApp
    ```
2. **Restore NuGet Packages:**
```bash
dotnet restore
```

## Running the Application
To run the application locally:
```bash
dotnet run --project WeatherSubscriptionWebApp.Api
```

## Running Unit Tests
Run the tests using the following command:
```bash
dotnet test WeatherSubscriptionWebApp.Tests
```

## Possible Further Improvements
1. **Expand the CQRS approach using MediatR, adding more pipeline behaviors (such as logging, validation, and performance monitoring).**
2. **Integrate a distributed cache like Redis for improved scalability across multiple instances.**
3. **Implement Polly policies for circuit breaker and retry logic on external API calls.**
4. **Secure the API endpoints with JWT-based authentication or OAuth2.**
