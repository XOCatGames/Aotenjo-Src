using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class HalfLucky47Artifact : LevelingArtifact, ICountable
    {
        public HalfLucky47Artifact() : base("half_lucky_47", Rarity.RARE, UnluckyFourArtifact.PROC_LIMIT)
        {
            SetHighlightRequirement((t, _) => (t.GetOrder() == 4 || t.GetOrder() == 7) && t.IsNumbered());
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

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.Selecting(tile)) return;
            if (tile.IsNumbered() && (tile.GetOrder() == 4 || tile.GetOrder() == 7))
            {
                int res = player.GenerateRandomInt(7);
                if (0 == res)
                {
                    effects.Add(new LuckySevenArtifact.LuckyEffect(this, tile));
                }
            }
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(player, localizer), Level, UnluckyFourArtifact.PROC_LIMIT,
                GetChanceMultiplier(player));
        }

        public override void AppendDiscardTileEffects(Player player, Tile tile,
            List<IAnimationEffect> onDiscardTileEffects, bool withForce, bool isClone)
        {
            base.AppendDiscardTileEffects(player, tile, onDiscardTileEffects, withForce, isClone);
            if (tile.IsNumbered() && tile.GetOrder() == 4 && player.GenerateRandomDeterminationResult(4) && Level > 0)
            {
                onDiscardTileEffects.Add(new UnluckyFourEffect(this, tile));
            }
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerRoundEvent.Start.Pre>(OnRoundStart);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerRoundEvent.Start.Pre>(OnRoundStart);
        }

        public void OnRoundStart(PlayerEvent playerEvent)
        {
            Level = UnluckyFourArtifact.PROC_LIMIT;
        }

        public int GetMaxCounter()
        {
            return UnluckyFourArtifact.PROC_LIMIT;
        }

        public int GetCurrentCounter()
        {
            return UnluckyFourArtifact.PROC_LIMIT - Level;
        }
    }
}