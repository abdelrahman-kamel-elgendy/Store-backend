# Store Backend

Store Backend is a .NET 10 ASP.NET Core Web API for managing a product catalog. It provides category and product CRUD endpoints, hierarchical categories, SQL Server persistence through Entity Framework Core, and a consistent JSON response envelope.

## Contents

- [Features and scope](#features-and-scope)
- [Technology](#technology)
- [Solution structure](#solution-structure)
- [Requirements](#requirements)
- [Configuration](#configuration)
- [Run locally](#run-locally)
- [API](#api)
- [Response format and errors](#response-format-and-errors)
- [Data model](#data-model)
- [Validation and business rules](#validation-and-business-rules)
- [Persistence behavior](#persistence-behavior)
- [Development and migrations](#development-and-migrations)
- [Security and limitations](#security-and-limitations)

## Features and scope

- Category and product CRUD APIs.
- Self-referencing categories with parent/child relationships.
- Category list and category-by-ID responses represented as nested category trees.
- Product responses include the category name and associated photo details.
- Pagination for category and product collection endpoints.
- Centralized model validation, service exception mapping, and trace IDs.
- Soft deletion and restore endpoints for catalog entities.
- Development-only OpenAPI document and Swagger UI.

This repository is a catalog backend foundation. It does not currently implement user accounts, authentication, authorization, orders, carts, checkout, payment processing, or photo upload.

## Technology

| Area | Technology |
|---|---|
| Language | C# |
| Runtime and API | .NET 10, ASP.NET Core controllers |
| Database | SQL Server |
| Data access | Entity Framework Core 10 |
| Mapping | AutoMapper |
| Local configuration | DotNetEnv and ASP.NET Core configuration |
| API description | ASP.NET Core OpenAPI and Swagger UI |

## Solution structure

The solution is split into three projects:

- `Store.Core`: domain entities, request and response DTOs, validation rules, and service/repository contracts.
- `Store.Infrastructure`: EF Core `AppDbContext`, entity configuration, migrations, and SQL Server repository implementations.
- `Store.API`: controllers, application services, AutoMapper profiles, API response helpers, middleware, and application startup.

```text
Store-backend/
├── .env.example
├── README.md
├── Store.slnx
├── Store.API/
│   ├── Controllers/
│   │   ├── BaseController.cs
│   │   ├── CategoryController.cs
│   │   └── ProductController.cs
│   ├── DTO/PaginationQuery.cs
│   ├── Exceptions/ApiExceptions.cs.cs
│   ├── Helper/ApiResponse.cs
│   ├── Mapping/
│   │   ├── CategoryMapping.cs
│   │   └── ProductMapping.cs
│   ├── MiddleWare/ExceptionHandlingMiddleware.cs
│   ├── Services/
│   │   ├── CategoryService.cs
│   │   ├── GenericService.cs
│   │   └── ProductService.cs
│   ├── Properties/launchSettings.json
│   ├── Program.cs
│   └── Store.API.csproj
├── Store.Core/
│   ├── DTO/
│   │   ├── Category/
│   │   ├── Photo/
│   │   └── Product/
│   ├── Entities/
│   ├── Interfaces/
│   │   ├── Entities/
│   │   ├── Repositories/
│   │   └── Services/
│   ├── Validation/ValidationRules.cs
│   └── Store.Core.csproj
└── Store.Infrastructure/
    ├── Data/
    │   ├── Config/
    │   ├── Migrations/
    │   └── AppDbContext.cs
    ├── Repositories/
    └── Store.Infrastructure.csproj
```

## Requirements

- .NET 10 SDK
- A reachable SQL Server instance
- Permission to create or update the configured database
- `dotnet-ef` for managing EF Core migrations from the command line

## Configuration

The API requires a connection string named `StoreDb`. Startup fails with a configuration error if it is missing. The repository includes `.env.example` with a local SQL Server example:

```env
ConnectionStrings__StoreDb=Server=localhost;Database=Store;Trusted_Connection=True;TrustServerCertificate=True
```

Create a local `.env` file from the example and adjust the connection string for your SQL Server:

```powershell
Copy-Item .env.example .env
```

`DotNetEnv` loads `.env` from the repository directory tree without overwriting environment variables already present in the process. You can instead set `ConnectionStrings__StoreDb` in your environment or hosting configuration. Never commit real passwords or credentials.

## Run locally

From the repository root:

```powershell
dotnet restore Store.slnx
dotnet build Store.slnx
dotnet run --project Store.API
```

The Development launch profile uses `http://localhost:5189`. When running in Development:

- Swagger UI: `http://localhost:5189/swagger`
- OpenAPI JSON: `http://localhost:5189/openapi/v1.json`

The API redirects HTTP requests to HTTPS when an HTTPS endpoint is configured by the host.

## API

Routes are implemented as MVC controllers. The shared `BaseController` defines the CRUD operations for both resources.

### Categories

Base route: `/api/category`

| Method | Route | Behavior |
|---|---|---|
| `GET` | `/api/category?page=1&pageSize=20` | Returns paged root categories; each root includes its descendant categories in `subCategories`. Pagination applies to roots. |
| `GET` | `/api/category/{id}` | Returns the requested category and its descendant tree. |
| `POST` | `/api/category` | Creates a category. Set `parentCategoryId` to a parent category ID or `null` for a root. |
| `PUT` | `/api/category/{id}` | Updates the category identified by the route ID. The request body does not include the category's own ID. |
| `DELETE` | `/api/category/{id}` | Soft-deletes the category. |
| `PATCH` | `/api/category/{id}/restore` | Restores a soft-deleted category. |

Category tree responses have this shape:

```json
{
  "id": 1,
  "name": "Electronics",
  "description": "Electronic devices and gadgets",
  "parentCategoryId": null,
  "subCategories": [
    {
      "id": 6,
      "name": "Phones",
      "description": "Smartphones and accessories",
      "parentCategoryId": 1,
      "subCategories": []
    }
  ]
}
```

### Products

Base route: `/api/product`

| Method | Route | Behavior |
|---|---|---|
| `GET` | `/api/product?page=1&pageSize=20` | Returns a page of products with category and photo details. |
| `GET` | `/api/product/{id}` | Returns one product with category and photo details. |
| `POST` | `/api/product` | Creates a product. |
| `PUT` | `/api/product/{id}` | Updates the product identified by the route ID. The request body does not include the product's own ID. |
| `DELETE` | `/api/product/{id}` | Soft-deletes the product. |
| `PATCH` | `/api/product/{id}/restore` | Restores a soft-deleted product. |

Pagination defaults to page `1` and page size `20`; the allowed page size is 1–100.

Example create-product request:

```http
POST http://localhost:5189/api/product
Content-Type: application/json

{
  "name": "iPhone 15",
  "description": "Apple smartphone 128GB",
  "price": 999.99,
  "categoryId": 6,
  "photoId": 0
}
```

Example create-category request:

```http
POST http://localhost:5189/api/category
Content-Type: application/json

{
  "name": "Phones",
  "description": "Smartphones and accessories",
  "parentCategoryId": 1
}
```

Product request fields include `name`, `description`, `price`, and `categoryId`. The current request DTO also accepts `photoId`; photo upload/management is not implemented as an API workflow.

## Response format and errors

JSON endpoints use a shared response envelope:

```json
{
  "success": true,
  "statusCode": 200,
  "message": "Products retrieved successfully.",
  "data": [],
  "meta": {
    "pagination": {
      "page": 1,
      "pageSize": 20,
      "totalItems": 0,
      "totalPages": 0,
      "hasPreviousPage": false,
      "hasNextPage": false
    }
  }
}
```

Success responses include `success`, `statusCode`, `message`, and `data`. Paged responses include pagination metadata. Failure responses include a safe message and may include `errors`; trace information is returned in `meta` when available. Internal exception details are logged server-side and are not included as an error-clarification field in responses.

Typical status codes include:

- `200 OK`: successful reads and updates.
- `201 Created`: successful resource creation, with a `Location` header.
- `204 No Content`: successful delete.
- `400 Bad Request`: invalid input or business validation error.
- `404 Not Found`: requested resource or category not found.
- `409 Conflict`: duplicate category/product name.
- `422 Unprocessable Entity`: business rule failure.
- `500 Internal Server Error`: unexpected failure, with a safe client-facing message.

ASP.NET Core model-state failures and service exceptions are converted into the same response envelope. Each error response includes a trace ID when available, which can be used to correlate the response with server logs.

## Data model

The persistence model contains:

- `Category`: name, description, optional parent category, child categories, and products.
- `Product`: name, description, price, required category, and photos.
- `Photo`: image name, URL, main-photo flag, and product ID.

Categories form a self-referencing hierarchy through `ParentCategoryId` and `SubCategories`. Product-to-category deletion is restricted by the database relationship configuration. Product photos are modeled in the domain but there is currently no photo management endpoint.

All three entities derive from `BaseEntity<int>`, which provides an integer ID, audit fields, and soft-delete fields. `AppDbContext` applies audit metadata during save operations and configures a global query filter to exclude soft-deleted entities from normal queries.

## Validation and business rules

Request DTOs use data annotations and `IValidatableObject`; shared text and numeric validation helpers are in `Store.Core/Validation/ValidationRules.cs`. API controllers rely on ASP.NET Core model validation.

Service-level rules include:

- Category names must be unique when creating or updating categories.
- A specified parent category must exist.
- A category cannot be its own parent.
- A product's category must exist.
- Product names must be unique when creating or updating products.
- Page numbers must be positive, and page sizes must be between 1 and 100.

AutoMapper profiles are registered at startup and validated before the application starts, so invalid mapping configuration fails early.

## Persistence behavior

- `DELETE` is a soft delete; it does not physically remove a catalog row.
- Normal queries exclude soft-deleted entities through the EF Core global query filter.
- `PATCH /api/{resource}/{id}/restore` restores a soft-deleted row.
- Category list pagination counts and pages root categories, then includes their descendants in the response tree.
- Product list reads include the product category and photos needed by the response DTO.

## Development and migrations

Build the solution with:

```powershell
dotnet build Store.slnx
```

EF Core migrations are maintained in `Store.Infrastructure/Data/Migrations`. To apply pending migrations to the configured database, install or use the `dotnet-ef` tool and run:

```powershell
dotnet ef database update --project Store.Infrastructure --startup-project Store.API
```

In the current startup configuration, the API does not automatically apply database migrations.

## Security and limitations

- Authentication and authorization are not currently implemented; endpoints should not be considered protected.
- Store secrets outside source control, such as in user secrets, environment variables, or a secure deployment configuration.
- There is no order, cart, checkout, payment, stock/inventory, or media-upload workflow.
- Health checks, distributed tracing, and deployment manifests are not included.
- No repository-level license file is currently present.
