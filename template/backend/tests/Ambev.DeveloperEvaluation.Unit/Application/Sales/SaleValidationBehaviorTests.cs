using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Common.Validation;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class SaleValidationBehaviorTests
{
    [Fact(DisplayName = "Validation behavior should stop invalid sale commands")]
    public async Task Given_InvalidSaleCommand_When_BehaviorRuns_Then_ShouldThrowValidationException()
    {
        var command = CreateValidCommand();
        command.Items[0].Quantity = 21;
        var behavior = new ValidationBehavior<CreateSaleCommand, CreateSaleResult>(
            [new CreateSaleCommandValidator()]);
        var nextWasCalled = false;

        var act = async () => await behavior.Handle(command, () =>
        {
            nextWasCalled = true;
            return Task.FromResult(new CreateSaleResult());
        }, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
        nextWasCalled.Should().BeFalse();
    }

    [Fact(DisplayName = "Validation behavior should continue valid sale commands")]
    public async Task Given_ValidSaleCommand_When_BehaviorRuns_Then_ShouldCallNext()
    {
        var command = CreateValidCommand();
        var expected = new CreateSaleResult { Id = Guid.NewGuid() };
        var behavior = new ValidationBehavior<CreateSaleCommand, CreateSaleResult>(
            [new CreateSaleCommandValidator()]);

        var result = await behavior.Handle(command, () => Task.FromResult(expected), CancellationToken.None);

        result.Should().Be(expected);
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
