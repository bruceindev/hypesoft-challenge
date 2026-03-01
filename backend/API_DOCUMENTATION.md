# Hypesoft API - Complete CRUD Implementation

## Architecture

This project follows Clean Architecture principles with the following layers:
- **Hypesoft.Domain**: Entities and Repository Interfaces
- **Hypesoft.Application**: Commands, Queries, Handlers, DTOs
- **Hypesoft.Infrastructure**: Repository Implementations, Database Context
- **Hypesoft.API**: Controllers, Middleware, Configuration

## Technology Stack

- .NET 9.0
- MediatR for CQRS pattern
- Entity Framework Core with SQL Server
- Swagger/OpenAPI
- Health Checks

## API Endpoints

### Products

#### Create Product
```
POST /api/products
Content-Type: application/json

{
  "name": "Product Name",
  "description": "Product Description",
  "price": 99.90,
  "categoryId": "guid",
  "stockQuantity": 100
}
```

#### Get All Products (Paginated)
```
GET /api/products?pageNumber=1&pageSize=10&searchTerm=&categoryId=
```

#### Get Product By ID
```
GET /api/products/{id}
```

#### Update Product
```
PUT /api/products/{id}
Content-Type: application/json

{
  "name": "Updated Name",
  "description": "Updated Description",
  "price": 149.90
}
```

#### Delete Product
```
DELETE /api/products/{id}
```

#### Update Product Stock
```
PATCH /api/products/{id}/stock
Content-Type: application/json

{
  "stockQuantity": 50
}
```

#### Get Low Stock Products
```
GET /api/products/low-stock?threshold=10
```

#### Get Total Stock Value
```
GET /api/products/total-stock-value
```

#### Get Total Products Count
```
GET /api/products/count
```

### Categories

#### Create Category
```
POST /api/categories
Content-Type: application/json

{
  "name": "Category Name"
}
```

#### Get All Categories
```
GET /api/categories
```

#### Get Category By ID
```
GET /api/categories/{id}
```

#### Update Category
```
PUT /api/categories/{id}
Content-Type: application/json

{
  "name": "Updated Category Name"
}
```

#### Delete Category
```
DELETE /api/categories/{id}
```
Note: Cannot delete category if it has products associated.

### Dashboard

#### Get Dashboard Statistics
```
GET /api/dashboard/stats

Response:
{
  "totalProducts": 100,
  "totalStockValue": 15000.50,
  "lowStockProductsCount": 5
}
```

### Health

#### Health Check
```
GET /api/health

GET /health (Microsoft Health Checks endpoint)
```

## Features Implemented

### Product Management
- ✅ Create product
- ✅ Get product by ID
- ✅ Update product
- ✅ Delete product
- ✅ Search by name
- ✅ Filter by category
- ✅ Pagination
- ✅ Manual stock update endpoint
- ✅ List low stock products (StockQuantity < 10)
- ✅ Total stock value calculation

### Category Management
- ✅ Create category
- ✅ List categories
- ✅ Update category
- ✅ Delete category
- ✅ Prevent deletion if category has products

### Stock Control
- ✅ Stock quantity cannot be negative
- ✅ Required fields validation
- ✅ Business rule validation in handlers

### Dashboard Support
- ✅ Total number of products
- ✅ Total stock value
- ✅ Products with low stock count

### Infrastructure
- ✅ Clean Architecture implementation
- ✅ MediatR for CQRS
- ✅ DTOs (never return domain entities)
- ✅ Async/await with CancellationToken
- ✅ REST conventions
- ✅ Proper HTTP status codes
- ✅ CreatedAtAction for POST
- ✅ NotFound for missing entities
- ✅ BadRequest for validation failures
- ✅ AsNoTracking for read queries
- ✅ Global exception handling middleware
- ✅ Security headers configuration
- ✅ CORS configuration
- ✅ Health checks
- ✅ Prepared for Keycloak integration

## Running the Application

```bash
dotnet restore
dotnet build
dotnet run --project src/Hypesoft.API
```

Access Swagger UI: `http://localhost:5047/swagger`

## Error Handling

The API uses global exception handling middleware that returns consistent error responses:

- `400 Bad Request`: ArgumentException (validation errors)
- `404 Not Found`: KeyNotFoundException (entity not found)
- `409 Conflict`: InvalidOperationException (business rule violations)
- `500 Internal Server Error`: Unhandled exceptions

## Security

- Security headers configured (X-Content-Type-Options, X-Frame-Options, X-XSS-Protection, Referrer-Policy)
- CORS enabled for development
- Authentication/Authorization middleware prepared for Keycloak integration

## Future Enhancements

- Keycloak authentication integration
- Rate limiting (ready for .NET 9 built-in support)
- Response caching
- API versioning
- Distributed caching with Redis
