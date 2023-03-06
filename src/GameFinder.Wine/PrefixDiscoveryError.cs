using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Vogen;

namespace GameFinder.Wine;

/// <summary>
/// Represents an error encountered during prefix discovery.
/// </summary>
[ValueObject<string>]
[PublicAPI]
[SuppressMessage("", "MA0097", Justification = "Affected code is auto generated.")]
[SuppressMessage("", "AddValidationMethod", Justification = "Validation method is not required.")]
public partial class PrefixDiscoveryError { }
