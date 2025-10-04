using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class InkBottleArtifact : LevelingArtifact, IFuProvider
    {
        private const int BASE = 0;
        private const int LEVEL_INCREMENT = 8;

        public InkBottleArtifact() : base("ink_bottle", Rarity.RARE, BASE)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), GetFu(player), LEVEL_INCREMENT);
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToAddFuFormat(GetFu(player));
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            if (Level == 0) return;
            effects.Add(ScoreEffect.AddFu(() => GetFu(player), this));
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);

            if (!player.DetermineFontCompactbility(tile, TileFont.PLAIN) &&
                !player.DetermineFontCompactbility(tile, TileFont.COLORLESS))
            {
                effects.Add(new ColorlessEffect(tile, this));
            }
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PreSetFontEvent += OnSetFont;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PreSetFontEvent -= OnSetFont;
        }

        private void OnSetFont(PlayerSetAttributeEvent evt)
        {
            if (evt.attributeToReceive.GetRegName() != TileFont.COLORLESS.GetRegName()) return;
            Level++;
        }

        private class ColorlessEffect : Effect
        {
            private Tile tile;
            private InkBottleArtifact inkBottleArtifact;

            public ColorlessEffect(Tile tile, InkBottleArtifact inkBottleArtifact)
            {
                this.tile = tile;
                this.inkBottleArtifact = inkBottleArtifact;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_decolorize");
            }

            public override Artifact GetEffectSource()
            {
                return inkBottleArtifact;
            }

            public override void Ingest(Player player)
            {
                tile.SetFont(TileFont.COLORLESS, player);
            }
        }

        public double GetFu(Player player)
        {
            return LEVEL_INCREMENT * Level;
        }
    }
}