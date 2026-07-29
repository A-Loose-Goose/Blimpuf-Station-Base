using Content.Shared.Body.Components;
using Content.Shared._Blimpuf.Medical.Body.Components;
using Content.Shared.Body.Systems;

namespace Content.Server._Blimpuf.Medical.Body.Systems;

public sealed partial class DipsomaniaSystem : EntitySystem
{
    [Dependency] private SharedBodySystem _body = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<DipsomaniaComponent, ComponentStartup>(OnDipsomaniaStartup);
    }

    private void OnDipsomaniaStartup(Entity<DipsomaniaComponent> ent, ref ComponentStartup args)
    {
        if (!TryComp<BodyComponent>(ent, out var body)
            || !_body.TryGetOrgansWithComponent<StomachComponent>((ent.Owner, body), out var stomachs))
            return;

        EnsureComp<DipsomaniaCarrierComponent>(stomachs[0]);
        RemComp<DipsomaniaComponent>(ent);
    }
}
