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
            return localizer("artifact_black_hole_description_grown");
        }

        public override bool IsAvailableInShops(Player player)
        {
            return false;
        }

        public override List<Artifact> GetComponents()
        {
            return ArtifactRecipes.recipes
                .First(r => r.outputID.Contains(GetNameID()))
                .inputID
                .Select(i => Artifacts.ArtifactList.First(a => a.GetNameID() == i))
                .Append(this)
                .ToList();
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostUseGadgetEvent += OnPostUseGadgetEvent;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostUseGadgetEvent -= OnPostUseGadgetEvent;
        }

        private void OnPostUseGadgetEvent(PlayerGadgetEvent evt)
        {
            Gadget gadget = evt.gadget;

            if (gadget.IsConsumable() && evt.player.GenerateRandomDeterminationResult(3) && gadget.uses == 1)
            {
                Gadget newGadget = gadget.Copy();
                newGadget.SetUses(1);
                evt.player.AddGadget(newGadget);
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