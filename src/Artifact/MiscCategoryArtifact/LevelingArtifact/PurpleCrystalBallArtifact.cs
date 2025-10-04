using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class PurpleCrystalBallArtifact : LevelingArtifact, IFanProvider, IJade
    {
        private const int FAN_PER_LEVEL = 4;

        public PurpleCrystalBallArtifact() : base("purple_crystal_ball", Rarity.RARE, 0)
        {
            SetHighlightRequirement((t, player) => t.IsYaoJiu(player));
        }

        public override string GetDescription(Player p, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FAN_PER_LEVEL, this.GetEffectiveJadeStack(p));
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);

            effects.Add(ScoreEffect.AddFan(() => GetFan(player), this));
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToAddFanFormat(GetFan(player));
        }

        public double GetFan(Player player)
        {
            return this.GetEffectiveJadeStack(player) * FAN_PER_LEVEL;
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block,
            List<Effect> effects)
        {
            if (!block.Any(player.Selecting)) return;

            if (block.Any(t => t.IsYaoJiu(player)))
            {
                effects.Add(new SimpleEffect("effect_purple_crystal_ball_upgrade", this, _ => this.Level++));
            }
            else
            {
                effects.Add(new SimpleEffect("effect_purple_crystal_ball_downgrade", this, _ => this.Level = Math.Max(0, this.Level - 1)));
            }
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.RetrieveEffectiveJadeStackEvent += PlayerOnRetrieveEffectiveJadeStackEvent;
        }
        
        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.RetrieveEffectiveJadeStackEvent -= PlayerOnRetrieveEffectiveJadeStackEvent;
        }

        private void PlayerOnRetrieveEffectiveJadeStackEvent(PlayerJadeEvent.RetrieveEffectiveStack evt)
        {
            if(evt.jade is Artifact a && evt.player.DetermineNeighborArtifacts(this, a))
                evt.effectiveStack += this.GetEffectiveJadeStack(evt.player) / 2;
        }
    }
}