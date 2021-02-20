# Contributing

## Requirements

- [.NET SDK](https://dotnet.microsoft.com/download)
- IDE: [Visual Studio](https://visualstudio.microsoft.com/) or [Rider](https://www.jetbrains.com/rider/)

## Running Unit Tests

Unit Tests for each Store Handler derive of [`AStoreHandlerTest.cs`](GameFinder.Tests/AStoreHandlerTest.cs) which has the actual test function. Derivatives of `AStoreHandlerTest` can override the Setup and Cleanup function as well as add custom checks before or after finding the games.

The setup functions are intended for use in a CI environment where the `CI` variable is set to `true`. During development I don't recommend setting this variable because some setup functions will modify your registry which you might not want.

I recommend only running the Unit Tests for the specific Store Handler you are currently working on so you don't have to deal with the other setup functions as well.
