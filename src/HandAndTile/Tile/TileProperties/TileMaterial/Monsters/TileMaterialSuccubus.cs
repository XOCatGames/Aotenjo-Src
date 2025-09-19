using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialSuccubus : TileMaterial
    {
        private const double FAN = 10f;

        public TileMaterialSuccubus(int ID) : base(ID, "succubus", null)
        {
        }

        public override int GetOrnamentSpriteID(Player player)
        {
            if (player == null) return -1;
            return player.GetArtifacts().Any(a => a.GetRegName() == Artifacts.VioletGoatHorn.GetRegName())? 
                39 : base.GetOrnamentSpriteID(player);
        }

        public override Rarity GetRarity()
        {
            return Rarity.COMMON;
        }

        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            return string.Format(base.GetDescription(localizer), FAN);
        }

        public override Effect[] GetEffects()
        {
            Effect[] baseEffects = base.GetEffects();
            Array.Resize(ref baseEffects, baseEffects.Length + 1);
            baseEffects[^1] = ScoreEffect.AddFan(FAN, null);
            return baseEffects;
        }

        public override void AppendUnusedEffects(Player player, Permutation perm, List<Effect> effects)
        {
            base.AppendUnusedEffects(player, perm, effects);
            effects.Add(ScoreEffect.AddFan(-FAN, null));
        }
    }
}