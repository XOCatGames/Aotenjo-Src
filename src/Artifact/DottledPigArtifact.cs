using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 只在赤云玩家上读取本局核心的未使用类别，其他套牌不产生此加成。
    /// </summary>
    public sealed class DottledPigArtifact : Artifact
    {
        public DottledPigArtifact() : base("dottled_pig", Rarity.EPIC)
        {
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            if(player is ScarletPlayer scarletPlayer)
            {
                if (scarletPlayer.IsCompatibleWithUnusedCategory(block.GetCategory()))
                {
                    effects.Add(ScoreEffect.MulFan(1.5D, this));
                }
            }
        }
    }
}
