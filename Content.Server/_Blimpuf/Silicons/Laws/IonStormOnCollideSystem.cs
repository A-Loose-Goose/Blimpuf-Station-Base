using Content.Server.Silicons.Laws;
using Content.Shared.Silicons.Laws.Components;
using Robust.Shared.Physics.Events;

namespace Content.Server._Blimpuf.Silicons.Laws;

public sealed partial class IonStormOnCollideSystem : EntitySystem
{
    [Dependency] private readonly IonStormSystem _ionStorm = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<IonStormOnCollideComponent, StartCollideEvent>(OnCollide);
    }

    private void OnCollide(EntityUid uid, IonStormOnCollideComponent comp, ref StartCollideEvent args)
    {
        var target = args.OtherEntity;

        if (!TryComp<SiliconLawBoundComponent>(target, out var lawBound))
            return;

        var ionTarget = EnsureComp<IonStormTargetComponent>(target);
        _ionStorm.IonStormTarget((target, lawBound, ionTarget));
    }
}
