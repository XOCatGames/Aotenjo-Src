using System;
using System.Collections.Generic;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialDemon : TileMaterial
    {
        private const int MULTIPLIER = 2;

        public TileMaterialDemon(int ID) : base(ID, "demon", null)
        {
        }

        public override int GetOrnamentSpriteID(Player player)
        {
            return 59;
        }

        public override TileMaterial Copy()
        {
            return new TileMaterialDemon(5);
        }

        public override Rarity GetRarity()
        {
            return Rarity.RARE;
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MULTIPLIER);
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            effects.Add(ScoreEffect.MulFan(MULTIPLIER, null));
            if (!player.Selecting(tile)) return;
            effects.Add(new TransformEffect(this));
        }

        private class TransformEffect : Effect
        {
            private TileMaterialDemon material;

            public TransformEffect(TileMaterialDemon material)
            {
                this.material = material;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_eroded");
            }

            public override Artifact GetEffectSource()
            {
                return null;
            }

            public override void Ingest(Player player)
            {
                List<Tile> cands = player.GetSelectedTilesCopy();
                Tile tile = cands.Find(t => t.properties.material == material);
                int index = cands.IndexOf(tile);
                int handTotal = cands.Count;
                int offset = player.GenerateRandomInt(handTotal - 1) + 1;
                index = (index + offset) % handTotal;
                Tile toChange = cands[index];

                toChange.SetMask(TileMask.Corrupted(toChange), player);
            }
        }
    }
}