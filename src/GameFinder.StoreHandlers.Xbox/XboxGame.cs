using GameFinder.Common;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Xbox;

/// <summary>
/// Represents a game installed with Xbox Game Pass.
/// </summary>
/// <param name="Id"></param>
/// <param name="DisplayName"></param>
/// <param name="Path"></param>
[PublicAPI]
public record XboxGame(XboxGameId Id, string DisplayName, AbsolutePath Path) : IGame;
