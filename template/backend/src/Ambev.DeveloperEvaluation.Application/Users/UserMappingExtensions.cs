using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Users;

public static class UserMappingExtensions
{
    public static User ToEntity(this CreateUserCommand command)
    {
        return new User
        {
            Username = command.Username,
            Password = command.Password,
            Phone = command.Phone,
            Email = command.Email,
            Status = command.Status,
            Role = command.Role
        };
    }

    public static CreateUserResult ToCreateUserResult(this User user)
    {
        return new CreateUserResult
        {
            Id = user.Id
        };
    }

    public static GetUserResult ToGetUserResult(this User user)
    {
        return new GetUserResult
        {
            Id = user.Id,
            Name = user.Username,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            Status = user.Status
        };
    }
}
