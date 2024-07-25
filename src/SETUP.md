# Project Setup
## Overview

This project provides an API for retrieving information about countries, regions, and languages. It uses ASP.NET Core with dependency injection, memory caching, and HTTP client services. The data is fetched from the [REST Countries API](https://restcountries.com/) and is cached in memory to improve performance.

## Prerequisites
- .NET SDK 6.0 or later
- Visual Studio 2019 or later / Visual Studio Code
- Internet access (for fetching data from the REST Countries API)

## Installation and Setup
Follow these steps to set up and run the project locally:

### Clone the Repo
Clone the repository to your local machine using the following command:

```bash
git clone https://github.com/AdoraNwodo/country-api.git
cd country-api
```

### Install Dependencies
Restore the .NET dependencies by running::
```bash
dotnet restore
```

### Configuration
The project uses the `appsettings.json` file for configuration (e.g. cache settings)

Ensure the `appsettings.json` file is configured as follows:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "CacheSettings": {
    "CacheKey": "Countries",
    "CacheDurationInHours": 24
  },
  "AllowedHosts": "*"
}
```

Remember to set `CacheKey` and `CacheDurationInHours` to your desired values.

### Run the Application
To run the application, execute the following command:
```bash
dotnet run
```

### Accessing the API
Once the application is running, it can be accessed at https://localhost:7028/api/countries or a similar URL depending on your local configuration.

The API includes the following endpoints:

- **GET /api/countries:** Retrieves a list of countries with optional sorting and filtering.
- **GET /api/countries/{code}:** Retrieves a specific country by its code.
- **GET /api/regions:** Retrieves a list of regions and the countries within each region.
- **GET /api/languages:** Retrieves a list of languages spoken and the countries where they are spoken.

### Testing
To run the tests, ensure the test project is included in your solution, then use:
```
dotnet test
```