﻿using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class PatchouliKnowledgeAchievement : Achievement
    {
        public PatchouliKnowledgeAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.PostRunEndEvent += PostRunEnd;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.PostRunEndEvent -= PostRunEnd;
        }

        private void PostRunEnd(Player player, bool won, PlayerStats stats)
        {
            List<YakuPack> yakuPacks = GameManager.Instance.yakuPacks;
            if (YakuTester.YAKUS_PREDICATE_MAP.Keys
                .Where(y => yakuPacks.Any(p => p.ContainsYaku(YakuTester.InfoMap[y]))).All(y => stats.PlayedYaku(y)))
                SetComplete();
        }
    }
}