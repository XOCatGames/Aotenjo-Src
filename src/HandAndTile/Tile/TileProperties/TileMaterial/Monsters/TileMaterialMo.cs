using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialMo : TileMaterial
    {
        private const double MUL = 3f;
        private const int DISCARD_COUNT = 5;

        public TileMaterialMo(int ID) : base(ID, "mo", null)
        {
        }

        public override Rarity GetRarity()
        {
            return Rarity.RARE;
        }

        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            return string.Format(base.GetDescription(localizer), MUL, DISCARD_COUNT);
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            if (player.Selecting(tile))
            {
                effects.Add(new MoEffect());
            }

            effects.Add(ScoreEffect.MulFan(MUL, null));
        }

        private class MoEffect : Effect
        {
            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_mo_discard_name");
            }

            public override Artifact GetEffectSource()
            {
                return null;
            }

            public override void Ingest(Player player)
            {
                //1. 找X枚随机牌
                List<Tile> toSwap = new List<Tile>();
                List<Tile> handCopy = player.GetHandDeckCopy().Except(player.GetSelectedTilesCopy())
                    .Where(t => t != null).ToList();
                for (int i = 0; i < DISCARD_COUNT; i++)
                {
                    if (handCopy.Count == 0) break;
                    Tile t = handCopy[player.GenerateRandomInt(handCopy.Count)];
                    toSwap.Add(t);
                    handCopy.Remove(t);
                }
                
                toSwap.ForEach( t => MessageManager.Instance.EnqueueToDiscard(t, true));
                
                // //2. 弃牌
                // List<int> posDrew = toSwap.Select(t => player.GetHandDeckCopy().IndexOf(t)).ToList();
                // toSwap.ForEach(t => { posDrew.Add(player.ReplaceTileAndKeepPosition(t)); });
                //
                // //3. 通过事件总线通知播放动画
                // EventManager.Instance.OnDrawTiles(posDrew);
            }
        }
    }
}