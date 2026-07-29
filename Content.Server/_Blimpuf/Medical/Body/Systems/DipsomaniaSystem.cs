using Content.Shared.Body.Components;
using Content.Shared._Blimpuf.Medical.Body.Components;
using Content.Shared.Body.Systems;
using Content.Server._Starlight.Medical.Body.Systems;

namespace Content.Server._Blimpuf.Medical.Body.Systems;

public sealed partial class DipsomaniaSystem : EntitySystem
{
    private const string DipsomaniaMetabolizerType = "Dipsomania";

    [Dependency] private SharedBodySystem _body = default!;
    [Dependency] private MetabolizerSystem _metabolizer = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<DipsomaniaComponent, ComponentStartup>(OnDipsomaniaStartup);
    }

    private void OnDipsomaniaStartup(Entity<DipsomaniaComponent> ent, ref ComponentStartup args)
    {
        if (!TryComp<BodyComponent>(ent, out var body)
            || !_body.TryGetOrgansWithComponent<StomachComponent>((ent.Owner, body), out var stomachs))
            return;

        var stomach = stomachs[0];
        if (!TryComp<MetabolizerComponent>(stomach, out var metabolizer))
            return;

        _metabolizer.TryAddMetabolizerType((stomach, metabolizer), DipsomaniaMetabolizerType);
        EnsureComp<DipsomaniaCarrierComponent>(stomach);
        RemComp<DipsomaniaComponent>(ent);
    }
}
