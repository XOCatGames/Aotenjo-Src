using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    internal class GlitchedOrizulu : Artifact
    {
        public GlitchedOrizulu() : base("glitched_orizulu", Rarity.EPIC)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            if (!permutation.JiangFulfillAll(t => t.IsHonor(player))) return;
            effects.Add(ScoreEffect.MulFan(3, this));
            effects.Add(new GlitchedOrizuluEffect(this));
        }
    }

    internal class GlitchedOrizuluEffect : Effect
    {
        private GlitchedOrizulu glitchedOrizulu;

        public GlitchedOrizuluEffect(GlitchedOrizulu glitchedOrizulu)
        {
            this.glitchedOrizulu = glitchedOrizulu;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func("effect_corrupt");
        }

        public override string GetSoundEffectName()
        {
            return "Corrupt";
        }

        public override Artifact GetEffectSource()
        {
            return glitchedOrizulu;
        }

        public override void Ingest(Player player)
        {
            LotteryPool<Tile> cand = new LotteryPool<Tile>();
            cand.AddRange(player.GetUnusedTilesInHand().Where(t =>
                t != null && t.properties.mask.GetRegName() != TileMask.Corrupted().GetRegName()));
            for (int i = 0; i < 3; i++)
            {
                if (cand.IsEmpty())
                {
                    break;
                }

                Tile tile = cand.Draw(player.GenerateRandomInt, false);
                tile.SetMask(TileMask.Corrupted(), player);
            }
        }
    }
}