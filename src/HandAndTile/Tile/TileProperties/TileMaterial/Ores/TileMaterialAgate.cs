using System;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialAgate : TileMaterial
    {
        private const int FAN = 3;

        public TileMaterialAgate(int ID) : base(ID, "agate", null)
        {
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(localizer("tile_agate_material_description"), FAN);
        }

        public override Effect[] GetEffects(Player player, Permutation perm)
        {
            Effect[] baseEffects = base.GetEffects();
            Array.Resize(ref baseEffects, baseEffects.Length + 1);
            baseEffects[^1] = new AgateEffect(FAN);
            return baseEffects;
        }
    }

    public class AgateEffect : Effect
    {
        private int base_fan;

        public AgateEffect(int fan)
        {
            base_fan = fan;
        }

        public override string GetEffectDisplay(Player player, Func<string, string> func)
        {
            return string.Format(func("effect_add_fan_format"), base_fan * GetAgateCount(player));
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return "ERROR!";
        }

        public override string GetSoundEffectName()
        {
            return "Agate";
        }

        public override Artifact GetEffectSource()
        {
            return null;
        }

        public override void Ingest(Player player)
        {
            int count = GetAgateCount(player);
            player.RoundAccumulatedScore = player.RoundAccumulatedScore.AddFan(base_fan * count);
        }

        public static int GetAgateCount(Player player)
        {
            Permutation perm = player.GetCurrentSelectedPerm();
            perm ??= player.GetAccumulatedPermutation();
            if (perm == null)
            {
                return 0;
            }

            return perm.ToTiles().Where(t => t.CompactWithMaterial(TileMaterial.Agate(), player)).Count();
        }
    }
}