using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server._Blimpuf.Smile;

    [RegisterComponent]
    public sealed partial class ReagentSuckerComponent : Component
    {
        [DataField] public string SolutionName = "tank";
        [DataField] public float Radius = 1.0f;
        [DataField] public float SprayDistance = 3.5f;
        [DataField("spraySound")] public SoundSpecifier SpraySound = new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/water_spray.ogg");
        [DataField] public float VaporSpread = 90f;
        [DataField] public FixedPoint2 TransferAmount = 10;
        [DataField] public int VaporAmount = 1;
        [DataField] public float SprayVelocity = 3.5f;
        [DataField] public float PushbackAmount = 5f;

        [DataField] public EntProtoId SuckUpLiquid = "ActionLiquidSuck";
        [DataField] public EntProtoId EmptyLiquid = "ActionLiquidEmpty";
        [DataField] public EntProtoId SprayLiquid = "ActionLiquidSpray";

        [DataField] public EntityUid? SuckUpLiquidEntity;
        [DataField] public EntityUid? EmptyLiquidEntity;
        [DataField] public EntityUid? SprayLiquidEntity;
    }

