using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class LuaTileMaterialBuilder
    {
        private int id;
        private string nameKey;
        private Rarity rarity = Rarity.COMMON;
        private SerializableMap<string, string> _data = new();

        // --- delegates ---
        private Func<TileMaterial, bool> isDebuff;
        private Action<Player, Permutation, Tile, List<Effect>, TileMaterial> onScoringEffects;
        private Action<Player, Permutation, List<Effect>, TileMaterial> onUnusedEffects;
        private Action<Player, Permutation, List<Effect>, Tile, Tile, TileMaterial> onDerivedTileUnusedEffects;
        private Action<Player, Permutation, List<IAnimationEffect>, Tile, TileMaterial> onRoundEndEffects;
        private Action<Player, Permutation, List<IAnimationEffect>, Tile, bool, bool, TileMaterial> onDiscardEffects;

        private Func<Func<string, string>, Player, TileMaterial, string> getDescription;
        private Action<Player, TileMaterial> onSubscribeToPlayer;
        private Action<Player, TileMaterial> onUnsubscribeToPlayer;

        private LuaTileMaterialBuilder() { }

        public static LuaTileMaterialBuilder Create(string nameKey)
        {
            return new LuaTileMaterialBuilder { id = 0, nameKey = nameKey };
        }

        // --- Chainable setters ---
        public LuaTileMaterialBuilder WithRarity(Rarity rarity) { this.rarity = rarity; return this; }
        public LuaTileMaterialBuilder WithData(string key, string value) { _data[key] = value; return this; }
        public LuaTileMaterialBuilder WithDebuff(Func<TileMaterial, bool> f) { isDebuff = f; return this; }

        public LuaTileMaterialBuilder OnScoringEffect(Action<Player, Permutation, Tile, List<Effect>, TileMaterial> f) { onScoringEffects = f; return this; }
        public LuaTileMaterialBuilder OnUnusedEffect(Action<Player, Permutation, List<Effect>, TileMaterial> f) { onUnusedEffects = f; return this; }
        public LuaTileMaterialBuilder OnDerivedTileUnusedEffect(Action<Player, Permutation, List<Effect>, Tile, Tile, TileMaterial> f) { onDerivedTileUnusedEffects = f; return this; }
        public LuaTileMaterialBuilder OnRoundEndEffect(Action<Player, Permutation, List<IAnimationEffect>, Tile, TileMaterial> f) { onRoundEndEffects = f; return this; }
        public LuaTileMaterialBuilder OnDiscardEffect(Action<Player, Permutation, List<IAnimationEffect>, Tile, bool, bool, TileMaterial> f) { onDiscardEffects = f; return this; }

        public LuaTileMaterialBuilder WithDescription(Func<Func<string, string>, Player, TileMaterial, string> f) { getDescription = f; return this; }
        public LuaTileMaterialBuilder OnSubscribe(Action<Player, TileMaterial> f) { onSubscribeToPlayer = f; return this; }
        public LuaTileMaterialBuilder OnUnsubscribe(Action<Player, TileMaterial> f) { onUnsubscribeToPlayer = f; return this; }

        // --- Build ---
        public LuaTileMaterial Build()
        {
            var material = new LuaTileMaterial(id, nameKey)
            {
                rarity = rarity,
                data = _data.Clone(),

                isDebuff = isDebuff,

                onScoringEffects = onScoringEffects,
                onUnusedEffects = onUnusedEffects,
                onDerivedTileUnusedEffects = onDerivedTileUnusedEffects,
                onRoundEndEffects = onRoundEndEffects,
                onDiscardEffects = onDiscardEffects,

                getDescription = getDescription,
                onSubscribeToPlayer = onSubscribeToPlayer,
                onUnsubscribeToPlayer = onUnsubscribeToPlayer
            };

            return material;
        }

        public LuaTileMaterial BuildAndRegister()
        {
            var material = Build();

            // 注册到全局 MaterialProviders
            TileMaterial.MaterialProviders = TileMaterial.MaterialProviders
                .Concat(new Func<TileMaterial>[] { Build })
                .ToArray();

            return material;
        }
    }
}
