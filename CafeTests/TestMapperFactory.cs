using AutoMapper;
using CafeApi.Profiles;
using Microsoft.Extensions.Logging.Abstractions;

namespace CafeTests;

public static class TestMapperFactory
{
    public static IMapper Create()
    {
        var expression = new MapperConfigurationExpression();
        expression.AddProfile<MappingProfile>();

        var config = new MapperConfiguration(expression, NullLoggerFactory.Instance);

        return config.CreateMapper();
    }
}