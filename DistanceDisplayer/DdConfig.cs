using Vintagestory.API.MathTools;

namespace DistanceDisplayer;

/// <summary>
/// Serialisable config.
/// </summary>
/// <param name="LastCoordinates">last known target set by dd</param>
public record DdConfig(Vec3d? LastCoordinates);