using System;
using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>注册模组遗物词条；回调中的 artifact 始终是承载词条的遗物。</summary>
    public sealed class LuaArtifactAffixBuilder
    {
        private readonly CustomArtifactAffixDefinition definition = new();

        private LuaArtifactAffixBuilder(string id) { definition.Id = id; }
        public static LuaArtifactAffixBuilder Create(string id) => new(id);
        public LuaArtifactAffixBuilder WithNameKey(string key) { definition.NameKey = key; return this; }
        public LuaArtifactAffixBuilder WithDescriptionKey(string key) { definition.DescriptionKey = key; return this; }
        public LuaArtifactAffixBuilder WithDescription(Func<Player, Func<string, string>, Artifact, string> callback)
        { definition.Description = callback; return this; }
        public LuaArtifactAffixBuilder WithCanApply(Func<Player, Artifact, bool> callback)
        { definition.CanApply = callback; return this; }
        public LuaArtifactAffixBuilder OnSelfEffect(Action<Player, Permutation, List<Effect>, Artifact> callback)
        { definition.OnSelf = callback; return this; }
        public LuaArtifactAffixBuilder OnTileEffect(Action<Player, Permutation, Tile, List<Effect>, Artifact> callback)
        { definition.OnTile = callback; return this; }
        public LuaArtifactAffixBuilder OnBlockEffect(Action<Player, Permutation, Block, List<Effect>, Artifact> callback)
        { definition.OnBlock = callback; return this; }
        public LuaArtifactAffixBuilder OnRoundEndEffect(Action<Player, Permutation, List<IAnimationEffect>, Artifact> callback)
        { definition.OnRoundEnd = callback; return this; }

        // 注册时复制定义，后续修改 Builder 不会改变已注册词条；重复 ID 不覆盖原模组。
        public bool BuildAndRegister() => CustomArtifactAffixes.Register(definition.Copy());
    }
}
