using Ambev.DeveloperEvaluation.Application.Common.Mappings;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ambev.DeveloperEvaluation.Unit.Application.Mapping;

internal static class MapperFactory
{
    public static IMapper Create()
    {
        var configuration = new MapperConfiguration(
            cfg => cfg.AddProfile<ApplicationMappingProfile>(),
            NullLoggerFactory.Instance);

        configuration.AssertConfigurationIsValid();
        return configuration.CreateMapper();
    }
}
