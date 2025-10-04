using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class DaturaArtifact : LevelingArtifact, IMultiplierProvider
    {
        public const int MAX_LEVEL = 20;
        public const float MUL_PER_LEVEL = 0.1f;

        public DaturaArtifact() : base("datura", Rarity.EPIC, 0)
        {
        }

        public override string GetDescription(Player p, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MUL_PER_LEVEL,
                Utils.NumberToFormat(1f + MAX_LEVEL * MUL_PER_LEVEL), Utils.NumberToFormat(GetMul(p)));
        }
        
        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToMulFanFormat(GetMul(player));
        }

        public double GetMul(Player player)
        {
            return 1f + (Level * MUL_PER_LEVEL);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            if (Level > 0)
                effects.Add(ScoreEffect.MulFan(GetMul(player), this));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            ((RainbowDeck.RainbowPlayer)player).PostPlayFlowerTileEvent += OnPlayFlowerTile;
        }

        private void OnPlayFlowerTile(Player player, FlowerTile flowerTile)
        {
            if (Level < 20)
                Level++;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            ((RainbowDeck.RainbowPlayer)player).PostPlayFlowerTileEvent -= OnPlayFlowerTile;
        }
    }
}