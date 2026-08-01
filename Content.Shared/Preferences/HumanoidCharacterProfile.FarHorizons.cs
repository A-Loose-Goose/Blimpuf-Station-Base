using System.IO;
using System.Linq;
using Content.Shared._Starlight.Traits;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Robust.Shared.Configuration;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Shared.Preferences;

public sealed partial class HumanoidCharacterProfile
{
    public const string SpeciesLoadoutDatabaseKey = "__species_loadout"; // Database will store species loadout as this "job"
    private static readonly ProtoId<TraitPrototype>[] ElfSpeciesVariantTraits = ["HighElf", "WoodElf", "DarkElf", "CaveElf"];

    [DataField]
    public RoleLoadout? SpeciesLoadout = null;

    public HumanoidCharacterProfile WithSpeciesLoadout(RoleLoadout? speciesLoadout) =>
    new(this) { SpeciesLoadout = speciesLoadout, };

    public RoleLoadout? GetSpeciesLoadoutOrDefault(ICommonSession? session, IPrototypeManager protoManager)
    {
        var speciesProto = protoManager.Index(Species);
        if (speciesProto.Loadout == null)
        {
            SpeciesLoadout = null;
            return SpeciesLoadout;
        }

        SpeciesLoadout ??= new RoleLoadout(speciesProto.Loadout.Value);
        SpeciesLoadout.Role = speciesProto.Loadout.Value;

        var loadoutProto = protoManager.Index(SpeciesLoadout.Role);
        foreach (var (group, _) in SpeciesLoadout.SelectedLoadouts.ShallowClone())
        {
            if (!loadoutProto.Groups.Contains(group))
                SpeciesLoadout.SelectedLoadouts.Remove(group);
        }

        SpeciesLoadout.SetDefault(this, session, protoManager);
        return SpeciesLoadout;
    }

    private static bool SpeciesLoadoutEquals(RoleLoadout? A, RoleLoadout? B)
    {
        if (A == null != (B == null))
            return false;

        if (A != null && B != null)
        {
            if (A.SelectedLoadouts.Count != B.SelectedLoadouts.Count)
                return false;

            foreach (var (k, v) in A.SelectedLoadouts)
                if (!B.SelectedLoadouts.TryGetValue(k, out var bValue) || !bValue.SequenceEqual(v))
                    return false;
        }

        return true;
    }

    // Blimpuf start
    private void NormalizeSpeciesVariantTraits(IPrototypeManager protoManager)
    {
        if (Species != "Elf" && Species != "NeoElf")
        {
            foreach (var traitId in ElfSpeciesVariantTraits)
            {
                _traitPreferences.Remove(traitId);
            }

            return;
        }

        ProtoId<TraitPrototype>? selectedVariant = null;
        foreach (var traitId in ElfSpeciesVariantTraits)
        {
            if (!protoManager.HasIndex(traitId) || !_traitPreferences.Contains(traitId))
                continue;

            selectedVariant = traitId;
            break;
        }

        if (selectedVariant == null)
        {
            foreach (var traitId in ElfSpeciesVariantTraits)
            {
                if (!protoManager.HasIndex(traitId))
                    continue;

                selectedVariant = traitId;
                break;
            }
        }

        if (selectedVariant == null)
            return;

        foreach (var traitId in ElfSpeciesVariantTraits)
        {
            if (traitId == selectedVariant.Value)
                continue;

            _traitPreferences.Remove(traitId);
        }

        _traitPreferences.Add(selectedVariant.Value);
    }
    // Blimpuf end
}
