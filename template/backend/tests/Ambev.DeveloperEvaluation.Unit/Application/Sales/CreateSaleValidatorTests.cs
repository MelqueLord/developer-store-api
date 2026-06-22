using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleValidatorTests
{
    private readonly CreateSaleCommandValidator _validator;

    public CreateSaleValidatorTests()
    {
        _validator = new CreateSaleCommandValidator();
    }

    [Fact(DisplayName = "Valid create sale command should pass validation")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Create sale command without items should fail validation")]
    public void Given_CommandWithoutItems_When_Validated_Then_ShouldHaveError()
    {
        var command = CreateValidCommand();
        command.Items = [];

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(sale => sale.Items);
    }

    [Fact(DisplayName = "Create sale command with item above 20 units should fail validation")]
    public void Given_CommandWithItemAboveTwentyUnits_When_Validated_Then_ShouldHaveError()
    {
        var command = CreateValidCommand();
        command.Items[0].Quantity = 21;

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Items[0].Quantity");
    }

    [Fact(DisplayName = "Create sale command without customer should fail validation")]
    public void Given_CommandWithoutCustomer_When_Validated_Then_ShouldHaveError()
    {
        var command = CreateValidCommand();
        command.CustomerId = Guid.Empty;
        command.CustomerName = string.Empty;

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(sale => sale.CustomerId);
        result.ShouldHaveValidationErrorFor(sale => sale.CustomerName);
    }

    private static CreateSaleCommand CreateValidCommand()
    {
        return new CreateSaleCommand
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Test customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Test branch",
            Items =
            [
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test product",
                    Quantity = 4,
                    UnitPrice = 10m
                }
            ]
        };
    }
}
