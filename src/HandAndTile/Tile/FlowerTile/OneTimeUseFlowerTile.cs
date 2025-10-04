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
        player.PreRoundEndEvent += ResetStatus;
    }

    public override void UnsubscribeFromPlayer(Player player)
    {
        base.UnsubscribeFromPlayer(player);
        player.PreRoundEndEvent -= ResetStatus;
    }

    private void ResetStatus(PlayerEvent playerEvent)
    {
        used = false;
    }
}