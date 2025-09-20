using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileProperties
    {
        [SerializeReference] public TileMaterial material;
        [SerializeReference] public TileFont font;
        [SerializeReference] public TileMask mask;

        private TileProperties() : this(TileMaterial.PLAIN, TileFont.PLAIN)
        {
        }

        private TileProperties(TileMaterial material, TileFont font) : this(material, font, TileMask.NONE)
        {
        }

        private TileProperties(TileMaterial material, TileFont font, TileMask mask)
        {
            this.material = material;
            this.font = font;
            this.mask = mask;
        }

        public TileProperties(TileProperties properties)
        {
            material = properties.material.Copy();
            font = properties.font.Copy();
            mask = properties.mask.Copy();
        }

        public TileProperties ChangeFont(TileFont font)
        {
            this.font = font;
            return this;
        }

        public TileProperties ChangeMaterial(TileMaterial material)
        {
            this.material = material;
            return this;
        }

        public TileProperties ChangeMask(TileMask mask)
        {
            this.mask = mask;
            return this;
        }

        public TileProperties CopyWithFont(TileFont newFont)
        {
            return new TileProperties(material, newFont, mask);
        }

        public TileProperties CopyWithMaterial(TileMaterial newMaterial)
        {
            return new TileProperties(newMaterial, font, mask);
        }

        public TileProperties CopyWithMask(TileMask newMask)
        {
            return new TileProperties(material, font, newMask);
        }

        public void AppendBonusEffects(List<Effect> effects, Permutation permutation, Player player, Tile tile)
        {
            material.AppendBonusEffects(player, permutation, tile, effects);
            font.AppendBonusEffects(player, permutation, tile, effects);
            mask.AppendBonusEffects(player, permutation, tile, effects);
        }

        public void AppendUnusedEffects(Player player, Permutation perm, List<Effect> effects)
        {
            material.AppendUnusedEffects(player, perm, effects);
            font.AppendUnusedEffects(player, perm, effects);
            mask.AppendUnusedEffects(player, perm, effects);
        }

        public void AppendToListOnTileUnusedEffect(Player player, Permutation perm, List<Effect> effects, Tile tile,
            Tile onTile)
        {
            material.AppendToListOnTileUnusedEffect(player, perm, effects, tile, onTile);
            font.AppendToListOnTileUnusedEffect(player, perm, effects, tile, onTile);
            mask.AppendToListOnTileUnusedEffect(player, perm, effects, tile, onTile);
        }

        public virtual void AppendToListRoundEndEffect(Player player, Permutation perm, List<IAnimationEffect> effects,
            Tile tile)
        {
            material.AppendToListRoundEndEffect(player, perm, effects, tile);
            font.AppendToListRoundEndEffect(player, perm, effects, tile);
            mask.AppendToListRoundEndEffect(player, perm, effects, tile);
        }

        internal void AppendDiscardEffects(Player player, Permutation perm, List<IAnimationEffect> effects, Tile tile,
            bool withForce, bool isClone)
        {
            material.AppendToListDiscardEffect(player, perm, effects, tile, withForce, isClone);
            font.AppendToListDiscardEffect(player, perm, effects, tile, withForce, isClone);
            mask.AppendToListDiscardEffect(player, perm, effects, tile, withForce, isClone);
        }

        public void SubscribeToPlayer(Player player)
        {
            material.SubscribeToPlayerEvents(player);
            font.SubscribeToPlayerEvents(player);
            mask.SubscribeToPlayerEvents(player);
        }

        public void UnsubcribeFromPlayer(Player player)
        {
            material.UnsubscribeToPlayerEvents(player);
            font.UnsubscribeToPlayerEvents(player);
            mask.UnsubscribeToPlayerEvents(player);
        }

        public string GetName(Func<string, string> localizer, Player player)
        {
            var maskName = (mask.GetRegName() == TileMask.NONE.GetRegName()) ? "" : localizer(mask.GetLocalizeKey()) + " ";
            var fontName = (font.GetRegName() == TileFont.PLAIN.GetRegName()) ? "" : localizer(font.GetLocalizeKey()) + " ";
            var matName = localizer(material.GetLocalizeKey());
            return maskName + fontName + matName;
        }

        public static TileProperties Plain()
        {
            return new TileProperties();
        }

        public string GetDescription(Player player, Func<string, string> localizer)
        {
            TileProperties properties = this;
            TileMask propertiesMask = properties.mask;
            string maskName = localizer(propertiesMask.GetLocalizeKey());
            string maskDesc = properties.mask.GetDescription(localizer, player);

            string maskContent = properties.mask.GetRegName() == TileMask.NONE.GetRegName() ? "" : $"\n\n{maskName}\n{maskDesc}";

            TileFont propertiesFont = properties.font;
            string fontName = localizer(propertiesFont.GetLocalizeKey());
            string fontDesc = properties.font.GetDescription(localizer, player);

            string fontContent = propertiesFont.GetRegName() == TileFont.PLAIN.GetRegName() ? "" : $"\n\n{fontName}\n{fontDesc}";

            TileMaterial propertiesMaterial = properties.material;
            string materialName = localizer(propertiesMaterial.GetLocalizeKey());
            string materialDesc = properties.material.GetDescription(localizer, player);

            string materialContent = propertiesMaterial.GetRegName() == TileMaterial.PLAIN.GetRegName() ? "" : $"\n\n{materialName}\n{materialDesc}";

            return $"{maskContent}{fontContent}{materialContent}";
        }

        public bool IsDebuffed()
        {
            return mask.IsDebuff() || font.IsDebuff() || material.IsDebuff();
        }
    }
}