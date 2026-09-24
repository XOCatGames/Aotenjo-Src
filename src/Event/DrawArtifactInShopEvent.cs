using System.Collections.Generic;

namespace Aotenjo
{
    public class DrawArtifactInShopEvent : PlayerEvent
    {
        public readonly List<Artifact> artifacts;

        public DrawArtifactInShopEvent(Player player, List<Artifact> artifacts) : base(player)
        {
            this.artifacts = artifacts;
        }

        public class On : DrawArtifactInShopEvent
        {
            public On(Player player, List<Artifact> artifacts) : base(player, artifacts)
            {
            }
        }

        public class Post : DrawArtifactInShopEvent
        {
            public Post(Player player, List<Artifact> artifacts) : base(player, artifacts)
            {
            }
        }
    }
}