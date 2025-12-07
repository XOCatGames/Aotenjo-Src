using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using XLua;

namespace Aotenjo
{
    public class LuaCraftableArtifact : CraftableArtifact
    {
        private Dictionary<string, string> luaData = new();
        
        // 所有方法的可选 delegate
        private readonly Func<Tile, Player, Artifact, bool> shouldHighlightTile;
        private readonly Func<Player, Artifact, bool> isAvailableGlobally;
        private readonly Func<Player, Func<string, string>, Artifact, string> getDescription;
        private readonly Func<Player, Func<string, string>, Artifact, string> getInShopDescription;
        private readonly Func<Player, Artifact, (string, double)> getAdditionalInfo;
        private readonly Func<Player, Artifact, string> getNameWithColor;
        private readonly Func<Player, Artifact, string> getName;
        private readonly Func<Player, Artifact, string> getSubHeader;

        private readonly Action<Player, Artifact> onObtain;
        private readonly Action<Player, Artifact> onRemoved;
        private readonly Action<Player, Artifact> preGameInitialized;
        private readonly Action<Player, Artifact> resetArtifactState;

        private readonly Action<Player, Permutation, Tile, List<Effect>, Artifact> onTileEffect;
        private readonly Action<Player, Permutation, Tile, List<Effect>, Artifact> onTilePostEffect;
        private readonly Action<Player, Permutation, Tile, List<Effect>, Artifact> onUnusedTileEffect;
        private readonly Action<Player, Tile, List<IAnimationEffect>, bool, bool, Artifact> onDiscardTileEffect;

        private readonly Action<Player, Permutation, Block, List<Effect>, Artifact> onBlockEffect;
        private readonly Action<Player, Permutation, Block, List<IAnimationEffect>, Artifact> onBlockAnimEffect;

        private readonly Action<Player, Permutation, List<Effect>, Artifact> onSelfEffect;
        private readonly Action<Player, Permutation, List<IAnimationEffect>, Artifact> onRoundEndEffect;

        private readonly Func<Player, Artifact, string> getSpriteId;
        
        private readonly Action<Player, Artifact> onSubscribeToPlayer;
        private readonly Action<Player, Artifact> onUnsubscribeToPlayer;
        
        public string GetDataOrDefault(string key, string defaultValue)
        {
            return luaData.GetValueOrDefault(key, defaultValue);
        }
        
        public void SetData(string key, string value)
        {
            luaData[key] = value;
        }

        public override void Deserialize(string data)
        {
            luaData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
        }
        
        public override string Serialize()
        {
            return JsonConvert.SerializeObject(luaData);
        }

        public LuaCraftableArtifact(
            string name,
            Rarity rarity,
            // delegates ...
            Func<Tile, Player, Artifact, bool> shouldHighlightTile = null,
            Func<Player, Artifact, bool> isAvailableGlobally = null,
            Func<Player, Func<string, string>, Artifact, string> getDescription = null,
            Func<Player, Func<string, string>, Artifact, string> getInShopDescription = null,
            Func<Player, Artifact, (string, double)> getAdditionalInfo = null,
            Func<Player, Artifact, string> getNameWithColor = null,
            Func<Player, Artifact, string> getName = null,
            Func<Player, Artifact, string> getSubHeader = null,
            Action<Player, Artifact> onObtain = null,
            Action<Player, Artifact> onRemoved = null,
            Action<Player, Artifact> preGameInitialized = null,
            Action<Player, Artifact> resetArtifactState = null,
            Action<Player, Permutation, Tile, List<Effect>, Artifact> onTileEffect = null,
            Action<Player, Permutation, Tile, List<Effect>, Artifact> onTilePostEffect = null,
            Action<Player, Tile, List<IAnimationEffect>, bool, bool, Artifact> onDiscardTileEffect = null,
            Action<Player, Permutation, Tile, List<Effect>, Artifact> onUnusedTileEffect = null,
            Action<Player, Permutation, Block, List<Effect>, Artifact> onBlockEffect = null,
            Action<Player, Permutation, Block, List<IAnimationEffect>, Artifact> onBlockAnimEffect = null,
            Action<Player, Permutation, List<Effect>, Artifact> onSelfEffect = null,
            Action<Player, Permutation, List<IAnimationEffect>, Artifact> onRoundEndEffect = null,
            Func<Player, Artifact, string> getSpriteID = null,
            Action<Player, Artifact> onSubscribeToPlayer = null,
            Action<Player, Artifact> onUnsubscribeToPlayer = null
        ) : base(name, rarity)
        {
            this.shouldHighlightTile = shouldHighlightTile;
            this.isAvailableGlobally = isAvailableGlobally;
            this.getDescription = getDescription;
            this.getInShopDescription = getInShopDescription;
            this.getAdditionalInfo = getAdditionalInfo;
            this.getNameWithColor = getNameWithColor;
            this.getName = getName;
            this.getSubHeader = getSubHeader;
            this.onObtain = onObtain;
            this.onRemoved = onRemoved;
            this.preGameInitialized = preGameInitialized;
            this.resetArtifactState = resetArtifactState;
            this.onTileEffect = onTileEffect;
            this.onTilePostEffect = onTilePostEffect;
            this.onDiscardTileEffect = onDiscardTileEffect;
            this.onUnusedTileEffect = onUnusedTileEffect;
            this.onBlockEffect = onBlockEffect;
            this.onBlockAnimEffect = onBlockAnimEffect;
            this.onSelfEffect = onSelfEffect;
            this.onRoundEndEffect = onRoundEndEffect;
            this.getSpriteId = getSpriteID;
            this.onSubscribeToPlayer = onSubscribeToPlayer;
            this.onUnsubscribeToPlayer = onUnsubscribeToPlayer;
        }

        // ---- 重写所有虚方法 ----

        public override string GetSpriteNamespaceID(Player player, string nmSpace = "unknown_mod")
        {
            return getSpriteId?.Invoke(player, this) ?? $"artifact:{GetRegName()}";
        }

        public override bool ShouldHighlightTile(Tile tile, Player player) =>
            shouldHighlightTile?.Invoke(tile, player, this) ?? base.ShouldHighlightTile(tile, player);

        public override bool IsAvailableGlobally(Player player) =>
            isAvailableGlobally?.Invoke(player, this) ?? base.IsAvailableGlobally(player);

        public override string GetDescription(Func<string, string> localizer) =>
            getDescription?.Invoke(null, localizer, this) ?? base.GetDescription(localizer);

        public override string GetDescription(Player player, Func<string, string> localizer) =>
            getDescription?.Invoke(player, localizer, this) ?? base.GetDescription(player, localizer);

        public override string GetInShopDescription(Player player, Func<string, string> localizer) =>
            getInShopDescription?.Invoke(player, localizer, this) ?? base.GetInShopDescription(player, localizer);

        public override (string, double) GetAdditionalDisplayingInfo(Player player) =>
            getAdditionalInfo?.Invoke(player, this) ?? base.GetAdditionalDisplayingInfo(player);

        public override string GetNameWithColor(Func<string, string> localizer) =>
            getNameWithColor?.Invoke(null, this) ?? base.GetNameWithColor(localizer);

        public override string GetName(Func<string, string> localizer) =>
            getName?.Invoke(null, this) ?? base.GetName(localizer);

        public override string GetSubHeader(Player player, Func<string, string> loc) =>
            getSubHeader?.Invoke(player, this) ?? base.GetSubHeader(player, loc);

        public override void OnObtain(Player player)
        {
            base.OnObtain(player);
            if (onObtain != null) onObtain(player, this);
        }

        public override void OnRemoved(Player player)
        {
            base.OnRemoved(player);
            if (onRemoved != null) onRemoved(player, this);
        }

        public override void PreGameInitialized(Player player)
        {
            base.PreGameInitialized(player);
            if (preGameInitialized != null) preGameInitialized(player, this);
        }

        public override void ResetArtifactState(Player player)
        {
            base.ResetArtifactState(player);
            if (resetArtifactState != null) resetArtifactState(player, this);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (onTileEffect != null) onTileEffect(player, permutation, tile, effects, this);
        }

        public override void AddOnTileEffectsPostEvents(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AddOnTileEffectsPostEvents(player, permutation, tile, effects);
            if (onTilePostEffect != null) onTilePostEffect(player, permutation, tile, effects, this);
        }

        public override void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendOnUnusedTileEffects(player, perm, tile, effects);
            if (onUnusedTileEffect != null) onUnusedTileEffect(player, perm, tile, effects, this);
        }

        public override void AppendDiscardTileEffects(Player player, Tile tile, List<IAnimationEffect> onDiscardTileEffects, bool withForce, bool isClone)
        {
            base.AppendDiscardTileEffects(player, tile, onDiscardTileEffects, withForce, isClone);
            if (onDiscardTileEffect != null) onDiscardTileEffect(player, tile, onDiscardTileEffects, withForce, isClone, this);
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            base.AddOnBlockEffects(player, permutation, block, effects);
            if (onBlockEffect != null) onBlockEffect(player, permutation, block, effects, this);
        }

        public override void AppendPostBlockAnimationEffects(Player player, Permutation permutation, Block block, List<IAnimationEffect> effects)
        {
            base.AppendPostBlockAnimationEffects(player, permutation, block, effects);
            if (onBlockAnimEffect != null) onBlockAnimEffect(player, permutation, block, effects, this);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            if (onSelfEffect != null) onSelfEffect(player, permutation, effects, this);
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            if (onRoundEndEffect != null) onRoundEndEffect(player, permutation, effects, this);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            onSubscribeToPlayer?.Invoke(player, this);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            onUnsubscribeToPlayer?.Invoke(player, this);
        }
    }
}
