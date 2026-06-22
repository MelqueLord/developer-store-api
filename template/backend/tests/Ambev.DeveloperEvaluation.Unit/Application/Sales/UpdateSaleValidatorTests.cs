using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleValidatorTests
{
    private readonly UpdateSaleCommandValidator _validator;

    public UpdateSaleValidatorTests()
    {
        _validator = new UpdateSaleCommandValidator();
    }

    [Fact(DisplayName = "Valid update sale command should pass validation")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Update sale command without ID should fail validation")]
    public void Given_CommandWithoutId_When_Validated_Then_ShouldHaveError()
    {
        var command = CreateValidCommand();
        command.Id = Guid.Empty;

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(sale => sale.Id);
    }

    [Fact(DisplayName = "Update sale command without items should fail validation")]
    public void Given_CommandWithoutItems_When_Validated_Then_ShouldHaveError()
    {
        var command = CreateValidCommand();
        command.Items = [];

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(sale => sale.Items);
    }

    [Fact(DisplayName = "Update sale command with item above 20 units should fail validation")]
    public void Given_CommandWithItemAboveTwentyUnits_When_Validated_Then_ShouldHaveError()
    {
        var command = CreateValidCommand();
        command.Items[0].Quantity = 21;

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Items[0].Quantity");
    }

    private static UpdateSaleCommand CreateValidCommand()
    {
        return new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Test customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Test branch",
            Items =
            [
                new UpdateSaleItemCommand
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
