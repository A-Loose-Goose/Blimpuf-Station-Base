using Content.Shared._Blimpuf.Geras;
using Robust.Client.GameObjects;

namespace Content.Client._Blimpuf.Geras;
public sealed class ClientGerasSystem : EntitySystem
{
    [Dependency] private readonly SpriteSystem _sprite = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<GerasColorComponent, AfterAutoHandleStateEvent>(OnUpdateColor);
    }
    
    private void OnUpdateColor(EntityUid uid, GerasColorComponent comp, ref AfterAutoHandleStateEvent args)
    {
        if (TryComp<SpriteComponent>(uid, out var sprite))
        {
            _sprite.LayerSetColor(uid, 0, comp.Color);
        }
    }
    
}