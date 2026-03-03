# COMPLETE WORKING EXAMPLE - CreateProduct Flow

This file shows the COMPLETE, CORRECT implementation of CreateProduct as a reference template for fixing other commands/queries.

## ✅ COMMAND

```csharp
// src/Hypesoft.Application/Products/Commands/CreateProduct/CreateProductCommand.cs
using MediatR;
using Hypesoft.Application.Products.DTOs;

namespace Hypesoft.Application.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<ProductResponseDto>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CategoryId { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
}
```

## ✅ VALIDATOR

```csharp
// src/Hypesoft.Application/Products/Commands/CreateProduct/CreateProductCommandValidator.cs
using FluentValidation;

namespace Hypesoft.Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product description is required")
            .MaximumLength(1000).WithMessage("Product description cannot exceed 1000 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required");
    }
}
```

## ✅ HANDLER

```csharp
// src/Hypesoft.Application/Products/Commands/CreateProduct/CreateProductHandler.cs
using AutoMapper;
using MediatR;
using Hypesoft.Application.Products.DTOs;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Commands.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateProductHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<ProductResponseDto> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Validation is handled by FluentValidation pipeline
        // No need for manual validation here

        // Business rule: Category must exist
        var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new ArgumentException($"Category with ID {request.CategoryId} does not exist");

        // Create domain entity using factory method
        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity,
            request.CategoryId,
            request.ImageUrl);

        // Persist
        await _productRepository.AddAsync(product, cancellationToken);

        // Map to DTO and return
        return _mapper.Map<ProductResponseDto>(product);
    }
}
```

## ✅ DTO

```csharp
// src/Hypesoft.Application/Products/DTOs/ProductResponseDto.cs
namespace Hypesoft.Application.Products.DTOs;

public class ProductResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PriceCurrency { get; set; } = "BRL";
    public string CategoryId { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public bool IsLowStock { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

## ✅ AUTOMAPPER PROFILE

```csharp
// src/Hypesoft.Application/Common/Mappings/MappingProfile.cs
using AutoMapper;
using Hypesoft.Application.Products.DTOs;
using Hypesoft.Domain.Entities;

namespace Hypesoft.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(dest => dest.PriceCurrency, opt => opt.MapFrom(src => src.Price.Currency))
            .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity.Value))
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.StockQuantity.IsLowStock()));
    }
}
```

## ✅ CONTROLLER

```csharp
// src/Hypesoft.API/Controllers/ProductsController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hypesoft.Application.Products.Commands.CreateProduct;

namespace Hypesoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="command">Product details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created product</returns>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById(
        string id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
```

## ✅ VALIDATION BEHAVIOR (for Program.cs)

```csharp
// src/Hypesoft.API/Behaviors/ValidationBehavior.cs
using FluentValidation;
using MediatR;

namespace Hypesoft.API.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}
```

## ✅ PROGRAM.CS REGISTRATION

```csharp
// src/Hypesoft.API/Program.cs (relevant sections)

// Add after MediatR registration:
builder.Services.AddValidatorsFromAssembly(
    typeof(Hypesoft.Application.Products.Commands.CreateProduct.CreateProductCommand).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddAutoMapper(
    typeof(Hypesoft.Application.Products.Commands.CreateProduct.CreateProductCommand).Assembly);
```

## ✅ TEST

```csharp
// tests/Hypesoft.Tests/Application/CreateProductCommandValidatorTests.cs
using FluentAssertions;
using FluentValidation.TestHelper;
using Hypesoft.Application.Products.Commands.CreateProduct;
using Xunit;

namespace Hypesoft.Tests.Application;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new CreateProductCommand { Name = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Price_Is_Zero()
    {
        var command = new CreateProductCommand { Price = 0 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            CategoryId = "507f1f77bcf86cd799439011",
            StockQuantity = 10
        };

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
```

---

## 🎯 PATTERN TO FOLLOW

Use this exact pattern for ALL other commands/queries:

### For Commands (Create/Update/Delete):
1. Command class with `string` IDs
2. Validator class with FluentValidation rules
3. Handler with:
   - Constructor injection (repos + mapper)
   - NO manual validation
   - Domain entity methods
   - AutoMapper to DTO

### For Queries:
1. Query class with filters/pagination
2. Handler with:
   - Caching check first
   - AsNoTracking queries
   - AutoMapper to DTO
   - Cache result

### For Controllers:
1. `[Authorize]` on class level
2. Specific `[Authorize(Policy = "...")]` on methods
3. XML documentation
4. Proper HTTP status codes
5. `string` IDs (not Guid)

---

## 🔄 MIGRATION CHECKLIST

For each existing command/query:

- [ ] Change all `Guid` → `string`
- [ ] Remove manual validation from handler
- [ ] Create FluentValidation validator
- [ ] Update DTO to match Domain (use Price.Amount, StockQuantity.Value)
- [ ] Verify AutoMapper mapping exists
- [ ] Add `[Authorize]` to controller method
- [ ] Create validator test
- [ ] Test with Swagger

---

**Use this file as your REFERENCE for fixing all other Application layer files!**
