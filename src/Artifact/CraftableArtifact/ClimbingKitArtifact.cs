using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class ClimbingKitArtifact : CountPairArtifact
    {
        private const int ADD_FU = 40;

        public ClimbingKitArtifact() : base("climbing_kit", Rarity.RARE)
        {
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

        public override bool CanBeSellByPlayer()
        {
            return false;
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.DetermineShiftedPairEvent += OnDetermineShifted;
            player.OnAddSingleAnimationEffectEvent += PostAddScoringEffect;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.DetermineShiftedPairEvent -= OnDetermineShifted;
            player.OnAddSingleAnimationEffectEvent -= PostAddScoringEffect;
        }

        private void PostAddScoringEffect(Player player, List<IAnimationEffect> lst, IAnimationEffect arg4)
        {
            lst.RemoveAll(effect =>
            {
                Effect eff = effect.GetEffect();
                if (eff is not ScoreEffect) return false;
                Artifact artifact = eff.GetEffectSource();
                if (artifact == null || artifact.GetNameID() == null || artifact.GetNameID() == "") return false;
                return (player.IsArtifactDebuffed(artifact));
            });
        }

        private void OnDetermineShifted(PlayerDetermineShiftedPairEvent evt)
        {
            bool res = evt.res;
            if (res || evt.step != 1) return;
            evt.res = evt.player.DetermineShiftedPair(evt.b1, evt.b2, 2, evt.catSensitive);
        }

        public override string GetDescription(Func<string, string> localize)
        {
            return string.Format(base.GetDescription(localize), ADD_FU);
        }

        
        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);

            effects.Add(new SilentEffect(() =>
            {
                foreach (var (b1, b2) in GetPairsWithDiff(permutation, 1, player))
                {
                    player.playHandEffectStack.Push(ScoreEffect.AddFu(ADD_FU, this).OnMultipleTiles(b1.tiles.Union(b2.tiles).ToList(), b1.tiles[1]));
                }
            }));
        }
    }
}