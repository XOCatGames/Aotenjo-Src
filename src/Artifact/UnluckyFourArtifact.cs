using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class UnluckyFourArtifact : LevelingArtifact, ICountable
    {
        public const int PROC_LIMIT = 4; // 25% chance to trigger
        private static readonly int CHANCE = 4;

        public UnluckyFourArtifact() : base("unlucky_four", Rarity.COMMON, PROC_LIMIT)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(player, localizer), Level, PROC_LIMIT,
                GetChanceMultiplier(player));
        }

        public override void AddDiscardTileEffects(Player player, Tile tile,
            List<IAnimationEffect> onDiscardTileEffects, bool withForce, bool isClone)
        {
            base.AddDiscardTileEffects(player, tile, onDiscardTileEffects, withForce, isClone);

            if (tile.IsNumbered() && tile.GetOrder() == 4 && player.GenerateRandomDeterminationResult(CHANCE) &&
                Level > 0)
            {
                onDiscardTileEffects.Add(new UnluckyFourEffect(this, tile));
            }
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);

            player.PreRoundStartEvent += OnRoundStart;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);

            player.PreRoundStartEvent += OnRoundStart;
        }

        public void OnRoundStart(PlayerEvent playerEvent)
        {
            Level = PROC_LIMIT;
        }

        public int GetMaxCounter()
        {
            return PROC_LIMIT;
        }

        public int GetCurrentCounter()
        {
            return PROC_LIMIT - Level;
        }
    }

    public class UnluckyFourEffect : Effect
    {
        private readonly LevelingArtifact artifact;
        private readonly Tile tile;

        public UnluckyFourEffect(LevelingArtifact artifact, Tile tile)
        {
            this.artifact = artifact;
            this.tile = tile;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func("effect_unlucky_four");
        }

        public override Artifact GetEffectSource()
        {
            return artifact;
        }

        public override void Ingest(Player player)
        {
            if (artifact.Level > 0)
            {
                player.DiscardLeft += 4;
                tile.addonFu += 4;
                artifact.Level--;
            }
        }

        public override string GetSoundEffectName()
        {
            return "GrowFu";
        }
    }
}