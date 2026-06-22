using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class SaleIdValidatorTests
{
    [Fact(DisplayName = "Valid get sale command should pass validation")]
    public void Given_ValidGetSaleCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        var validator = new GetSaleValidator();
        var command = new GetSaleCommand(Guid.NewGuid());

        var result = validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Get sale command without ID should fail validation")]
    public void Given_GetSaleCommandWithoutId_When_Validated_Then_ShouldHaveError()
    {
        var validator = new GetSaleValidator();
        var command = new GetSaleCommand(Guid.Empty);

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(sale => sale.Id);
    }

    [Fact(DisplayName = "Valid cancel sale command should pass validation")]
    public void Given_ValidCancelSaleCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        var validator = new CancelSaleValidator();
        var command = new CancelSaleCommand(Guid.NewGuid());

        var result = validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Cancel sale command without ID should fail validation")]
    public void Given_CancelSaleCommandWithoutId_When_Validated_Then_ShouldHaveError()
    {
        var validator = new CancelSaleValidator();
        var command = new CancelSaleCommand(Guid.Empty);

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(sale => sale.Id);
    }
}
