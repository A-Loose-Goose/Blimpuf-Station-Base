using Robust.Shared.Prototypes;

namespace Content.Server._Blimpuf.RotMultiplier;

[RegisterComponent]
public sealed partial class RotRateMulitplierComponent : Component
{
    [DataField]
    public float RotRateMulitplier = 1f;
}
