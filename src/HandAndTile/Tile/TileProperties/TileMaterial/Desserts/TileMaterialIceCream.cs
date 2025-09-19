using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialIceCream : TileMaterialDessert
    {
        private const double MULTIPLIER = 3;
        [SerializeField] private int materialID;

        public TileMaterialIceCream(int id): base(id, "dessert_ice_cream", 4)
        {
        }
        
        public TileMaterialIceCream(int id, int remainingUses) : base(id, "dessert_ice_cream", 4)
        {
            materialID = id;
            this.usesLeft = remainingUses;
        }

        public override int GetOrnamentSpriteID(Player player)
        {
            return 198;
        }

        public override Rarity GetRarity() => Rarity.RARE;

        protected override void AddDessertEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            effects.Add(ScoreEffect.MulFan(MULTIPLIER, null));

            if (player.Selecting(tile))
                effects.Add(new FreezeSameCategoryEffect(tile));
        }

        protected override int GetMaterialTransformThreshold() => 1;

        public override TileMaterial Copy()
        {
            return new TileMaterialIceCream(216);
        }

        protected override string GetDescription(Func<string, string> loc)
        {
            return string.Format(loc(GetDescriptionLocalizeKey()), usesLeft, maxUses, MULTIPLIER);
        }

        [Serializable]
        private class FreezeSameCategoryEffect : Effect
        {
            private readonly Tile src;
            public FreezeSameCategoryEffect(Tile s) { src = s; }

            public override string GetEffectDisplay(Func<string, string> f)
                => f("effect_freeze");

            public override Artifact GetEffectSource() => null;

            public override void Ingest(Player player)
            {
                var candidates = player.GetHandDeckCopy()
                                    .Except(player.GetSelectedTilesCopy())
                                    .Where(t => t.GetCategory() == src.GetCategory() &&
                                                t.properties.mask is not TileMaskFrozen)
                                    .ToList();

                if (candidates.Count == 0) return;

                Tile target = candidates[player.GenerateRandomInt(candidates.Count)];
                target.SetMask(TileMask.Frozen(), player);
            }
        }
    }
}
