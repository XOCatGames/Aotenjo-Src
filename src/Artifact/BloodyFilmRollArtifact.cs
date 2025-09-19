using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class BloodyFilmRollArtifact : Artifact
    {
        private HashSet<Tile> affectedTiles = new HashSet<Tile>();
        
        public BloodyFilmRollArtifact() : base("bloody_film_roll", Rarity.EPIC)
        {
            SetPrerequisite(p => p.GetAllTiles().Any(t => p.DetermineFontCompactbility(t, TileFont.COLORLESS)));
            SetHighlightRequirement((t, p) => p.DetermineFontCompactbility(t, TileFont.COLORLESS));
        }
        
        public override void ResetArtifactState(Player player)
        {
            base.ResetArtifactState(player);
            affectedTiles = new HashSet<Tile>();
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (!player.DetermineFontCompactbility(tile, TileFont.COLORLESS) || !player.Selecting(tile)) return;
            
            //防止递归
            if (!affectedTiles.Add(tile)) return;
            
            effects.Add(new TextEffect("effect_bloody_film_roll", this));
            
            //重复计分
            effects.Add(new TileScoringEffectAppendEffect(player, tile, permutation, player.playHandEffectStack));
            effects.Add(new SilentEffect(() => affectedTiles.Remove(tile)));
        }
    }
}