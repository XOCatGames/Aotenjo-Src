using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class EmeraldWoodBracelet : Artifact
    {
        public EmeraldWoodBracelet() : base("emerald_wood_bracelet", Rarity.RARE)
        {
            SetHighlightRequirement((t, p) => t.CompactWithMaterial(TileMaterial.EmeraldWood(), p));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.OnPostAddScoringAnimationEffectEvent += OnPostAddScoringAnimationEffect;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.OnPostAddScoringAnimationEffectEvent -= OnPostAddScoringAnimationEffect;
        }

        private void OnPostAddScoringAnimationEffect(Permutation perm, Player player, List<IAnimationEffect> list)
        {
            List<IAnimationEffect> emeraldWoodEffects = new();

            foreach (var effect in list)
            {
                if (effect is OnTileAnimationEffect onTile &&
                    onTile.tile.CompactWithMaterial(TileMaterial.EmeraldWood(), player))
                {
                    emeraldWoodEffects.Add(onTile.Clone());
                }
            }

            IEnumerable<Yaku> yakus = perm.GetYakus(player).Select(t => YakuTester.InfoMap[t]).Where(y =>
                player.GetSkillSet().GetLevel(y) > 0 && y.GetYakuCategories().Contains(1));
            foreach (Yaku yaku in yakus)
            {
                list.Add(new EmeraldWoodTextEffect(yaku, this));
                foreach (var effect in emeraldWoodEffects)
                {
                    list.Add(effect);
                }
            }
        }

        private class EmeraldWoodTextEffect : TextEffect
        {
            public Yaku Yaku { get; }

            public EmeraldWoodTextEffect(Yaku yaku, Artifact artifact) : base("effect_emerald_wood_bracelet_format",
                artifact)
            {
                Yaku = yaku;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return string.Format(base.GetEffectDisplay(func), Yaku.GetFormattedName(func));
            }
        }
    }
}