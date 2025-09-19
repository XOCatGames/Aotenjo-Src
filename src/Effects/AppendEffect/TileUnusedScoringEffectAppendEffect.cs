using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class TileUnusedScoringEffectAppendEffect : ScoringEffectAppendEffect
    {
        private readonly Player player;
        private readonly Tile tile;
        private readonly Permutation permutation;

        public TileUnusedScoringEffectAppendEffect(Player player, Tile tile, Permutation permutation, Stack<IAnimationEffect> effectStack) : base(effectStack)
        {
            this.player = player;
            this.tile = tile;
            this.permutation = permutation;
        }

        public override List<IAnimationEffect> GetEffects()
        {
            List<IAnimationEffect> animationEffects = new List<IAnimationEffect>();
            List<Effect> effects = new();
            tile.properties.AppendUnusedEffects(player, permutation, effects);
            
            animationEffects.AddRange(effects.ConvertAll(e => e.OnTile(tile)));
            
            foreach (var onTile in permutation.ToTiles().OrderBy(t => player.TileSettlingOrder(t, permutation)))
            {
                //TODO: 不够动态
                effects.Clear();
                tile.properties.AppendToListOnTileUnusedEffect(player, permutation, effects, tile, onTile);
                animationEffects.AddRange(effects.ConvertAll(e => e.OnTile(onTile)));
            }
            
            animationEffects.Add(new ArtifactOnTileUnusedAppendEffect(effectStack, tile, permutation, player, 0));
            
            return animationEffects;
        }
    }
}