using Content.Shared.Clothing;
using Content.Shared._Starlight.VentCrawl.Components;
using Content.Shared._Starlight.VentCrawl;

namespace Content.Server._Starlight.VentCrawl;

public sealed class VentCrawlClothingSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VentCrawlClothingComponent, ClothingGotEquippedEvent>(OnClothingEquip);
        SubscribeLocalEvent<VentCrawlClothingComponent, ClothingGotUnequippedEvent>(OnClothingUnequip);
    }

    private void OnClothingEquip(Entity<VentCrawlClothingComponent> ent, ref ClothingGotEquippedEvent args)
        => AddComp<VentCrawlerComponent>(args.Wearer);

    private void OnClothingUnequip(Entity<VentCrawlClothingComponent> ent, ref ClothingGotUnequippedEvent args)
        => RemComp<VentCrawlerComponent>(args.Wearer);
}
