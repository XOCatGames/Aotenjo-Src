using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialHellWood : TileMaterialWood
    {
        private const double FAN_BASE = 3D;
        private const double FAN_EXTRA = 2D;
        public const string STAT_KEY = "hellwood_discarded";

        public TileMaterialHellWood(int ID) : base(ID, "hell_wood")
        {
        }

        public override int GetSpriteID(Player player)
        {
            if (player == null) return base.GetSpriteID();
            int count = player.stats.GetCustomStats(STAT_KEY);
            if (count <= 4)
                return 64;
            if (count <= 20)
                return 65;
            return 66;
        }

        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            return string.Format(GetDescription(localizer), GetFan(player), FAN_EXTRA);
        }

        private double GetFan(Player player)
        {
            return FAN_BASE + FAN_EXTRA * player.stats.GetCustomStats(STAT_KEY);
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            effects.Add(ScoreEffect.AddFan(() => GetFan(player), null));
        }

        public override void AppendToListDiscardEffect(Player player, Permutation perm, List<IAnimationEffect> effects,
            Tile tile, bool withForce, bool isClone)
        {
            base.AppendToListDiscardEffect(player, perm, effects, tile, withForce, isClone);
            effects.Add(new DiscardHellwoodEffect(tile));
            if (withForce && !isClone)
            {
                effects.Add(new TextEffect("effect_hell_wood_retrigger").OnTile(tile));
                player.AppendDiscardTileEffect(effects, tile, false, true);
            }
        }
    }

    public class DiscardHellwoodEffect : OnMultipleTileAnimationEffect
    {
        private Tile main;

        public DiscardHellwoodEffect(Tile main) : base(new SimpleEffect("effect_discard_hellwood", null,
            p => p.stats.RecordCustomStats(TileMaterialHellWood.STAT_KEY, 1)))
        {
            this.main = main;
        }

        public override List<Tile> GetAffectedTiles(Player player)
        {
            Permutation permutation = player.GetAccumulatedPermutation();
            return player.GetHandDeckCopy()
                .Union(permutation == null ? new List<Tile>() : permutation.ToTiles())
                .Where(t => t.properties.material is TileMaterialHellWood)
                .Append(main)
                .Distinct()
                .ToList();
        }

        public override Tile GetMainTile(Player player)
        {
            return main;
        }
    }
}