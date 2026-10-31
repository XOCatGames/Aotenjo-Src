using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 耗尽判定读取本局完整目录中的成长状态，包含未持有的对象；礼罐仍只统计当前库存。
    /// </summary>
    public sealed class VermilionBirdFanArtifact : Artifact
    {
        public VermilionBirdFanArtifact() : base("vermilion_bird_fan", Rarity.RARE)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            string[] ids = { Artifacts.Tanghulu.Id, Artifacts.BambooWine.Id, Artifacts.Mower.Id, Artifacts.Xiaolongbao.Id, Artifacts.FrogToy.Id, Artifacts.HalfLucky47.Id, Artifacts.UnluckyFour.Id, Artifacts.Saw.Id };
            var list = player.GetRunArtifacts().Where(a => ids.Contains(a.GetRegName()));
            int arc = list.Count(a => ((LevelingArtifact)a).Level == 0);
            arc += player.GetArtifacts().OfType<PinataArtifact>().Count(a => a.IsComplete);
            int cnt = player.GetGadgets().Count(a => !a.IsConsumable() && a.uses == 0);

            for (int i = 0; i < arc; i++)
            {
                effects.Add(ScoreEffect.MulFan(2f, this));
            }

            for (int i = 0; i < cnt; i++)
            {
                effects.Add(ScoreEffect.MulFan(1.2f, this));
            }
        }
    }
}
