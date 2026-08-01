using Content.Shared._Blimpuf.Medical.Body.Components;
using Robust.Client.GameObjects;

namespace Content.Client._Blimpuf.Medical.Body;

public sealed partial class DipsomaniaVisualizerSystem : EntitySystem
{
    [Dependency] private SpriteSystem _sprite = default!;

    private static readonly Color DipsomaniaTint = Color.FromHex("#B7C97A");

    public override void Initialize()
    {
        SubscribeLocalEvent<DipsomaniaCarrierComponent, ComponentStartup>(OnCarrierStartup);
    }

    private void OnCarrierStartup(Entity<DipsomaniaCarrierComponent> ent, ref ComponentStartup args)
    {
        if (!TryComp<SpriteComponent>(ent, out var sprite))
            return;

        _sprite.SetColor((ent.Owner, sprite), DipsomaniaTint);
    }
}
