#pragma warning disable CS1591
namespace Brewery.Sdk.DevKitPro;

/// <summary>
/// Optimization level for a source/binary build.
/// </summary>
public enum GCCOptimizationLevel
{
    O0,
    O1,
    O2,
    O3,
    Os,
    Ofast,
    Og,
    Oz
}