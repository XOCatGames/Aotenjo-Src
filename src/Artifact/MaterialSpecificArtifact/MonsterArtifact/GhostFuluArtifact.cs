using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class GhostFuluArtifact : Artifact
    {
        public Tile tile;

        public GhostFuluArtifact() : base("ghost_fulu", Rarity.COMMON)
        {
            
        }

        public override bool ShouldHighlightTile(Tile t, Player player)
        {
            return t.CompatWith(tile);
        }

        public override string GetDescription(Player player, Func<string, string> loc)
        {
            return string.Format(base.GetDescription(player, loc), tile.GetLocalizedName(loc));
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (tile.CompatWith(this.tile))
            {
                effects.Add(new TransformMaterialEffect(TileMaterial.Ghost(), this, tile, "effect_ghost_fulu_name"));
            }
        }

        public override string Serialize()
        {
            return tile.ToString();
        }

        public override void Deserialize(string data)
        {
            tile = new Tile(data);
        }

        public override void ResetArtifactState(Player player)
        {
            base.ResetArtifactState(player);
            List<Tile> tiles = player.GetUniqueFullDeck();
            tile = tiles[new Random().Next(tiles.Count)];
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerRoundEvent.End.Pre>(PreRoundEnd);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerRoundEvent.End.Pre>(PreRoundEnd);
        }

        private void PreRoundEnd(PlayerEvent playerEvent)
        {
            ChangeGhostingTile(playerEvent.player);
        }

        private void ChangeGhostingTile(Player player)
        {
            List<Tile> cands = player.GetUniqueFullDeck().Where(t => player.GetAllTiles().Any(t2 => t2.CompatWith(t)))
                .ToList();
            if (cands.Count == 0)
            {
                return;
            }

            Tile newTile = cands[player.GenerateRandomInt(cands.Count)];
            tile = newTile;
        }
    }
}