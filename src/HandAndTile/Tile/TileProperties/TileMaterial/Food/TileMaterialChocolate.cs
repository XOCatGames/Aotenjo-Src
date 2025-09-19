using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialChocolate : TileMaterial
    {
        private const int INITIAL_LEVEL = 2;
        private const double DECREMENT = 0.25;

        [SerializeField] private int level = INITIAL_LEVEL;

        public TileMaterialChocolate(int ID) : base(ID, "chocolate", null)
        {
        }

        private TileMaterialChocolate(int ID, int level) : this(ID)
        {
            this.level = level;
        }

        public override TileMaterial Copy()
        {
            return new TileMaterialChocolate(4, level);
        }

        public override Effect[] GetEffects(Player player, Permutation permutation)
        {
            Effect[] baseEffects = base.GetEffects(player, permutation);
            List<Effect> effects = new List<Effect>(baseEffects);
            if (level > 0)
            {
                effects.Add(new ChocolateEffect(this, level));
                effects.Add(new TextEffect("effect_melting", null, "Food"));
            }

            return effects.ToArray();
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(localizer("tile_chocolate_material_description"), 1 + DECREMENT * level, DECREMENT);
        }

        private void Melt(Player player)
        {
            if (level > 0)
            {
                level--;
                player.stats.RecordCustomStats(PlayerStatsType.EAT_FOOD, 1);
                if (level == 0)
                {
                    Tile tile = player.GetAllTiles().Find(t => t.properties.material == this);
                    tile.SetMaterial(PLAIN, player, true);
                }
            }
        }

        private class ChocolateEffect : Effect
        {
            private TileMaterialChocolate TileMaterial;
            private ScoreEffect ScoreEffect;
            private int level;

            public ChocolateEffect(TileMaterialChocolate TileMaterial, int level)
            {
                this.TileMaterial = TileMaterial;
                ScoreEffect = ScoreEffect.MulFan(1 + DECREMENT * level, null);
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return ScoreEffect.GetEffectDisplay(func);
            }

            public override Artifact GetEffectSource()
            {
                return ScoreEffect.GetEffectSource();
            }

            public override void Ingest(Player player)
            {
                (ScoreEffect).Ingest(player);
                TileMaterial.Melt(player);
            }
        }
    }
}