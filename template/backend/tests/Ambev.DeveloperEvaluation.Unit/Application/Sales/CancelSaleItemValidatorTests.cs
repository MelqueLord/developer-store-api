using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelSaleItemValidatorTests
{
    private readonly CancelSaleItemValidator _validator;

    public CancelSaleItemValidatorTests()
    {
        _validator = new CancelSaleItemValidator();
    }

    [Fact(DisplayName = "Valid cancel sale item command should pass validation")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        var command = new CancelSaleItemCommand(Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Cancel sale item command without IDs should fail validation")]
    public void Given_CommandWithoutIds_When_Validated_Then_ShouldHaveErrors()
    {
        var command = new CancelSaleItemCommand(Guid.Empty, Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.SaleId);
        result.ShouldHaveValidationErrorFor(item => item.ItemId);
    }
}
