using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

[Serializable]
public class ReadingFlowerTile : OneTimeUseFlowerTile
{
    public ReadingFlowerTile() : base(Category.SiYe, 4)
    {
    }

    public override void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendScoringEffect(effects, player, perm);
        if (used) return;
        used = true;
        effects.Add(new OnTileAnimationEffect(this, new UpgradeYakusEffect()));
    }


    private class UpgradeYakusEffect : Effect
    {
        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func("effect_flower_read_name");
        }

        public override Artifact GetEffectSource()
        {
            return null;
        }

        public override void Ingest(Player player)
        {
            Permutation perm = player.GetCurrentSelectedPerm();
            List<YakuType> activatedYakuTypes = perm.GetYakus(player);

            List<YakuType> validYakus = activatedYakuTypes
                .Where(y => !activatedYakuTypes.Any(t => YakuTester.GetYakuChildsExcludeSelf(t).Contains(y))).ToList();
            validYakus.ForEach(y => { player.GetSkillSet().IncreaseLevel(y); });
        }
    }
}