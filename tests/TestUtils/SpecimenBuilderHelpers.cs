using System.Reflection;
using AutoFixture.Kernel;

namespace TestUtils;

public static class SpecimenBuilderHelpers
{
    public static Type? ExtractType(this object request)
    {
        return request switch
        {
            ParameterInfo parameterInfo => parameterInfo.ParameterType,
            SeededRequest { Request: Type type } => type,
            Type t => t,
            _ => null,
        };
    }
}
