using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace DistanceDisplayer;

/// <summary>
/// HUD for distance displayer. Hooked into the same triggers as the HudElementCoordinates.
/// </summary>
public class DistanceDisplayerHud(ICoreClientAPI capi, DistanceDisplayerModSystem.LastCoordinates coords)
    : HudElement(capi)
{
    private const string Id = "distancedisplayerhud";

    /// <summary>
    /// <b>WORKAROUND</b>
    /// <p>
    /// By using this offset we remove the ability for the coordinates to reposition when the minimap (in top right) moves,
    /// but it appears necessary in order to display the text in this corner of the screen and not on top of the map.
    /// This workaround does work but has non-ideal ordering and does not reposition or react to the minimap presence.
    /// </p>
    /// </summary>
    private const int MinimapYOffset = 265;

    /// <summary>
    /// Main setup to be called on world entry. Hooks into the HudElementCoordinates system to show a box under it.
    /// </summary>
    public void Register()
    {
        var windowBounds = ElementBounds.Fixed(EnumDialogArea.RightTop, 0.0, 0.0, 190.0, 36.0);
        var backgroundBounds = windowBounds.ForkBoundingParent(5.0, 5.0, 5.0, 5.0);
        var distanceBounds = ElementBounds.Percentual(EnumDialogArea.CenterTop, 1, 0.5);
        var targetBounds = ElementBounds.Percentual(EnumDialogArea.CenterBottom, 1, 0.5);
        SingleComposer = capi.Gui
            .CreateCompo(Id,
                ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.RightTop)
                    .WithFixedAlignmentOffset(-GuiStyle.DialogToScreenPadding,
                        GuiStyle.DialogToScreenPadding + MinimapYOffset))
            .AddGameOverlay(backgroundBounds)
            .AddDynamicText("", CairoFont.WhiteSmallishText()
                .WithOrientation(EnumTextOrientation.Center), distanceBounds, "dist")
            .AddDynamicText("", CairoFont.WhiteDetailText()
                .WithOrientation(EnumTextOrientation.Center), targetBounds, "target")
            .Compose();
        if (!capi.World.Config.GetBool("allowCoordinateHud", true))
        {
            // Also respect HudElementCoordinates settings since we tie into those
            var clientMain = capi.World as ClientMain;
            clientMain?.EnqueueMainThreadTask(() =>
            {
                clientMain.UnregisterDialog(this);
                capi.Input.SetHotKeyHandler(Id, null);
                Dispose();
            }, "unregdd");
        }
        else
        {
            capi.Event.RegisterGameTickListener(Periodic, 250);
            // From HudElementCoordinates
            ClientSettings.Inst.AddWatcher<bool>("showCoordinateHud", on =>
            {
                if (on)
                    TryOpen();
                else
                    TryClose();
            });
        }

        TryOpen();
    }

    public override string ToggleKeyCombinationCode => Id;

    public override bool TryOpen()
    {
        return ClientSettings.ShowCoordinateHud && base.TryOpen();
    }

    private void Periodic(float dt)
    {
        var target = coords();
        if (target == null)
        {
            SingleComposer.GetDynamicText("dist").SetNewText("No target.");
            SingleComposer.GetDynamicText("target").SetNewText("Set one with .dd set!");
            return;
        }

        var player = capi.World.Player.Entity.Pos.AsBlockPos.ToVec3d();
        if (player == null)
            return;

        var dist = (int)player.DistanceTo(target);
        SingleComposer.GetDynamicText("dist")
            .SetNewText($"{dist} block{(dist != 1 ? "s" : "")}");

        // Offset to return to local coordinates (matching HudElementCoordinates)
        // We also use flooring instead of rounding since it is more accurate scaling
        var tx = (int)(target.X - capi.World.DefaultSpawnPosition.X);
        var ty = (int)target.Y;
        var tz = (int)(target.Z - capi.World.DefaultSpawnPosition.Z);
        SingleComposer.GetDynamicText("target")
            .SetNewText($"to ({tx}, {ty}, {tz})");
    }
}