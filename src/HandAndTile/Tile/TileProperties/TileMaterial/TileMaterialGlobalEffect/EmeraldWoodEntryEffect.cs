using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo.TileMaterialGlobalEffect
{
    public class EmeraldWoodEntryEffect : TileMaterialEntryEffect
    {
        public EmeraldWoodEntryEffect() : base("emerald_wood_entry_effect")
        {
        }
        
        public override void SubscribeToPlayerEvents(Player player)
        {
            base.SubscribeToPlayerEvents(player);
            player.OnAddSingleDiscardTileAnimationEffectEvent += Player_OnAddSingleDiscardTileAnimationEffectEvent;
        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            player.OnAddSingleDiscardTileAnimationEffectEvent -= Player_OnAddSingleDiscardTileAnimationEffectEvent;
        }

        private void Player_OnAddSingleDiscardTileAnimationEffectEvent(Player p, List<IAnimationEffect> effects,
            Tile onTile, bool forced)
        {
            foreach (Tile t in p.GetAllTiles())
            {
                if (t == null || t.GetCategory() != onTile.GetCategory() ||
                    t.properties.material is not TileMaterialEmeraldWood) continue;
                effects.Add(new EmeraldWoodDiscardEffect(onTile, forced));
                return;
            }
        }

        private class EmeraldWoodDiscardEffect : OnMultipleTileAnimationEffect
        {
            private Tile tile;
            private int count;

            public EmeraldWoodDiscardEffect(Tile tile, bool forced) : base(new EmeraldWoodEffect(tile, forced))
            {
                this.tile = tile;
            }

            public override List<Tile> GetAffectedTiles(Player player)
            {
                return player.GetSettledTiles()
                    .Union(player.GetHandDeckCopy())
                    .Where(t => t.properties.material is TileMaterialEmeraldWood &&
                                t.GetCategory() == tile.GetCategory())
                    .Append(tile)
                    .Distinct()
                    .ToList();
            }

            public override Tile GetMainTile(Player player)
            {
                return tile;
            }

            private class EmeraldWoodEffect : TextEffect
            {
                public Tile Tile { get; }
                public bool Forced { get; }

                public EmeraldWoodEffect(Tile tile, bool forced) : base("effect_emerald_wood_grow", null, "Grow")
                {
                    Tile = tile;
                    Forced = forced;
                }

                public override string GetEffectDisplay(Player player, Func<string, string> func)
                {
                    int count = player.GetAllTiles().Count(t => 
                        t?.properties.material is TileMaterialEmeraldWood &&
                        t.GetCategory() == Tile.GetCategory());
                    return string.Format(func("effect_emerald_wood_grow"), count);
                }

                public override void Ingest(Player player)
                {
                    base.Ingest(player);
                    foreach (var item in player.GetAllTiles()
                                 .Where(t => t != null && t.GetCategory() == Tile.GetCategory() &&
                                             t.properties.material is TileMaterialEmeraldWood)
                                 .Select(t => (TileMaterialEmeraldWood)t.properties.material))
                    {
                        if (Forced) item.count += 4;
                        else item.count++;
                    }
                }
            }
        }
    }
}