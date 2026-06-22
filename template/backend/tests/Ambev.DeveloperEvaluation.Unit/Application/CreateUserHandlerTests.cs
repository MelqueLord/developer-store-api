using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="CreateUserHandler"/> class.
/// </summary>
public class CreateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _handler = new CreateUserHandler(_userRepository, _passwordHasher);
    }

    [Fact(DisplayName = "Given valid user data When creating user Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        var command = CreateUserHandlerTestData.GenerateValidCommand();
        var createdUserId = Guid.NewGuid();

        _userRepository.CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var user = callInfo.Arg<User>();
                user.Id = createdUserId;
                return user;
            });
        _passwordHasher.HashPassword(Arg.Any<string>()).Returns("hashedPassword");

        var createUserResult = await _handler.Handle(command, CancellationToken.None);

        createUserResult.Should().NotBeNull();
        createUserResult.Id.Should().Be(createdUserId);
        await _userRepository.Received(1).CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid user data When creating user Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        var command = new CreateUserCommand();

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given user creation request When handling Then password is hashed")]
    public async Task Handle_ValidRequest_HashesPassword()
    {
        var command = CreateUserHandlerTestData.GenerateValidCommand();
        var originalPassword = command.Password;
        const string hashedPassword = "h@shedPassw0rd";

        _userRepository.CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());
        _passwordHasher.HashPassword(originalPassword).Returns(hashedPassword);

        await _handler.Handle(command, CancellationToken.None);

        _passwordHasher.Received(1).HashPassword(originalPassword);
        await _userRepository.Received(1).CreateAsync(
            Arg.Is<User>(u => u.Password == hashedPassword),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given valid command When handling Then creates user entity from command")]
    public async Task Handle_ValidRequest_MapsCommandToUser()
    {
        var command = CreateUserHandlerTestData.GenerateValidCommand();

        _userRepository.CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());
        _passwordHasher.HashPassword(Arg.Any<string>()).Returns("hashedPassword");

        await _handler.Handle(command, CancellationToken.None);

        await _userRepository.Received(1).CreateAsync(
            Arg.Is<User>(user =>
                user.Username == command.Username &&
                user.Email == command.Email &&
                user.Phone == command.Phone &&
                user.Status == command.Status &&
                user.Role == command.Role),
            Arg.Any<CancellationToken>());
    }
}
