using System;
using Aotenjo;
using UnityEngine;

[Serializable]
public class OneTimeUseFlowerTile : FlowerTile
{
    [SerializeField] protected bool used;

    protected OneTimeUseFlowerTile(Category category, int order) : base(category, order)
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        base.SubscribeToPlayerEvents(player);
        EventBus.Subscribe<PlayerRoundEvent.End.Pre>(PreRoundEnd);
    }

    public override void UnsubscribeFromPlayer(Player player)
    {
        base.UnsubscribeFromPlayer(player);
        EventBus.Subscribe<PlayerRoundEvent.End.Pre>(PreRoundEnd);
    }

    private void PreRoundEnd(PlayerEvent playerEvent)
    {
        used = false;
    }
}