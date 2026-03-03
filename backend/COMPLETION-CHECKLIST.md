# ✅ HYPESOFT BACKEND - COMPLETION CHECKLIST

Use this checklist to track progress toward 70/100 minimum score.

---

## 📋 PHASE 1: FIX APPLICATION LAYER (Target: 6 hours)

### Commands - Products

- [ ] **UpdateProductCommand.cs**
  - [ ] Change `Guid Id` → `string Id`
  - [ ] Change `Guid CategoryId` → `string CategoryId`
  - [ ] Test: `dotnet build`

- [ ] **UpdateProductCommandValidator.cs**
  - [ ] Create new file
  - [ ] Copy pattern from WORKING-EXAMPLE.md
  - [ ] Add validation rules
  - [ ] Test: Build succeeds

- [ ] **UpdateProductHandler.cs**
  - [ ] Remove manual validation
  - [ ] Use AutoMapper
  - [ ] Fix type parameters
  - [ ] Test: Build succeeds

- [ ] **DeleteProductCommand.cs**
  - [ ] Change `Guid Id` → `string Id`

- [ ] **DeleteProductHandler.cs**
  - [ ] Fix type parameters
  - [ ] Test: Build succeeds

- [ ] **UpdateProductStockCommand.cs**
  - [ ] Change `Guid Id` → `string Id`

- [ ] **UpdateProductStockHandler.cs**
  - [ ] Fix type parameters
  - [ ] Test: Build succeeds

### Queries - Products

- [ ] **GetProductsQuery.cs**
  - [ ] Change `Guid? CategoryId` → `string? CategoryId`
  - [ ] Add pagination properties
  - [ ] Test: Build succeeds

- [ ] **GetProductsHandler.cs**
  - [ ] Add caching logic
  - [ ] Implement search/filter
  - [ ] Use AutoMapper
  - [ ] Test: Build succeeds

- [ ] **GetProductByIdQuery.cs**
  - [ ] Change `Guid Id` → `string Id`

- [ ] **GetProductByIdHandler.cs**
  - [ ] Fix type parameters
  - [ ] Use AutoMapper
  - [ ] Test: Build succeeds

- [ ] **GetLowStockProductsQuery.cs**
  - [ ] Verify implementation

- [ ] **GetLowStockProductsHandler.cs**
  - [ ] Implement correctly
  - [ ] Use AutoMapper
  - [ ] Test: Build succeeds

### Commands/Queries - Categories

- [ ] **CreateCategoryCommand.cs**
  - [ ] Verify types (should be string)

- [ ] **CreateCategoryHandler.cs**
  - [ ] Use AutoMapper
  - [ ] Remove manual validation

- [ ] **UpdateCategoryCommand.cs**
  - [ ] Change `Guid Id` → `string Id`

- [ ] **UpdateCategoryHandler.cs**
  - [ ] Fix types
  - [ ] Use AutoMapper

- [ ] **DeleteCategoryCommand.cs**
  - [ ] Change `Guid Id` → `string Id`

- [ ] **DeleteCategoryHandler.cs**
  - [ ] Fix types

- [ ] **GetCategoriesQuery.cs**
  - [ ] Verify implementation

- [ ] **GetCategoriesHandler.cs**
  - [ ] Use AutoMapper
  - [ ] Test: Build succeeds

- [ ] **GetCategoryByIdQuery.cs**
  - [ ] Change `Guid Id` → `string Id`

- [ ] **GetCategoryByIdHandler.cs**
  - [ ] Fix types
  - [ ] Use AutoMapper

### Dashboard

- [ ] **GetDashboardStatsQuery.cs**
  - [ ] Verify structure

- [ ] **GetDashboardStatsHandler.cs**
  - [ ] Implement stats calculation
  - [ ] Add caching
  - [ ] Group by category
  - [ ] Test: Build succeeds

### DTOs

- [ ] **ProductResponseDto.cs**
  - [ ] Change `Guid Id` → `string Id`
  - [ ] Change `Guid CategoryId` → `string CategoryId`
  - [ ] Add `decimal Price` field
  - [ ] Add `string PriceCurrency` field
  - [ ] Add `int StockQuantity` field
  - [ ] Add `bool IsLowStock` field

- [ ] **CategoryResponseDto.cs**
  - [ ] Change `Guid Id` → `string Id`
  - [ ] Verify fields match domain

### AutoMapper

- [ ] **MappingProfile.cs**
  - [ ] Map Product → ProductResponseDto
  - [ ] Map Category → CategoryResponseDto
  - [ ] Handle Price ValueObject
  - [ ] Handle StockQuantity ValueObject
  - [ ] Test: Build succeeds

### Build Verification

- [ ] Run `dotnet build` → **0 errors**
- [ ] Run `dotnet build` → **< 5 warnings**
- [ ] Review warnings → Fix critical ones

---

## 📋 PHASE 2: ADD CONTROLLERS AUTHORIZATION (Target: 2 hours)

### ProductsController

- [ ] Add `[Authorize]` at class level
- [ ] Add `[Authorize(Policy = "Admin")]` to POST
- [ ] Add `[Authorize(Policy = "Admin")]` to PUT
- [ ] Add `[Authorize(Policy = "Admin")]` to DELETE
- [ ] Add `[Authorize(Policy = "User")]` to GET
- [ ] Add `[Authorize(Policy = "Manager")]` to UpdateStock
- [ ] Fix all parameter types (`Guid` → `string`)
- [ ] Add XML documentation comments
- [ ] Test: Build succeeds

### CategoriesController

- [ ] Add `[Authorize]` at class level
- [ ] Add `[Authorize(Policy = "Admin")]` to POST/PUT/DELETE
- [ ] Add `[Authorize(Policy = "User")]` to GET
- [ ] Fix all parameter types
- [ ] Add XML documentation
- [ ] Test: Build succeeds

### DashboardController

- [ ] Add `[Authorize(Policy = "User")]`
- [ ] Verify implementation
- [ ] Add XML documentation
- [ ] Test: Build succeeds

---

## 📋 PHASE 3: CREATE TEST PROJECT (Target: 1 hour)

### Setup

- [ ] Create project:
  ```bash
  dotnet new xunit -n Hypesoft.Tests -o tests/Hypesoft.Tests
  ```

- [ ] Add packages:
  ```bash
  cd tests/Hypesoft.Tests
  dotnet add package FluentAssertions
  dotnet add package Moq
  dotnet add package Microsoft.AspNetCore.Mvc.Testing
  ```

- [ ] Add references:
  ```bash
  dotnet add reference ../../src/Hypesoft.Domain
  dotnet add reference ../../src/Hypesoft.Application
  ```

- [ ] Add to solution:
  ```bash
  cd ../..
  dotnet sln add tests/Hypesoft.Tests/Hypesoft.Tests.csproj
  ```

- [ ] Test: `dotnet build`

---

## 📋 PHASE 4: CREATE DOMAIN TESTS (Target: 3 hours)

### ProductTests.cs

- [ ] Copy from TEST-SUITE-EXAMPLE.md
- [ ] Test: `Create_WithValidData_ShouldCreateProduct`
- [ ] Test: `Create_WithEmptyName_ShouldThrowException`
- [ ] Test: `Create_WithNegativePrice_ShouldThrowException`
- [ ] Test: `Create_WithLowStock_ShouldRaiseLowStockEvent`
- [ ] Test: `UpdateStock_ToLowValue_ShouldRaiseLowStockEvent`
- [ ] Test: `DecreaseStock_BelowZero_ShouldThrowException`
- [ ] Test: `Update_WithValidData_ShouldUpdateProduct`
- [ ] Test: `Deactivate_ShouldSetIsActiveToFalse`
- [ ] Test: `Activate_ShouldSetIsActiveToTrue`
- [ ] Run: `dotnet test` → **9 tests pass**

### PriceValueObjectTests.cs

- [ ] Copy from TEST-SUITE-EXAMPLE.md
- [ ] Test: `Create_WithValidAmount_ShouldCreatePrice`
- [ ] Test: `Create_WithNegativeAmount_ShouldThrowException`
- [ ] Test: `Create_WithCustomCurrency_ShouldUseThatCurrency`
- [ ] Test: `ApplyDiscount_WithValidPercentage_ShouldReducePrice`
- [ ] Test: `ApplyDiscount_WithInvalidPercentage_ShouldThrowException`
- [ ] Test: `Equals_WithSameValues_ShouldReturnTrue`
- [ ] Test: `Equals_WithDifferentValues_ShouldReturnFalse`
- [ ] Run: `dotnet test` → **7 tests pass**

### StockQuantityValueObjectTests.cs

- [ ] Copy from TEST-SUITE-EXAMPLE.md
- [ ] Test: `Create_WithValidQuantity_ShouldCreateStockQuantity`
- [ ] Test: `Create_WithNegativeQuantity_ShouldThrowException`
- [ ] Test: `IsLowStock_WithQuantityAboveThreshold_ShouldReturnFalse`
- [ ] Test: `IsLowStock_WithQuantityBelowThreshold_ShouldReturnTrue`
- [ ] Test: `Increase_WithValidAmount_ShouldIncreaseQuantity`
- [ ] Test: `Decrease_WithValidAmount_ShouldDecreaseQuantity`
- [ ] Test: `Decrease_WithAmountGreaterThanStock_ShouldThrowException`
- [ ] Run: `dotnet test` → **7 tests pass**

### CategoryTests.cs

- [ ] Create test file
- [ ] Test: `Create_WithValidData_ShouldCreateCategory`
- [ ] Test: `Create_WithEmptyName_ShouldThrowException`
- [ ] Test: `Update_WithValidData_ShouldUpdateCategory`
- [ ] Test: `Deactivate_ShouldSetIsActiveToFalse`
- [ ] Test: `Activate_ShouldSetIsActiveToTrue`
- [ ] Run: `dotnet test` → **5 tests pass**

**Total Domain Tests: ~28 tests**

---

## 📋 PHASE 5: CREATE APPLICATION TESTS (Target: 2 hours)

### Validators

- [ ] **CreateProductCommandValidatorTests.cs**
  - [ ] Copy from TEST-SUITE-EXAMPLE.md
  - [ ] Test all validation rules (8 tests)
  - [ ] Run: `dotnet test` → **8 tests pass**

- [ ] **UpdateProductCommandValidatorTests.cs**
  - [ ] Create similar to Create validator tests
  - [ ] Test all validation rules (7 tests)
  - [ ] Run: `dotnet test` → **7 tests pass**

- [ ] **CreateCategoryCommandValidatorTests.cs**
  - [ ] Test validation rules (5 tests)
  - [ ] Run: `dotnet test` → **5 tests pass**

### Handlers (with Mocks)

- [ ] **CreateProductHandlerTests.cs**
  - [ ] Mock IProductRepository
  - [ ] Mock ICategoryRepository
  - [ ] Mock IMapper
  - [ ] Test: Happy path
  - [ ] Test: Category not found
  - [ ] Run: `dotnet test` → **2 tests pass**

**Total Application Tests: ~22 tests**

---

## 📋 PHASE 6: TEST DOCKER INFRASTRUCTURE (Target: 2 hours)

### Start Services

- [ ] Run: `docker-compose up -d`
- [ ] Wait 30 seconds
- [ ] Verify: `docker-compose ps` shows all services healthy

### Verify MongoDB

- [ ] Connect: `docker exec -it hypesoft-mongodb mongosh -u admin -p admin123`
- [ ] Test: `show dbs`
- [ ] Test: `use hypesoft`
- [ ] Exit: `exit`

### Verify Redis

- [ ] Connect: `docker exec -it hypesoft-redis redis-cli`
- [ ] Test: `ping` → PONG
- [ ] Test: `set test "value"` → OK
- [ ] Test: `get test` → "value"
- [ ] Exit: `exit`

### Verify PostgreSQL (Keycloak DB)

- [ ] Check: `docker logs hypesoft-postgres | grep "ready"`
- [ ] Should see: "database system is ready to accept connections"

### Verify Keycloak

- [ ] Open browser: http://localhost:8080
- [ ] Login: admin / admin123
- [ ] Should see: Keycloak Admin Console

---

## 📋 PHASE 7: CONFIGURE KEYCLOAK (Target: 2 hours)

Follow **KEYCLOAK-SETUP.md** step-by-step:

### Realm

- [ ] Create realm "hypesoft"
- [ ] Verify realm is active

### Client

- [ ] Create client "hypesoft-api"
- [ ] Enable client authentication
- [ ] Enable authorization
- [ ] Add redirect URIs
- [ ] Save client

### Roles

- [ ] Create role "admin"
- [ ] Create role "manager"
- [ ] Create role "user"

### Users

- [ ] Create user "admin"
  - [ ] Set password: admin123
  - [ ] Assign role: admin
  - [ ] Email verified: YES

- [ ] Create user "manager"
  - [ ] Set password: manager123
  - [ ] Assign role: manager
  - [ ] Email verified: YES

- [ ] Create user "user"
  - [ ] Set password: user123
  - [ ] Assign role: user
  - [ ] Email verified: YES

### Test Token Retrieval

- [ ] Get admin token:
  ```bash
  curl -X POST "http://localhost:8080/realms/hypesoft/protocol/openid-connect/token" \
    -d "grant_type=password" \
    -d "client_id=hypesoft-api" \
    -d "username=admin" \
    -d "password=admin123"
  ```
- [ ] Verify: Returns `access_token`
- [ ] Decode token at jwt.io
- [ ] Verify: Contains role "admin"

---

## 📋 PHASE 8: RUN API & TEST (Target: 2 hours)

### Run API

- [ ] Build: `dotnet build`
- [ ] Run: `dotnet run --project src/Hypesoft.API`
- [ ] Verify: Listening on http://localhost:5000

### Test Health Check

- [ ] Test: `curl http://localhost:5000/health`
- [ ] Verify: Returns "Healthy"
- [ ] Verify: MongoDB check healthy
- [ ] Verify: Redis check healthy
- [ ] Verify: Keycloak check healthy

### Test Swagger

- [ ] Open: http://localhost:5000/swagger
- [ ] Verify: API documentation loads
- [ ] Click "Authorize"
- [ ] Paste admin token
- [ ] Verify: Lock icon turns closed

### Test Create Category

- [ ] POST /api/categories
- [ ] Body:
  ```json
  {
    "name": "Test Category",
    "description": "Test Description"
  }
  ```
- [ ] Verify: Returns 201 Created
- [ ] Note the category ID

### Test Create Product

- [ ] POST /api/products
- [ ] Body:
  ```json
  {
    "name": "Test Product",
    "description": "Test Description",
    "price": 99.99,
    "categoryId": "{CATEGORY_ID}",
    "stockQuantity": 10
  }
  ```
- [ ] Verify: Returns 201 Created
- [ ] Note the product ID

### Test Get Products

- [ ] GET /api/products
- [ ] Verify: Returns array with created product
- [ ] Verify: Response time < 500ms

### Test Get Product By ID

- [ ] GET /api/products/{id}
- [ ] Verify: Returns product details

### Test Dashboard

- [ ] GET /api/dashboard/stats
- [ ] Verify: Returns statistics
- [ ] Verify: totalProducts = 1
- [ ] Verify: productsByCategory shows test category

### Test Authorization

- [ ] Remove token from Swagger
- [ ] Try GET /api/products
- [ ] Verify: Returns 401 Unauthorized

- [ ] Add user token (not admin)
- [ ] Try POST /api/products
- [ ] Verify: Returns 403 Forbidden

---

## 📋 PHASE 9: RUN ALL TESTS (Target: 30 min)

### Unit Tests

- [ ] Run: `dotnet test`
- [ ] Verify: 50+ tests pass
- [ ] Verify: 0 tests fail
- [ ] Verify: Test time < 30 seconds

### Coverage Report

- [ ] Run: `dotnet test --collect:"XPlat Code Coverage"`
- [ ] Verify: Coverage > 60%
- [ ] Target areas:
  - Domain: > 85%
  - Application: > 60%
  - Overall: > 60%

---

## 📋 PHASE 10: FINAL VERIFICATION (Target: 1 hour)

### Code Quality

- [ ] Run: `dotnet build` → 0 errors
- [ ] Check warnings → < 5 warnings
- [ ] Review TODO comments → None remain
- [ ] Review dead code → None remains

### Documentation

- [ ] README.md is complete ✅
- [ ] All setup instructions work
- [ ] Docker commands documented
- [ ] Keycloak setup documented

### Performance

- [ ] Test query speed: < 500ms
- [ ] Test with cache: Faster on second call
- [ ] Test rate limiting: Blocks after 100 requests

### Security

- [ ] All endpoints require auth
- [ ] Roles are enforced correctly
- [ ] Security headers present
- [ ] CORS works correctly

---

## 🎯 FINAL CHECKLIST - DEFINITION OF DONE

### Must Have (70/100):

- [ ] `dotnet build` succeeds (0 errors)
- [ ] `dotnet test` shows 60%+ coverage
- [ ] `docker-compose up -d` starts all services
- [ ] `curl http://localhost:5000/health` responds "Healthy"
- [ ] Keycloak returns JWT token for test users
- [ ] One complete CRUD flow works with authentication:
  - [ ] Create category (authenticated)
  - [ ] Create product (authenticated)
  - [ ] Get products (authenticated)
  - [ ] Get dashboard (authenticated)
- [ ] Authorization policies enforced (403 for wrong role)
- [ ] README.md has complete setup instructions

### Verification:

Score yourself:
- Architecture: ____/20
- Code Quality: ____/15
- Tests: ____/15
- Performance: ____/10
- Security: ____/10
- Functional: ____/15
- Documentation: ____/10
- Professional: ____/5

**TOTAL: ____/100**

**Target: 70+/100** ✅

---

## 📊 PROGRESS TRACKING

| Phase | Status | Time Spent | Notes |
|-------|--------|------------|-------|
| 1. Fix Application | ⬜ | ___h | |
| 2. Add [Authorize] | ⬜ | ___h | |
| 3. Create Tests Project | ⬜ | ___h | |
| 4. Domain Tests | ⬜ | ___h | |
| 5. Application Tests | ⬜ | ___h | |
| 6. Docker Setup | ⬜ | ___h | |
| 7. Keycloak Config | ⬜ | ___h | |
| 8. Integration Test | ⬜ | ___h | |
| 9. Run All Tests | ⬜ | ___h | |
| 10. Final Verification | ⬜ | ___h | |

**Legend**: ⬜ Not Started | 🟡 In Progress | ✅ Complete

---

**Print this checklist and mark items as you complete them!**

Good luck! 🚀
