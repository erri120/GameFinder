using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture.Kernel;
using TestUtils;

namespace GameFinder.StoreHandlers.EGS.Tests.AutoData;

public class FileSystemBuilder : ISpecimenBuilder
{
    private static readonly MockFileSystem FileSystem = new();

    public object Create(object request, ISpecimenContext context)
    {
        var type = request.ExtractType();
        if (type != typeof(IFileSystem) && type != typeof(MockFileSystem))
            return new NoSpecimen();
        return FileSystem;
    }
}
