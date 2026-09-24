using System.Linq;

namespace Aotenjo
{
    public class ImpossibleAchievement : Achievement
    {
        private static YakuType[] IMPOSSIBLE_YAKUS =
        {
            FixedYakuType.WuGuiYi,
            FixedYakuType.DuoGuiYi,
            FixedYakuType.ShuangTongZiKe,
            FixedYakuType.ShuangTongZiShun,
            FixedYakuType.YiSeShuangTongKe,
            FixedYakuType.SanTongZiKe,
            FixedYakuType.SanTongZiShun,
            FixedYakuType.YiSeSanTongKe,
            FixedYakuType.SiTongZiKe,
            FixedYakuType.SiTongZiShun,
            FixedYakuType.YiSeSiTongKe,
            FixedYakuType.SiFengShun
        };

        public ImpossibleAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            EventBus.Subscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, PreAppendSettleScoringEffects);
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            EventBus.Unsubscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, PreAppendSettleScoringEffects);
        }

        private void PreAppendSettleScoringEffects(PlayerPermutationEvent permutationEvent)
        {
            Player player = permutationEvent.player;
            Permutation permutation = permutationEvent.permutation;
            if (permutation.GetYakus(player)
                .Where(y => player.GetSkillSet().GetLevel(y) > 0 || permutation.IsFullHand(player))
                .Any(y => IMPOSSIBLE_YAKUS.Contains(y)))
            {
                SetComplete();
            }
        }
    }
}