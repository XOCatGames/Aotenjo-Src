using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class AmethystAmuletArtifact : Artifact
    {
        
        private const int GIFT_COUNT = 10;
        
        public AmethystAmuletArtifact() : base("amethyst_amulet", Rarity.RARE)
        {
            SetHighlightRequirement((tile, _) =>
                tile.properties.mask.GetRegName() == TileMask.Corrupted().GetRegName());
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), GIFT_COUNT);
        }

        public override void OnObtain(Player player)
        {
            base.OnObtain(player);
            foreach (var plainTile in player.DrawPlainTilesFromPool(GIFT_COUNT))
            {
                plainTile.SetMask(TileMask.Corrupted(), player);
            }
        }


        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerEvents.PostAddSingleTileAnimationEffectEvent>(player, Decorrupt);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerEvents.PostAddSingleTileAnimationEffectEvent>(player, Decorrupt);
        }

        private void Decorrupt(Permutation arg1, Player arg2, List<OnTileAnimationEffect> arg3, OnTileAnimationEffect arg4, Tile arg5)
        {
            if(arg4.effect is TileMaskCorrupted.CorruptedEffect cefff)
            {
                cefff.coefficient = 1f;
            }
        }
    }
}