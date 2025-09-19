using System;

namespace Aotenjo
{
    public class ClayBallArtifact : Artifact
    {
        private const int ADDON_FU_AMOUNT = 5;

        public ClayBallArtifact() : base("clay_ball", Rarity.RARE)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), ADDON_FU_AMOUNT);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PreSetTransformEvent += OnSetTransform;
            player.PostModifyTileCarvatureEvent += OnModifyFace;
            player.PreSetFontEvent += OnSetFont;
        }

        private void OnSetFont(PlayerSetAttributeEvent evt)
        {
            if (!evt.isCopy)
            {
                evt.tile.addonFu += ADDON_FU_AMOUNT;
            }
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PreSetTransformEvent -= OnSetTransform;
            player.PostModifyTileCarvatureEvent -= OnModifyFace;
            player.PreSetFontEvent -= OnSetFont;
        }

        private void OnModifyFace(Player _, Tile tile, Tile.Category cat, int ord)
        {
            tile.addonFu += ADDON_FU_AMOUNT;
        }

        private void OnSetTransform(PlayerSetTransformEvent evt)
        {
            if (evt.transform != null)
            {
                evt.tile.addonFu += ADDON_FU_AMOUNT;
            }
        }
    }
}