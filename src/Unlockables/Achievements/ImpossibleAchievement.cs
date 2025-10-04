using System.Linq;

namespace Aotenjo
{
    public class ImpossibleAchievement : Achievement
    {
        private static YakuType[] IMPOSSIBLE_YAKUS =
        {
            YakuType.WuGuiYi,
            YakuType.DuoGuiYi,
            YakuType.ShuangTongZiKe,
            YakuType.ShuangTongZiShun,
            YakuType.YiSeShuangTongKe,
            YakuType.SanTongZiKe,
            YakuType.SanTongZiShun,
            YakuType.YiSeSanTongKe,
            YakuType.SiTongZiKe,
            YakuType.SiTongZiShun,
            YakuType.YiSeSiTongKe,
            YakuType.SiFengShun
        };

        public ImpossibleAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.PreAppendSettleScoringEffectsEvent += PreAppendSettleScoringEffects;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.PreAppendSettleScoringEffectsEvent -= PreAppendSettleScoringEffects;
        }

        private void PreAppendSettleScoringEffects(PlayerPermutationEvent permutationEvent)
        {
            Player player = permutationEvent.player;
            Permutation permutation = permutationEvent.permutation;
            if (permutation.GetYakus(player)
                .Where(y => player.GetSkillSet().GetLevel(y) > 0 || permutation.IsFullHand())
                .Any(y => IMPOSSIBLE_YAKUS.Contains(y)))
            {
                SetComplete();
            }
        }
    }
}