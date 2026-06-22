using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users;

public static class UserMappingExtensions
{
    public static CreateUserCommand ToCommand(this CreateUserRequest request)
    {
        return new CreateUserCommand
        {
            Username = request.Username,
            Password = request.Password,
            Phone = request.Phone,
            Email = request.Email,
            Status = request.Status,
            Role = request.Role
        };
    }

    public static GetUserCommand ToGetUserCommand(this Guid id)
    {
        return new GetUserCommand(id);
    }

    public static DeleteUserCommand ToDeleteUserCommand(this Guid id)
    {
        return new DeleteUserCommand(id);
    }

    public static CreateUserResponse ToResponse(this CreateUserResult result)
    {
        return new CreateUserResponse
        {
            Id = result.Id
        };
    }

    public static GetUserResponse ToResponse(this GetUserResult result)
    {
        return new GetUserResponse
        {
            Id = result.Id,
            Name = result.Name,
            Email = result.Email,
            Phone = result.Phone,
            Role = result.Role,
            Status = result.Status
        };
    }
}
