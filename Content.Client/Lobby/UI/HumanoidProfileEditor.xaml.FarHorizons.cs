using System.Linq;
using Content.Client.Lobby.UI.Loadouts;
using Content.Shared._Starlight.Traits;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences.Loadouts;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;

namespace Content.Client.Lobby.UI;

public sealed partial class HumanoidProfileEditor
{
    // Blimpuf start - species variant menu
    private static readonly ProtoId<TraitPrototype>[] SpeciesVariantTraits = ["HighElf", "WoodElf", "DarkElf", "CaveElf"];

    private List<SpeciesPrototype> _subspecies = [];
    private List<ProtoId<TraitPrototype>> _speciesVariants = [];
    // Blimpuf end

    private void UpdateSubspecies()
    {
        CSubspecies.Visible = false;
        _subspecies = [];
        SubspeciesButton.Clear();

        var species = _species.Find(x => x.ID == Profile?.Species) ?? _species.First();

        if(species.HasSubspecies == false && species.SubspeciesOf == null)
            return;

        List<SpeciesPrototype> subspecies = [];
        var selected = 0;

        if (species.HasSubspecies)
        {
            List<SpeciesPrototype> allSubspecies = [.. _prototypeManager.EnumeratePrototypes<SpeciesPrototype>().Where(p => p.SubspeciesOf == species.ID)];
            allSubspecies.Sort((a, b) => string.Compare(a.SubspeciesName ?? a.Name, b.SubspeciesName ?? b.Name, StringComparison.OrdinalIgnoreCase));

            subspecies.Add(species);
            subspecies.AddRange(allSubspecies);
        }
        else if (species.SubspeciesOf != null)
        {
            List<SpeciesPrototype> allSubspecies = [.. _prototypeManager.EnumeratePrototypes<SpeciesPrototype>().Where(p => p.SubspeciesOf == species.SubspeciesOf)];
            allSubspecies.Sort((a, b) => string.Compare(a.SubspeciesName ?? a.Name, b.SubspeciesName ?? b.Name, StringComparison.OrdinalIgnoreCase));
            var parent = _prototypeManager.Index(species.SubspeciesOf);

            subspecies.Add(parent);
            subspecies.AddRange(allSubspecies);
            selected = subspecies.IndexOf(species);
        }

        if (subspecies.Count == 0)
            return;

        for (var i = 0; i < subspecies.Count; i++)
        {
            _subspecies.Add(subspecies[i]);

            var name = Loc.GetString(subspecies[i].SubspeciesName == null ? subspecies[i].Name : subspecies[i].SubspeciesName!.Value);
            SubspeciesButton.AddItem(name, i);
        }


        SubspeciesButton.SelectId(selected);
        CSubspecies.Visible = true;
    }

    // Blimpuf start
    private void UpdateSpeciesVariant()
    {
        CSpeciesVariant.Visible = false;
        _speciesVariants = [];
        SpeciesVariantButton.Clear();

        if (Profile == null)
            return;

        _speciesVariants = GetSpeciesVariants(Profile.Species);
        ApplySpeciesVariantSelection(_speciesVariants);

        if (_speciesVariants.Count == 0)
            return;

        var selectedId = _speciesVariants.FindIndex(Profile.TraitPreferences.Contains);
        if (selectedId < 0)
            selectedId = 0;

        for (var i = 0; i < _speciesVariants.Count; i++)
        {
            var variant = _prototypeManager.Index(_speciesVariants[i]);
            SpeciesVariantButton.AddItem(Loc.GetString(variant.Name), i);
        }

        SpeciesVariantButton.SelectId(selectedId);
        CSpeciesVariant.Visible = true;
    }

    private List<ProtoId<TraitPrototype>> GetSpeciesVariants(string speciesId)
    {
        if (speciesId != "Elf" && speciesId != "NeoElf")
            return [];

        return [.. SpeciesVariantTraits.Where(_prototypeManager.HasIndex<TraitPrototype>)];
    }

    private void ApplySpeciesVariantSelection(List<ProtoId<TraitPrototype>> validVariants)
    {
        if (Profile == null)
            return;

        if (validVariants.Count == 0)
        {
            foreach (var traitId in SpeciesVariantTraits.Where(Profile.TraitPreferences.Contains).ToArray())
            {
                Profile = Profile.WithoutTraitPreference(traitId, _prototypeManager);
            }

            return;
        }

        ProtoId<TraitPrototype>? selectedVariant = null;

        foreach (var traitId in validVariants)
        {
            if (!Profile.TraitPreferences.Contains(traitId))
                continue;

            selectedVariant = traitId;
            break;
        }

        selectedVariant ??= validVariants[0];

        foreach (var traitId in SpeciesVariantTraits)
        {
            if (!Profile.TraitPreferences.Contains(traitId) || traitId == selectedVariant.Value)
                continue;

            Profile = Profile.WithoutTraitPreference(traitId, _prototypeManager);
        }

        if (!Profile.TraitPreferences.Contains(selectedVariant.Value))
            Profile = Profile.WithTraitPreference(selectedVariant.Value, _prototypeManager);
    }

    private void SetSpeciesVariant(ProtoId<TraitPrototype> variant)
    {
        if (Profile == null)
            return;

        foreach (var traitId in SpeciesVariantTraits)
        {
            if (Profile.TraitPreferences.Contains(traitId) && traitId != variant)
                Profile = Profile.WithoutTraitPreference(traitId, _prototypeManager);
        }

        Profile = Profile.WithTraitPreference(variant, _prototypeManager);
        UpdateSpeciesVariant();
        Traits.UpdateRequirements(Profile);
        SetDirty();
    }
    // Blimpuf end

    private void UpdateSpeciesLoadout()
    {
        CSpeciesLoadout.Visible = false;
        SpeciesLoadout.OnPressed -= SpeciesLoadoutPressed;

        if (Profile == null ||
            !_prototypeManager.TryIndex(Profile.Species, out var species) ||
            species.Loadout == null ||
            !_prototypeManager.TryIndex(species.Loadout, out var loadoutProto))
            return;


        CSpeciesLoadout.Visible = true;
        SpeciesLoadout.OnPressed += SpeciesLoadoutPressed;
    }

    private void SpeciesLoadoutPressed(BaseButton.ButtonEventArgs args)
    {
         if (Profile == null ||
            !_prototypeManager.TryIndex(Profile.Species, out var species) ||
            species.Loadout == null ||
            !_prototypeManager.TryIndex(species.Loadout, out var loadoutProto))
            return;

        RoleLoadout? loadout = null;

        if (Profile!.SpeciesLoadout == null)
        {
            loadout = Profile.GetSpeciesLoadoutOrDefault(_playerManager.LocalSession, _prototypeManager);
            loadout!.SetDefault(Profile, _playerManager.LocalSession, _prototypeManager);
        } else {
            loadout = Profile.SpeciesLoadout!.Clone();
            loadout!.SetDefault(Profile, _playerManager.LocalSession, _prototypeManager);
        }

        OpenSpeciesLoadout(species, loadout, loadoutProto);
    }

    private void OpenSpeciesLoadout(SpeciesPrototype species, RoleLoadout speciesLoadout, RoleLoadoutPrototype speciesLoadoutProto)
    {
        _loadoutWindow?.Dispose();
        _loadoutWindow = null;
        var collection = IoCManager.Instance;

        if (collection == null || _playerManager.LocalSession == null || Profile == null || species.Loadout == null)
            return;

        var session = _playerManager.LocalSession;

        _loadoutWindow = new LoadoutWindow(Profile, speciesLoadout, speciesLoadoutProto, _playerManager.LocalSession, collection)
        {
            Title = Loc.GetString("loadout-window-title-loadout", ("job", $"{Loc.GetString(species.Name)}")),
        };

        // Refresh the buttons etc.
        _loadoutWindow.RefreshLoadouts(speciesLoadout, session, collection);
        _loadoutWindow.OpenCenteredLeft();

        _loadoutWindow.OnLoadoutPressed += (loadoutGroup, loadoutProto) =>
        {
            speciesLoadout.AddLoadout(loadoutGroup, loadoutProto, _prototypeManager);
            _loadoutWindow.RefreshLoadouts(speciesLoadout, session, collection);
            Profile = Profile?.WithSpeciesLoadout(speciesLoadout);
            ReloadPreview();
        };

        _loadoutWindow.OnLoadoutUnpressed += (loadoutGroup, loadoutProto) =>
        {
            speciesLoadout.RemoveLoadout(loadoutGroup, loadoutProto, _prototypeManager);
            _loadoutWindow.RefreshLoadouts(speciesLoadout, session, collection);
            Profile = Profile?.WithSpeciesLoadout(speciesLoadout);
            ReloadPreview();
        };

        ReloadPreview();

        _loadoutWindow.OnClose += () =>
        {
            JobOverride = null;
            ReloadPreview();
        };

        if (Profile is null)
            return;

        UpdateJobPreferences();
    }
}
