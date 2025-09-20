using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class KnowledgelessBoss : Boss
{
    private List<Tuple<YakuType, int>> robbedYakus = new List<Tuple<YakuType, int>>();

    public KnowledgelessBoss() : base("Knowledgeless")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        robbedYakus = new List<Tuple<YakuType, int>>();
        player.PostSettlePermutationEvent += RobYakus;
        EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.PostSettlePermutationEvent -= RobYakus;
        EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
    }

    private void RobYakus(PlayerPermutationEvent eventData)
    {
        Player player = eventData.player;
        Permutation perm = eventData.permutation;
        foreach (var yaku in perm.GetYakus(player)
                     .Where(a => YakuTester.InfoMap[a].rarity > Rarity.COMMON && player.GetSkillSet().GetLevel(a) > 0))
        {
            if (yaku == FixedYakuType.ShiSanYao || yaku == FixedYakuType.QiDui) return;
            robbedYakus.Add(new Tuple<YakuType, int>(yaku, player.GetSkillSet().GetLevel(yaku)));
            player.GetSkillSet().SetLevel(yaku, 0);
        }
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return LuaArtifactBuilder.Create($"{this.name}_reversed", Rarity.COMMON)
            .OnSubscribeToPlayer((p, a) => p.RetrieveYakuMultiplierEvent += OnYakuMult)
            .OnUnsubscribeToPlayer((p, a) => p.RetrieveYakuMultiplierEvent -= OnYakuMult)
            .Build();
    }

    private void OnYakuMult(PlayerYakuEvent.RetrieveMultiplier yakuEvent)
    {
        if (yakuEvent.yakuType.ToYaku().rarity >= Rarity.RARE)
        {
            yakuEvent.multiplier *= 2;
        }
    }

    private void PostRoundEnd(PlayerEvent eventData)
    {
        Player player = eventData.player;
        foreach (var yaku in robbedYakus)
        {
            player.GetSkillSet().AddLevel(yaku.Item1, yaku.Item2);
        }

        robbedYakus.Clear();
    }
}