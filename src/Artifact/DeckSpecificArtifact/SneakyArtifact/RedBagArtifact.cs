using System.Collections.Generic;

namespace Aotenjo
{
    public class RedBagArtifact : SneakyArtifact
    {
        private const float FAN = 4f;

        public RedBagArtifact() : base("red_bag", Rarity.COMMON)
        {
        }

        public override bool ShouldHighlightTile(Tile tile, Player player)
        {
            if (player is SneakyPlayer sneakyPlayer)
            {
                return (sneakyPlayer.SneakedLastRound(tile));
            }

            return false;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (player is SneakyPlayer sneakyPlayer)
            {
                if (sneakyPlayer.SneakedLastRound(tile))
                {
                    effects.Add(ScoreEffect.AddFan(FAN, this));
                }
            }
        }
    }
}