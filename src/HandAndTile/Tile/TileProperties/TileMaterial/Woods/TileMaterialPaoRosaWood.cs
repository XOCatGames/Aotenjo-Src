using System;
using System.Collections.Generic;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialPaoRosaWood : TileMaterialWood
    {
        private const double MUL_PER_LEVEL = 0.5D;
        private const double MUL_BASE = 1.5D;
        public const int LEVEL_MAX = 3;
        public int level;

        public TileMaterialPaoRosaWood(int ID) : base(ID, "pao_rosa_wood")
        {
        }

        public override int GetOrnamentSpriteID(Player player)
        {
            return 49;
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), Utils.NumberToFormat(GetMul()),
                Utils.NumberToFormat(MUL_PER_LEVEL), Utils.NumberToFormat(LEVEL_MAX));
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            effects.Add(ScoreEffect.MulFan(GetMul(), null));
        }

        public override void AppendToListDiscardEffect(Player player, Permutation perm, List<IAnimationEffect> effects,
            Tile tile, bool withForce, bool isClone)
        {
            base.AppendToListDiscardEffect(player, perm, effects, tile, withForce, isClone);
            effects.Add(new UpgradeEffect(this).OnTile(tile));
        }

        private double GetMul()
        {
            return level * MUL_PER_LEVEL + MUL_BASE;
        }

        public override Rarity GetRarity()
        {
            return Rarity.RARE;
        }

        public override void SubscribeToPlayerEvents(Player player)
        {
            base.SubscribeToPlayerEvents(player);
            player.DetermineDiscardTileEvent += OnPreDiscardTile;
        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            player.DetermineDiscardTileEvent -= OnPreDiscardTile;
        }

        private void OnPreDiscardTile(PlayerDiscardTileEvent.Determine pre)
        {
            if (pre.tile.properties.material != this) return;
            if (!pre.forced && !pre.canceled)
            {
                pre.canceled = true;
            }
        }

        private class UpgradeEffect : TextEffect
        {
            private TileMaterialPaoRosaWood tileMatPaoRosa;

            public UpgradeEffect(TileMaterialPaoRosaWood tileMaterialJacarandaWood) : base("effect_pao_rosa_upgrade")
            {
                tileMatPaoRosa = tileMaterialJacarandaWood;
            }

            public override Artifact GetEffectSource()
            {
                return null;
            }

            public override void Ingest(Player player)
            {
                if (tileMatPaoRosa.level < LEVEL_MAX)
                {
                    tileMatPaoRosa.level++;
                }
            }
        }
    }
}