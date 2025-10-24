# Fulxa - ETL

Fluxa, an ETL software for Bussiness Inelligense, Data Integration and Data Analaysis purposes.

---

## 📋 Table of Contents

- [About the Project](#-about-the-project)
- [Architecture](#-architecture)
- [Technologies](#-technologies)
- [Getting Started](#-getting-started)
- [Environment Variables](#-environment-variables)
- [Docker](#-docker)
- [API Documentation](#-api-documentation)
- [License](#-license)
<!-- - [Contact](#-contact) -->

---

## 🌟 About the Project

This is a **Clean Architecture** project for an ASP.NET Core API. It's designed to be a solid foundation for building scalable, maintainable, and testable applications. The architecture separates the application into distinct layers, ensuring a clear separation of concerns and reducing coupling between components.

The project is structured with the following key layers:

- **Domain**: This is the core of the application, containing the business logic and domain entities. It has no dependencies on other layers.
- **Application**: This layer handles the application's use cases, orchestrating domain objects to fulfill business requirements. It depends on the Domain layer.
- **Infrastructure**: This layer provides the implementation for interfaces defined in the Application layer, such as database access, file systems, and external services. It depends on the Application and Domain layers.
- **API (ASP.NET)**: This is the entry point for the application, exposing functionality through a RESTful API. It depends on the Application and Infrastructure layers.

---

## 🏗️ Architecture

The solution is organized into the following projects:

- `ETL.Domain`: Contains the core business logic, entities, and interfaces.
- `ETL.Application`: Implements the use cases and application services.
- `ETL.Infrastructure`: Provides concrete implementations for data access and other services.
- `ETL.API`: The ASP.NET Core API project.

---

## 💻 Technologies

- [ASP.NET Core 9](https://dotnet.microsoft.com/en-us/apps/aspnet)
- [Angular 18](https://angular.io/)
- [MediatR](https://github.com/jbogard/MediatR)
- [AutoMapper](https://automapper.org/)
- [Dapper](https://github.com/DapperLib/Dapper), [SQLKata](https://sqlkata.com/)
- [FluentValidation](https://fluentvalidation.net/)
- [Xunit](https://xunit.net/), [FluentAssertions](https://fluentassertions.com/), & [NSubstitude](https://nsubstitute.github.io)
- [Docker](https://www.docker.com/)

---

## 🚀 Getting Started

### Prerequisites

- .NET 9 SDK
- PostgreSQL
- Docker (optional, for running with Docker Compose)

### Running Locally

1.  **Clone the repository:**

    ```bash
    git clone https://github.com/Star-Academy/Summer1404-Project-Team02.git
    ```

2.  **Restore dependencies:**

    ```bash
    dotnet restore
    ```

3.  **Update the connection string:**
    Ensure your `appsettings.json` file in the `ETL.API` project has the correct PostgreSQL connection string.

4.  **Run migrations:**

    ```bash
    dotnet ef database update --project ETL.Infrastructure --startup-project ETL.API
    ```

5.  **Run the API:**
    ```bash
    dotnet run --project ETL.API
    ```
    The API will be available at `http://localhost:8080` (or the port configured in `launchSettings.json`).

---

## ⚙️ Environment Variables

The application uses environment variables for configuration. You'll need to set the following variables to run the application correctly:

| Variable                               | Description                                        | Example Value                                                                                         |
| :------------------------------------- | :------------------------------------------------- | :---------------------------------------------------------------------------------------------------- |
| `DOTNET_ENVIRONMENT`                   | Development/Prodcuction.                           | `Development`                                                                                         |
| `Authentication__Authority`            | The base URL of the Keycloak authorization server. | `http://localhost:8080/realms/[realm-name]`                                                           |
| `Authentication__ClientId`             | Client ID for the backend application.             | `server-client`                                                                                       |
| `Authentication__ClientSecret`         | Client secret for the backend application.         | `???`                                                                                                 |
| `Authentication__RedirectUri`          | Redirect URI of frontend.                          | `http://localhost:4200`                                                                               |
| `Authentication__KeycloakBaseUrl`      | The base URL of Keycloak.                          | `http://localhost:8080`                                                                               |
| `Authentication__Realm`                | The Keycloak realm.                                | `team2`                                                                                               |
| `KeycloakAdmin__ClientId`              | Client ID for Keycloak administration.             | `admin-client`                                                                                        |
| `KeycloakAdmin__ClientSecret`          | Client secret for Keycloak administration.         | `???`                                                                                                 |
| `ConnectionStrings__DefaultConnection` | The database connection string.                    | `Host=localhost;Port=5432;Database=[your-database];Username=[your-username];Password=[your-password]` |

---

## 🐳 Docker

This repository includes a `Dockerfile` to build a container image of the API and a `docker-compose.yml` file to run the API and its dependencies (like the database) in a containerized environment.

- **Dockerfile Link**: [`Dockerfile`](https://github.com/Star-Academy/Summer1404-Project-Team02/etl-server/Dockerfile)

### Build the Docker image

```bash
docker build -t your-image-name .
```

You can add `-e DOTNET_ENVIRONMENT=Development` flag, if you want to run in development environment.

### Run with Docker Compose

1.  **Update the `docker-compose.yml` file**:
    Ensure the environment variables match your configuration.

2.  **Run the containers:**

    ```bash
    docker-compose up --build
    ```

---

## 📖 API Documentation

The API documentation is available via Scalar UI. Once the application is running, navigate to:

```
http://localhost:8080/scalar
```

<!-- -----

## 📧 Contact

Your Name - your.email@example.com

Project Link: [https://github.com/yourusername/your-repository](https://www.google.com/search?q=https://github.com/yourusername/your-repository)

----- -->

## ⚖️ License

Distributed under the MIT License. See `LICENSE.md` for more information.
