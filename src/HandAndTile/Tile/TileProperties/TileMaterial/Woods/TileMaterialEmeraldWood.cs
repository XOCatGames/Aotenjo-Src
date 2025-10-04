using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialEmeraldWood : TileMaterialWood
    {
        private const int FU = 5;
        [SerializeField] public int count;

        public TileMaterialEmeraldWood(int id) : base(id, "emerald_wood")
        {
        }

        protected override int GetSpriteID()
        {
            return count switch
            {
                <= 8 => 54,
                <= 16 => 55,
                <= 24 => 56,
                _ => 57
            };
        }

        public override TileMaterial Copy()
        {
            TileMaterialEmeraldWood mat = new TileMaterialEmeraldWood(GetSpriteID());
            mat.count = count;
            return mat;
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU, FU * 2, GetFu());
        }

        private double GetFu()
        {
            return count * FU;
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            effects.Add(ScoreEffect.AddFu(GetFu, null));
        }

        public override void SubscribeToPlayerEvents(Player player)
        {
            base.SubscribeToPlayerEvents(player);
            player.PostRoundEndEvent += PostRoundEnd;
        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            player.PostRoundEndEvent -= PostRoundEnd;
        }

        private void PostRoundEnd(PlayerEvent playerEvent)
        {
            
            count = 0;
        }
    }
}