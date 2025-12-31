using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DistanceDisplayer;

/// <summary>
///     Distance Displayer
/// </summary>
public class DistanceDisplayerModSystem : ModSystem
{
    public delegate Vec3d? LastCoordinates();

    private const string ConfigFileName = "DdConfig.json";
    private Vec3d? _lastCoordinates;
    private DistanceDisplayerHud? _hud;

    public override bool ShouldLoad(EnumAppSide side)
    {
        return side == EnumAppSide.Client;
    }

    public override void StartClientSide(ICoreClientAPI capi)
    {
        var parsers = capi.ChatCommands.Parsers;
        capi.ChatCommands.Create("dd")
            .WithDescription("Distance Displayer commands")
            .RequiresPlayer()
            .BeginSubCommand("set")
                .WithDescription("Set Distance Displayer location")
                .WithArgs(parsers.WorldPosition("target"))
                .HandleWith(args =>
                {
                    var coords = (Vec3d)args.Parsers[0].GetValue();
                    _lastCoordinates = coords;
                    capi.StoreModConfig(new DdConfig(coords), ConfigFileName);
                    return TextCommandResult.Success("Ok. To clear, use .dd clear");
                })
            .EndSubCommand()
            .BeginSubCommand("clear")
                .WithDescription("Clear Distance Displayer location")
                .HandleWith(_ =>
                {
                    _lastCoordinates = null;
                    capi.StoreModConfig(new DdConfig(null), ConfigFileName);
                    return TextCommandResult.Success("Cleared");
                })
            .EndSubCommand();

        try
        {
            var config = capi.LoadModConfig<DdConfig>(ConfigFileName);
            // config is null if the JSON file DNE, in which case we can keep _lastCoordinates as null
            // But if we do have something to work with then we will preload it in now
            _lastCoordinates = config?.LastCoordinates;
        }
        catch (Exception e)
        {
            Mod.Logger.Error("Could not load DistanceDisplayer configuration.");
            Mod.Logger.Error(e);
        }

        _hud = new DistanceDisplayerHud(capi, () => _lastCoordinates);
        capi.Event.LevelFinalize += () => { _hud.Register(); };
    }
}