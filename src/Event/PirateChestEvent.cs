using System.Collections.Generic;
using Aotenjo;

public class PirateChestEvent : PlayerEvent
{
    public List<PirateChestReward> rewards;

    public PirateChestEvent(List<PirateChestReward> rewards, Player player) : base(player)
    {
        this.rewards = rewards;
    }
}