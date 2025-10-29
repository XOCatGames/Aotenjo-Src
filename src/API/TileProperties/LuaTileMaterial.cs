using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class LuaTileMaterial : TileMaterial
    {
        public SerializableMap<string, string> data = new SerializableMap<string, string>();
        public Rarity rarity;
        public Func<TileMaterial, bool> isDebuff;

        [NonSerialized] public Action<Player, Permutation, Tile, List<Effect>, TileMaterial> onScoringEffects;
        [NonSerialized] public Action<Player, Permutation, List<Effect>, TileMaterial> onUnusedEffects;
        [NonSerialized] public Action<Player, Permutation, List<Effect>, Tile, Tile, TileMaterial> onDerivedTileUnusedEffects;
        [NonSerialized] public Action<Player, Permutation, List<IAnimationEffect>, Tile, TileMaterial> onRoundEndEffects;
        [NonSerialized] public Action<Player, Permutation, List<IAnimationEffect>, Tile, bool, bool, TileMaterial> onDiscardEffects;
        
        
        [NonSerialized] public Func<Func<string, string>, Player, TileMaterial, string> getDescription;
        [NonSerialized] public Action<Player, TileMaterial> onSubscribeToPlayer;
        [NonSerialized] public Action<Player, TileMaterial> onUnsubscribeToPlayer;
        
        public override TileMaterial Copy()
        {
            var tileMaterial = GetMaterial(GetRegName());
            if (tileMaterial is LuaTileMaterial luaMaterial)
            {
                luaMaterial.data = data.Clone();
            }
            return tileMaterial;
        }

        public override Rarity GetRarity()
        {
            return rarity;
        }

        protected override string GetSpriteNamespaceID(Player player, string nmSpace = "aotenjo")
        {
            return $"tile_material:{GetRegName().Replace("_material", "")}";
        }

        public override bool IsDebuff()
        {
            return isDebuff?.Invoke(this) ?? base.IsDebuff();
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            onScoringEffects?.Invoke(player, perm, tile, effects, this);
        }

        public override void AppendUnusedEffects(Player player, Permutation perm, List<Effect> effects)
        {
            base.AppendUnusedEffects(player, perm, effects);
            onUnusedEffects?.Invoke(player, perm, effects, this);
        }

        public override void AppendToListOnTileUnusedEffect(Player player, Permutation perm, List<Effect> effects, Tile scoringTile, Tile onEffectTile)
        {
            base.AppendToListOnTileUnusedEffect(player, perm, effects, scoringTile, onEffectTile);
            onDerivedTileUnusedEffects?.Invoke(player, perm, effects, scoringTile, onEffectTile, this);
        }

        public override void AppendToListRoundEndEffect(Player player, Permutation perm, List<IAnimationEffect> effects, Tile tile)
        {
            base.AppendToListRoundEndEffect(player, perm, effects, tile);
            onRoundEndEffects?.Invoke(player, perm, effects, tile, this);
        }

        public override void AppendToListDiscardEffect(Player player, Permutation perm, List<IAnimationEffect> effects, Tile tile, bool withForce, bool isClone)
        {
            base.AppendToListDiscardEffect(player, perm, effects, tile, withForce, isClone);
            onDiscardEffects?.Invoke(player, perm, effects, tile, withForce, isClone, this);
        }

        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            return getDescription?.Invoke(localizer, player, this) ?? base.GetDescription(localizer, player);
        }

        public override void SubscribeToPlayerEvents(Player player)
        {
            base.SubscribeToPlayerEvents(player);
            onSubscribeToPlayer?.Invoke(player, this);
        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            onUnsubscribeToPlayer?.Invoke(player, this);
        }

        public LuaTileMaterial(int ID, string nameKey) : base(ID, nameKey, null)
        {
        }
        
        public string GetDataOrDefault(string key, string defaultValue)
        {
            return data.Get(key) ?? defaultValue;
        }
        
        public void SetData(string key, string value)
        {
            data[key] = value;
        }
    }
}