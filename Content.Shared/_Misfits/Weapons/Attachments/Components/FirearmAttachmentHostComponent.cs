using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared._Misfits.Weapons.Attachments.Components;

/// <summary>
/// Marks a firearm as accepting modular attachments.
/// The muzzle slot accepts either a suppressor or a bayonet, making the two mutually exclusive.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedFirearmAttachmentSystem))]
public sealed partial class FirearmAttachmentHostComponent : Component
{
    public const string DefaultMuzzleSlot = "weapon_muzzle";
    public const string DefaultOpticSlot = "weapon_optic";

    [DataField, AutoNetworkedField]
    public string MuzzleSlotId = DefaultMuzzleSlot;

    /// <summary>
    /// Defined here instead of in ItemSlots YAML so concrete guns cannot accidentally replace it
    /// when they define their own magazine and chamber slots.
    /// </summary>
    [DataField]
    public ItemSlot MuzzleSlot = new();

    /// <summary>
    /// Optics are opt-in per firearm so scopes cannot be fitted to every rifle automatically.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool EnableOpticSlot;

    [DataField, AutoNetworkedField]
    public string OpticSlotId = DefaultOpticSlot;

    [DataField]
    public ItemSlot OpticSlot = new();

    /// <summary>
    /// Rifle-owned proxy action used to activate a contained scope. Actions cannot be
    /// granted through a different entity's action container.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? OpticToggleActionEntity;

    [DataField, AutoNetworkedField]
    public EntProtoId OpticToggleAction = "N14ActionToggleScope";

    [DataField, AutoNetworkedField]
    public EntityUid? OpticCycleActionEntity;

    [DataField, AutoNetworkedField]
    public EntProtoId OpticCycleAction = "N14ActionCycleZoomLevel";

    /// <summary>
    /// Selects the reusable attachment sprite that matches this firearm's icon orientation.
    /// </summary>
    [DataField, AutoNetworkedField]
    public FirearmAttachmentVisualOrientation VisualOrientation = FirearmAttachmentVisualOrientation.Diagonal;

    /// <summary>
    /// Offset from the center of the firearm sprite to its muzzle attachment origin, in tiles.
    /// Attachment layers may extend beyond the firearm's original 32x32 canvas.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Vector2 MuzzleVisualOffset = Vector2.Zero;
}

public enum FirearmAttachmentVisualOrientation : byte
{
    Horizontal,
    Diagonal,
}
