using FluentValidation.TestHelper;
using Hypesoft.Application.Categories.Commands.CreateCategory;

namespace Hypesoft.Tests.Application;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new CreateCategoryCommand { Name = "", Description = "Descrição válida" };
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(value => value.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Empty()
    {
        var command = new CreateCategoryCommand { Name = "Eletrônicos", Description = "" };
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(value => value.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new CreateCategoryCommand { Name = "Eletrônicos", Description = "Categoria para produtos eletrônicos" };
        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
