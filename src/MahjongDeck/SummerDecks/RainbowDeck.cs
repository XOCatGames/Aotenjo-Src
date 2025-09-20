using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class RainbowDeck : MahjongDeck
    {
        public RainbowDeck() : base("rainbow_deck", "rainbow", "rainbow", "intermediate", 4)
        {
        }

        public override bool IsUnlocked(PlayerStats stats)
        {
            return Constants.DEBUG_MODE || stats.GetUnseededRunRecords().Any(rec => rec.acsensionLevel >= 1 && rec.won)
                || stats.GetCustomStats(PlayerStatsType.WUMENQI_WIN) > 0;
        }

        public override Player CreateNewPlayer(string seed, MaterialSet set, int ascension)
        {
            Player player = new RainbowPlayer(seed, this, set, ascension);
            return player;
        }

        public class RainbowPlayer : Player
        {
            public List<FlowerTile> PlayedFlowerTiles;

            public delegate void PlayFlowerTileEvent(Player player, FlowerTile flowerTile);

            public event PlayFlowerTileEvent PrePlayFlowerTileEvent;
            public event PlayFlowerTileEvent PostPlayFlowerTileEvent;

            public RainbowPlayer(string seed, MahjongDeck deck, MaterialSet set, int ascension) : base(
                Hand.PlainFullHand().tiles.Union(new Hand("1234f").tiles).ToList(), seed,
                deck, PlayerProperties.DEFAULT, 0, SkillSet.RainbowSkillSet(), set, ascension)
            {
                PlayedFlowerTiles = new List<FlowerTile>();
            }

            /// <summary>
            /// 打出一枚花牌并补充一枚手牌
            /// </summary>
            /// <param name="flower">打出的花牌</param>
            /// <returns>补充的手牌，如果牌库没有牌或是打出的牌不在手牌内则返回Null</returns>
            public Tile PlayFlowerTile(FlowerTile flower)
            {
                PrePlayFlowerTileEvent?.Invoke(this, flower);

                HandDeck.Remove(flower);
                PlayedFlowerTiles.Add(flower);
                flower.OnPlayed(this, GetAccumulatedPermutation());
                int pos = DrawTileToHandDeck();

                PostPlayFlowerTileEvent?.Invoke(this, flower);

                if (pos == -1) return null;
                return HandDeck[pos];
            }

            public bool CanPlayFlowerTile(FlowerTile tile)
            {
                return PlayedFlowerTiles.Count < GetMaxFlowerTileCount() && HandDeck.Contains(tile);
            }

            public override List<Tile> GetAllTiles()
            {
                return base.GetAllTiles().Union(PlayedFlowerTiles).ToList();
            }

            public int GetMaxFlowerTileCount()
            {
                return 8;
            }

            public override void ResetTilePool()
            {
                base.ResetTilePool();
                TilePool.AddRange(PlayedFlowerTiles);
                PlayedFlowerTiles.Clear();
            }

            protected override void AddExtraScoringEffects(List<IAnimationEffect> queue)
            {
                Permutation perm = GetCurrentSelectedPerm();
                foreach (FlowerTile flower in PlayedFlowerTiles)
                {
                    flower.AppendScoringEffect(queue, this, perm);
                    List<Effect> artifactEffects = new();
                    foreach (Artifact artifact in GetArtifacts())
                    {
                        artifact.AppendOnTileEffects(this, perm, flower, artifactEffects);
                    }

                    queue.AddRange(artifactEffects.Select(e => new OnTileAnimationEffect(flower, e)));
                }
            }

            protected override void AppendOnTileRoundEndEffect(List<IAnimationEffect> onRoundEndEffects)
            {
                base.AppendOnTileRoundEndEffect(onRoundEndEffects);
                foreach (FlowerTile flower in PlayedFlowerTiles)
                {
                    flower.AppendRoundEndEffect(onRoundEndEffects, this, GetCurrentSelectedPerm());
                }
            }

            public override List<Destination> GenerateDestinations()
            {
                List<Destination> destinations = base.GenerateDestinations();
                if (Level % 4 == 1 && Level < 16)
                {
                    if (GenerateRandomInt(3) <= 1)
                    {
                        destinations[0] = new AddFlowerTileDestination(this);
                    }
                }

                return destinations;
            }
        }
    }
}