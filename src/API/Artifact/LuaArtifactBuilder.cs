using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class LuaArtifactBuilder
    {
        private string name;
        private Rarity rarity;

        // --- Getters ---
        private Func<Tile, Player, Artifact, bool> shouldHighlightTile;
        private Func<Player, Artifact, bool> isAvailableGlobally;
        private Func<Player, Func<string, string>, Artifact, string> getDescription;
        private Func<Player, Func<string, string>, Artifact, string> getInShopDescription;
        private Func<Player, Artifact, (string, double)> getAdditionalInfo;
        private Func<Player, Artifact, string> getNameWithColor;
        private Func<Player, Artifact, string> getName;
        private Func<Player, Artifact, string> getSubHeader;
        private Func<Player, Artifact, string> getSpriteID;

        // --- Events ---
        private Action<Player, Artifact> onObtain;
        private Action<Player, Artifact> onRemoved;
        private Action<Player, Artifact> preGameInitialized;
        private Action<Player, Artifact> resetArtifactState;
        private Action<Player, Permutation, Tile, List<Effect>, Artifact> onTileEffect;
        private Action<Player, Permutation, Tile, List<Effect>, Artifact> onTilePostEffect;
        private Action<Player, Tile, List<IAnimationEffect>, bool, bool, Artifact> onDiscardTileEffect;
        private Action<Player, Permutation, Tile, List<Effect>, Artifact> onUnusedTileEffect;
        private Action<Player, Permutation, Block, List<Effect>, Artifact> onBlockEffect;
        private Action<Player, Permutation, Block, List<IAnimationEffect>, Artifact> onBlockAnimEffect;
        private Action<Player, Permutation, List<Effect>, Artifact> onSelfEffect;
        private Action<Player, Permutation, List<IAnimationEffect>, Artifact> onRoundEndEffect;
        private Action<Player, Artifact> onSubscribeToPlayer;
        private Action<Player, Artifact> onUnsubscribeToPlayer;
        
        private List<string> deckIn = new List<string>();
        private List<string> deckBlocked = new List<string>();
        private List<string> setIn = new List<string>();
        private List<string> setBlocked = new List<string>();
        private List<string> materialRequired = new List<string>();

        private LuaArtifactBuilder() { }

        public static LuaArtifactBuilder Create(string name, Rarity rarity)
        {
            return new LuaArtifactBuilder { name = name, rarity = rarity };
        }

        // --- Chainable Setters ---
        public LuaArtifactBuilder WithHighlight(Func<Tile, Player, Artifact, bool> f) { shouldHighlightTile = f; return this; }
        public LuaArtifactBuilder WithAvailability(Func<Player, Artifact, bool> f) { isAvailableGlobally = f; return this; }
        public LuaArtifactBuilder WithDescription(Func<Player, Func<string, string>, Artifact, string> f) { getDescription = f; return this; }
        public LuaArtifactBuilder WithInShopDescription(Func<Player, Func<string, string>, Artifact, string> f) { getInShopDescription = f; return this; }
        public LuaArtifactBuilder WithAdditionalInfo(Func<Player, Artifact, (string, double)> f) { getAdditionalInfo = f; return this; }
        public LuaArtifactBuilder WithNameWithColor(Func<Player, Artifact, string> f) { getNameWithColor = f; return this; }
        public LuaArtifactBuilder WithName(Func<Player, Artifact, string> f) { getName = f; return this; }
        public LuaArtifactBuilder WithSubHeader(Func<Player, Artifact, string> f) { getSubHeader = f; return this; }
        public LuaArtifactBuilder WithSpriteID(Func<Player, Artifact, string> f) { getSpriteID = f; return this; }

        public LuaArtifactBuilder OnObtain(Action<Player, Artifact> f) { onObtain = f; return this; }
        public LuaArtifactBuilder OnRemoved(Action<Player, Artifact> f) { onRemoved = f; return this; }
        public LuaArtifactBuilder PreGameInitialized(Action<Player, Artifact> f) { preGameInitialized = f; return this; }
        public LuaArtifactBuilder ResetArtifactState(Action<Player, Artifact> f) { resetArtifactState = f; return this; }

        public LuaArtifactBuilder OnTileEffect(Action<Player, Permutation, Tile, List<Effect>, Artifact> f) { onTileEffect = f; return this; }
        public LuaArtifactBuilder OnTilePostEffect(Action<Player, Permutation, Tile, List<Effect>, Artifact> f) { onTilePostEffect = f; return this; }
        public LuaArtifactBuilder OnDiscardTileEffect(Action<Player, Tile, List<IAnimationEffect>, bool, bool, Artifact> f) { onDiscardTileEffect = f; return this; }
        public LuaArtifactBuilder OnUnusedTileEffect(Action<Player, Permutation, Tile, List<Effect>, Artifact> f) { onUnusedTileEffect = f; return this; }
        public LuaArtifactBuilder OnBlockEffect(Action<Player, Permutation, Block, List<Effect>, Artifact> f) { onBlockEffect = f; return this; }
        public LuaArtifactBuilder OnBlockAnimEffect(Action<Player, Permutation, Block, List<IAnimationEffect>, Artifact> f) { onBlockAnimEffect = f; return this; }
        public LuaArtifactBuilder OnSelfEffect(Action<Player, Permutation, List<Effect>, Artifact> f) { onSelfEffect = f; return this; }
        public LuaArtifactBuilder OnRoundEndEffect(Action<Player, Permutation, List<IAnimationEffect>, Artifact> f) { onRoundEndEffect = f; return this; }
        
        public LuaArtifactBuilder OnSubscribeToPlayer(Action<Player, Artifact> f) { onSubscribeToPlayer = f; return this; }
        public LuaArtifactBuilder OnUnsubscribeToPlayer(Action<Player, Artifact> f) { onUnsubscribeToPlayer = f; return this; }
        
        public LuaArtifactBuilder WithDeckIn(string[] ids) { deckIn = ids.ToList(); return this; }
        public LuaArtifactBuilder WithDeckBlocked(string[] ids) { deckBlocked = ids.ToList(); return this; }
        public LuaArtifactBuilder WithSetIn(string[] ids) { setIn = ids.ToList(); return this; }
        public LuaArtifactBuilder WithSetBlocked(string[] ids) { setBlocked = ids.ToList(); return this; }
        public LuaArtifactBuilder WithMaterialRequired(string[] ids) { materialRequired = ids.ToList(); return this; }

        // --- Build ---
        public LuaArtifact Build()
        {
            LuaArtifact artifact = null;
            artifact = new LuaArtifact(
                name,
                rarity,
                shouldHighlightTile,
                isAvailableGlobally,
                getDescription,
                getInShopDescription,
                getAdditionalInfo,
                getNameWithColor,
                getName,
                getSubHeader,
                onObtain,
                onRemoved,
                preGameInitialized,
                resetArtifactState,
                onTileEffect,
                onTilePostEffect,
                onDiscardTileEffect,
                onUnusedTileEffect,
                onBlockEffect,
                onBlockAnimEffect,
                onSelfEffect,
                onRoundEndEffect,
                getSpriteID,
                onSubscribeToPlayer,
                onUnsubscribeToPlayer
            )
            {
                deckIn = deckIn,
                deckBlocked = deckBlocked,
                setIn = setIn,
                setBlocked = setBlocked,
                materialRequired = materialRequired
            };

            return artifact;
        }

        public LuaCraftableArtifact BuildCraftable()
        {
            LuaCraftableArtifact artifact = null;
            artifact = new LuaCraftableArtifact(
                name,
                rarity,
                shouldHighlightTile,
                isAvailableGlobally,
                getDescription,
                getInShopDescription,
                getAdditionalInfo,
                getNameWithColor,
                getName,
                getSubHeader,
                onObtain,
                onRemoved,
                preGameInitialized,
                resetArtifactState,
                onTileEffect,
                onTilePostEffect,
                onDiscardTileEffect,
                onUnusedTileEffect,
                onBlockEffect,
                onBlockAnimEffect,
                onSelfEffect,
                onRoundEndEffect,
                getSpriteID,
                onSubscribeToPlayer,
                onUnsubscribeToPlayer
            )
            {
                deckIn = deckIn,
                deckBlocked = deckBlocked,
                setIn = setIn,
                setBlocked = setBlocked,
                materialRequired = materialRequired
            };

            return artifact;
        }

        public LuaCraftableArtifact BuildAndRegisterCraftable()
        {
            LuaCraftableArtifact artifact = BuildCraftable();
            Artifacts.ArtifactList.Add(artifact);
            return artifact;
        }
        
        public LuaArtifact BuildAndRegister()
        {
            LuaArtifact artifact = Build();
            Artifacts.ArtifactList.Add(artifact);
            return artifact;
        }
    }
}
