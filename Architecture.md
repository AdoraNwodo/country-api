# CountryAPI Project Architecture

## Overview

The architecture of the CountryAPI project is designed for scalability, maintainability, and performance. This backend API uses .NET and incorporates coding best practices for serving country-related data.

## Components

### 1. **User/Console Interface**
   - **Description**: The entry point for users to interact with the API. Users send HTTP requests to the API to retrieve information about countries, regions, and languages. This interface ([CountryAPISimulator](./src/CountryApiSimulator/)) was built as a Console App, and it makes 10,000 simultaneous calls for each endpoint. Apart from the [CountryAPISimulator](./src/CountryApiSimulator/), you can also interact with the API through the browser or tools like postman.    
   - **Interactions**: Sends requests to the API Layer.

### 2. **API Layer (CountryAPI)**
   - **Description**: The core application built using ASP.NET Core. This layer handles all incoming requests, processes them, and sends back appropriate responses.
   - **Key Features**:
     - **Routing and Endpoints**: Manages various API routes for country data.
     - **Core Business Logic**: The `RestCountriesService` class houses the core business logic for each endpoint.
   - **Interactions**:
     - Receives HTTP requests from the User Interface.
     - Checks the Cache Layer for cached data.
     - If data is not cached, fetches it from the External API.
     - Caches the data and sends the response back to the User Interface.

### 3. **Cache Layer**
   - **Description**: The application also has In-memory caching. `IMemoryCache` stores frequently accessed data, reducing the need for repeated external API calls and improving response times.
   - **Interactions**:
     - Provides cached data to the API Layer when available.
     - Receives data from the API Layer to store for future requests.

### 4. **External API (REST Countries API)**
   - **Description**: A third-party API that provides detailed information about countries. The CountryAPI uses this as a data source.
   - **Interactions**:
     - The API Layer queries this service to fetch country data when it is not available in the cache.

###  5. **Middleware Layer**
  - **Description**: A layer of middleware that processes HTTP requests and responses. It includes custom middleware for tasks such as global error handling and serving standard JSON responses.
  - **Key Features**: 
      - **Global Error Handling**: Custom middleware captures 404 errors and other exceptions, providing informative JSON responses to users This middleware provides a friendly message at invalid URLs. This goal of this is to guide users to valid endpoints and documentation.
  - **Interactions**: It sits between the API Layer and the request/response cycle for validation.

### 5. **CI/CD Pipeline (GitHub Actions)**
   - **Description**: Automated workflows that handle the build, test, and Azure deployment processes.
   - **Key Features**:
     - **Continuous Integration**: Automatically builds and tests the application on each commit.
     - **Continuous Deployment**: Deploys the Azure infrastructure and also deploys the application to the Azure infra after successful tests.
   - **Interactions**:
     - Integrates with the source code repository to trigger workflows.

### 6. **Infrastructure (Azure)**
   - **Description**: The cloud environment where the application is hosted. Azure provides scalable and secure hosting for the CountryAPI.
   - **Components**:
     - **App Service**: Hosts the API application.
     - **Potential Additional Services**: Could include distributed caching and monitoring services.
   - **Interactions**:
     - Deploys the API application and manages resources.

## Data Flow

1. **Request Initiation**: Users send requests via the User Interface.
2. **API Processing**: The API Layer processes the request, checking the Cache Layer for existing data.
3. **Data Retrieval**: If data is not cached, the API Layer fetches it from the External API.
4. **Caching**: Retrieved data is cached for future requests.
5. **Response**: The API Layer sends the processed data back to the User Interface.
6. **Continuous Integration**: Changes to the codebase trigger the CI/CD pipeline, which builds, tests, and deploys the application.

This architecture shows the different components, how they interact with eachother, and how data flows.
