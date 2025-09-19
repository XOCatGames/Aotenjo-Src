using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class NeonBagArtifact : SneakyArtifact
    {
        private const float FU = 40f;

        public NeonBagArtifact() : base("neon_bag", Rarity.RARE)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (player is SneakyPlayer p)
            {
                if (p.SneakedLastRound(tile))
                {
                    effects.Add(ScoreEffect.AddFu(FU, this));
                }
            }
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            if (player is SneakyPlayer p)
            {
                foreach (var item in p.sneakedTiles.Where(t => t.IsNumbered()))
                {
                    effects.Add(new OnTileAnimationEffect(item, new ChangeSuitEffect(item, this)));
                }
            }
        }

        private class ChangeSuitEffect : Effect
        {
            private Tile item;
            private NeonBagArtifact neonBagArtifact;

            public ChangeSuitEffect(Tile item, NeonBagArtifact neonBagArtifact)
            {
                this.item = item;
                this.neonBagArtifact = neonBagArtifact;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_change_suit");
            }

            public override Artifact GetEffectSource()
            {
                return neonBagArtifact;
            }

            public override void Ingest(Player player)
            {
                item.ModifyCategory(
                    (Tile.Category)(((int)item.GetBaseCategory() + 1 + player.GenerateRandomInt(2)) % 3), player);
            }
        }
    }
}