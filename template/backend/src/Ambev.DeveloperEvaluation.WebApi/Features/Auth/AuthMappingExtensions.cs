using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth;

public static class AuthMappingExtensions
{
    public static AuthenticateUserCommand ToCommand(this AuthenticateUserRequest request)
    {
        return new AuthenticateUserCommand
        {
            Email = request.Email,
            Password = request.Password
        };
    }

    public static AuthenticateUserResponse ToResponse(this AuthenticateUserResult result)
    {
        return new AuthenticateUserResponse
        {
            Token = result.Token,
            Email = result.Email,
            Name = result.Name,
            Role = result.Role
        };
    }
}
