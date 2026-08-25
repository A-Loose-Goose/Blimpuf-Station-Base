using Robust.Client.Graphics;

namespace Content.Client._Starlight.Overlay.Overlays;

public sealed class TNebriVisionOverlay : BaseVisionOverlay
{
    public TNebriVisionOverlay(ShaderPrototype shader) : base(shader)
        => ZIndex = (int?)OverlayZIndexes.TNebri;
}
