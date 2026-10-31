using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class BlackHoleArtifact : LevelingArtifact
    {
        public BlackHoleArtifact() : base("black_hole", Rarity.EPIC, 10)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            if (Level != 0)
                return string.Format(base.GetDescription(localizer), Level, GetChanceMultiplier(player));
            // 成长后的模板只接收概率分子，仍需应用赤云麻将的概率加成，避免直接显示占位符。
            return string.Format(localizer("artifact_black_hole_description_grown"), GetChanceMultiplier(player));
        }

        public override bool IsAvailableInShops(Player player)
        {
            return false;
        }

        public override List<string> GetComponentIds()
        {
            return ArtifactRecipes.recipes
                .First(r => r.outputID.Contains(GetNameID()))
                .inputID
                .Append(GetRegName())
                .ToList();
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerEvents.PostUseGadgetEvent>(player, OnPostUseGadgetEvent);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerEvents.PostUseGadgetEvent>(player, OnPostUseGadgetEvent);
        }

        private void OnPostUseGadgetEvent(PlayerGadgetEvent evt)
        {
            Gadget gadget = evt.gadget;

            if (gadget.IsConsumable() && evt.player.GenerateRandomDeterminationResult(3) && gadget.uses == 1)
            {
                Gadget newGadget = gadget.Copy();
                newGadget.SetUses(1);
                evt.player.RequestGadgetReward(newGadget, source: this);
            }
        }


        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            if (Level == 0)
            {
                effects.Add(ScoreEffect.AddFan(100, this));
            }
        }
    }
}
