using System;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace GameFinder.Tests
{
    public abstract class AStoreHandlerTest<TStoreHandler, TGame>
        where TStoreHandler : AStoreHandler<TGame>, new()
        where TGame : AStoreGame
    {
        [Fact]
        protected void TestStoreHandler()
        {
            var storeHandler = DoSetup();
            ChecksBeforeFindingGames(storeHandler);
            var res = storeHandler.FindAllGames();
            Assert.True(res);
            ChecksAfterFindingGames(storeHandler);
            DoCleanup();
        }

        protected virtual TStoreHandler DoSetup()
        {
            return new TStoreHandler();
        }
        
        protected virtual void ChecksBeforeFindingGames(TStoreHandler storeHandler) { }

        protected virtual void ChecksAfterFindingGames(TStoreHandler storeHandler)
        {
            Assert.NotEmpty(storeHandler.Games);
        }
        
        protected virtual void DoCleanup() { }

        protected ILogger Logger;
        
        protected AStoreHandlerTest(ITestOutputHelper output)
        {
            Logger = new TestLogger(output);
        }
    }

    public class TestLogger : ILogger
    {
        private readonly ITestOutputHelper _testOutputHelper;
        
        public TestLogger(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var dateTime = DateTime.UtcNow;
            _testOutputHelper.WriteLine("{0:s}|{1}|{2}", dateTime, logLevel, formatter(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}
