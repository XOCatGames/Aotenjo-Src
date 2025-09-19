using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class PatternStarterPlayerEffect : StarterBoostEffect
{
    public PatternStarterPlayerEffect() : base("starter_pattern")
    {
    }

    public override void Boost(Player player)
    {
        List<Yaku> yakus = player.deck.GetAvailableYakus().Where(y => y.rarity == Rarity.RARE).ToList();
        LotteryPool<Yaku> toDraw = new LotteryPool<Yaku>();
        toDraw.AddRange(yakus);
        List<Yaku> res = toDraw.DrawRange(player.GenerateRandomInt, 3, false);
        MessageManager.Instance.OnSoundEvent("book");
        foreach (var yaku in res)
        {
            player.GetSkillSet().AddLevel(yaku.yakuTypeID, 2);
        }
    }

    public class Lite : StarterBoostEffect
    {
        public Lite() : base("starter_pattern_lite")
        {
        }

        public override void Boost(Player player)
        {
            List<Yaku> yakus = player.deck.GetAvailableYakus().Where(y => y.rarity == Rarity.COMMON).ToList();
            MessageManager.Instance.OnSoundEvent("book");
            foreach (var yaku in yakus)
            {
                player.GetSkillSet().AddLevel(yaku.yakuTypeID, 2);
            }
        }
    }
}