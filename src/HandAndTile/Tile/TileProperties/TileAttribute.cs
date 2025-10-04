using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public abstract class TileAttribute
    {
        [SerializeReference] protected Effect effect;

        [SerializeField] protected int spriteID;
        [SerializeField] protected string nameKey;

        public TileAttribute(int spriteID, string nameKey, Effect effect)
        {
            this.spriteID = spriteID;
            this.nameKey = nameKey;
            this.effect = effect;
        }


        public virtual bool IsDebuff()
        {
            return false;
        }

        public virtual void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            effects.AddRange(GetEffects(player, perm));
        }

        public virtual Effect[] GetEffects(Player player, Permutation permutation)
        {
            return GetEffects();
        }

        public virtual void AppendUnusedEffects(Player player, Permutation perm, List<Effect> effects)
        {
        }

        public virtual void AppendToListOnTileUnusedEffect(Player player, Permutation perm, List<Effect> effects,
            Tile scoringTile, Tile onEffectTile)
        {
        }

        public virtual void AppendToListRoundEndEffect(Player player, Permutation perm, List<IAnimationEffect> effects,
            Tile tile)
        {
        }

        public virtual void AppendToListDiscardEffect(Player player, Permutation perm, List<IAnimationEffect> effects,
            Tile tile, bool withForce, bool isClone)
        {
        }

        /// <returns>Effect[]的一份拷贝</returns>
        public virtual Effect[] GetEffects()
        {
            if (effect == null)
            {
                return new Effect[0];
            }

            return new[] { effect };
        }

        public virtual int GetSpriteID(Player player)
        {
            return GetSpriteID();
        }

        protected virtual int GetSpriteID()
        {
            return spriteID;
        }

        public virtual string GetLocalizeKey()
        {
            return "tile_" + nameKey + "_name";
        }

        public virtual string GetShortLocalizeKey()
        {
            return GetLocalizeKey();
        }

        public virtual string GetDescription(Func<string, string> localizer, Player player)
        {
            return GetDescription(localizer);
        }

        protected virtual string GetDescription(Func<string, string> localizer)
        {
            return localizer(GetDescriptionLocalizeKey());
        }

        protected virtual string GetDescriptionLocalizeKey()
        {
            return $"tile_{nameKey}_description";
        }

        public virtual string GetRegName()
        {
            return nameKey;
        }

        public virtual void SubscribeToPlayerEvents(Player player)
        {
            if (this is TileMaterial material)
            {
                player.stats.RecordObtainMaterial(material);
            }
        }

        public virtual void UnsubscribeToPlayerEvents(Player player)
        {
        }
    }
}