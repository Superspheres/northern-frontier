using Content.Shared._Misfits.Weapons.Attachments;
using Content.Shared._Misfits.Weapons.Attachments.Components;
using Content.Shared.Containers.ItemSlots;
using Robust.Client.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client._Misfits.Weapons.Attachments;

/// <summary>
/// Draws an installed muzzle attachment as an offset layer on its firearm.
/// </summary>
public sealed class FirearmAttachmentVisualizerSystem : EntitySystem
{
    private static readonly ResPath AttachmentRsi = new("/Textures/_Misfits/Objects/Weapons/Guns/Attachments/firearm_attachments.rsi");

    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    private readonly HashSet<EntityUid> _pendingRefresh = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FirearmAttachmentHostComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<FirearmAttachmentHostComponent, FirearmAttachmentVisualsChangedEvent>(OnVisualsChanged);
    }

    private void OnStartup(Entity<FirearmAttachmentHostComponent> ent, ref ComponentStartup args)
    {
        _pendingRefresh.Add(ent);
    }

    private void OnVisualsChanged(
        Entity<FirearmAttachmentHostComponent> ent,
        ref FirearmAttachmentVisualsChangedEvent args)
    {
        _pendingRefresh.Add(ent);
    }

    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);

        foreach (var uid in _pendingRefresh)
            Refresh(uid);

        _pendingRefresh.Clear();
    }

    private void Refresh(EntityUid uid)
    {
        if (!TryComp<FirearmAttachmentHostComponent>(uid, out var host) ||
            !TryComp<SpriteComponent>(uid, out var sprite))
        {
            return;
        }

        var layer = sprite.LayerMapReserveBlank(FirearmAttachmentVisualLayers.Muzzle);
        _sprite.LayerSetVisible((uid, sprite), layer, false);

        if (!_itemSlots.TryGetSlot(uid, host.MuzzleSlotId, out var slot) ||
            slot.Item is not { } attachment)
        {
            return;
        }

        string? attachmentName = null;
        if (HasComp<SuppressorAttachmentComponent>(attachment))
            attachmentName = "suppressor";
        else if (HasComp<BayonetAttachmentComponent>(attachment))
            attachmentName = "bayonet";

        if (attachmentName == null)
            return;

        var orientation = host.VisualOrientation == FirearmAttachmentVisualOrientation.Horizontal
            ? "horizontal"
            : "diagonal";
        var state = $"{attachmentName}-{orientation}";

        _sprite.LayerSetSprite((uid, sprite), layer, new SpriteSpecifier.Rsi(AttachmentRsi, state));
        _sprite.LayerSetOffset((uid, sprite), layer, host.MuzzleVisualOffset);
        _sprite.LayerSetVisible((uid, sprite), layer, true);
    }
}

public enum FirearmAttachmentVisualLayers : byte
{
    Muzzle,
}
