using Content.Server._Starlight.Thaven;
using Content.Shared._Starlight.Thaven;
using Robust.Shared.Physics.Events;

namespace Content.Server._Blimpuf.Thaven;

public sealed partial class AddThavenMoodOnCollideSystem : EntitySystem
{
    [Dependency] private ThavenMoodsSystem _thavenMood = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<AddThavenMoodOnCollideComponent, StartCollideEvent>(OnCollide);
    }

    private void OnCollide(EntityUid uid, AddThavenMoodOnCollideComponent comp, ref StartCollideEvent args)
    {
        if (comp.Used)
            return;

        var target = args.OtherEntity;

        if (!TryComp<ThavenMoodsComponent>(target, out var moodBound))
            return;


        comp.Used = true;
        _thavenMood.TryAddRandomMood((target, moodBound), moodBound.Wildcard);
        RemCompDeferred<AddThavenMoodOnCollideComponent>(uid);
    }
}
