using Content.Client._Starlight.Overlay.Overlays;
using Content.Shared._Starlight.Overlay.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.Overlay.Systems;

public sealed partial class TNebriVisionSystem : EntitySystem
{
    [Dependency] private IPlayerManager _player = default!;
    [Dependency] private IOverlayManager _overlayMan = default!;
    [Dependency] private IPrototypeManager _prototypeManager = default!;
    private TNebriVisionOverlay _overlay = default!;
    private const string TNebriShaderPrototype = "TNebriShader";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TNebriVisionComponent, ComponentInit>(OnVisionInit);
        SubscribeLocalEvent<TNebriVisionComponent, ComponentShutdown>(OnVisionShutdown);

        SubscribeLocalEvent<TNebriVisionComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<TNebriVisionComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);

        _overlay = new(_prototypeManager.Index<ShaderPrototype>(TNebriShaderPrototype));
    }

    private void OnPlayerAttached(Entity<TNebriVisionComponent> ent, ref LocalPlayerAttachedEvent args)
        => AttemptAddVision(ent.Owner);

    private void OnPlayerDetached(Entity<TNebriVisionComponent> ent, ref LocalPlayerDetachedEvent args)
        => AttemptRemoveVision(ent.Owner, true);

    private void OnVisionInit(Entity<TNebriVisionComponent> ent, ref ComponentInit args)
        => AttemptAddVision(ent.Owner);

    private void OnVisionShutdown(Entity<TNebriVisionComponent> ent, ref ComponentShutdown args)
        => AttemptRemoveVision(ent.Owner);

    private void AttemptAddVision(EntityUid uid)
    {
        //ENSURE this is the local player
        if (_player.LocalSession?.AttachedEntity != uid) return;

        //only add if its active
        if (!TryComp<TNebriVisionComponent>(uid, out var tNebriVision) || !tNebriVision.Active) return;

        _overlayMan.AddOverlay(_overlay);
    }

    /// <summary>
    /// Attempt to remove the overlay from the local player.
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="force">Use if you need to forcefully remove the overlay no matter what. Only should be used with events that ONLY the local player can fire, like attach/detach</param>
    private void AttemptRemoveVision(EntityUid uid, bool force = false)
    {
        //ENSURE this is the local player
        if (_player.LocalSession?.AttachedEntity != uid && !force) return;

        _overlayMan.RemoveOverlay(_overlay);
    }
}
