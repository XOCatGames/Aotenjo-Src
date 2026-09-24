using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        [Serializable]
        public class RainbowPlayer : Player, ISerializationCallbackReceiver
        {
            // 保留旧版 JSON 的真实字段名；引用列表保存派生类型与跨列表引用关系。
            [SerializeField] public List<FlowerTile> PlayedFlowerTiles;
            [SerializeReference] private List<FlowerTile> playedFlowerTiles;
            // JsonUtility 会把缺失的列表初始化为空，不能用 null 判断旧版存档。
            [SerializeField] private bool hasPlayedFlowerTileReferences;
            [SerializeReference] private List<FlowerTile> newPlayedFlowerTiles = new();

            public void OnBeforeSerialize()
            {
                playedFlowerTiles = PlayedFlowerTiles;
                hasPlayedFlowerTileReferences = true;
            }

            public void OnAfterDeserialize()
            {
                if (hasPlayedFlowerTileReferences)
                {
                    PlayedFlowerTiles = playedFlowerTiles ?? new List<FlowerTile>();
                }
                else
                {
                    // 保留旧 JSON 字段名，并按花色和序号恢复旧版按值保存的花牌类型。
                    PlayedFlowerTiles = (PlayedFlowerTiles ?? new List<FlowerTile>())
                        .Where(tile => tile != null).Select(tile => (FlowerTile)tile.Copy()).ToList();
                }
            }

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
                if (!CanPlayFlowerTile(flower)) return null;
                PrePlayFlowerTileEvent?.Invoke(this, flower);
                if (!CanPlayFlowerTile(flower)) return null;

                HandDeck.Remove(flower);
                PlayedFlowerTiles.Add(flower);
                newPlayedFlowerTiles ??= new List<FlowerTile>();
                newPlayedFlowerTiles.Add(flower);
                if (flower.properties.mask is not TileMaskSuppressed)
                    flower.OnPlayed(this, GetAccumulatedPermutation());
                int pos = DrawTileToHandDeck();

                PostPlayFlowerTileEvent?.Invoke(this, flower);

                if (pos == -1) return null;
                return HandDeck[pos];
            }

            public bool CanPlayFlowerTile(FlowerTile tile)
            {
                return tile != null && PlayedFlowerTiles.Count < GetMaxFlowerTileCount() &&
                       HandDeck.Contains(tile) && CanSelectTile(tile);
            }

            public override List<Tile> GetAllTiles()
            {
                return base.GetAllTiles().Union(PlayedFlowerTiles).ToList();
            }

            public override List<Tile> GetScoringTiles(Permutation permutation)
            {
                return base.GetScoringTiles(permutation).Union(PlayedFlowerTiles).ToList();
            }

            public override List<Tile> GetPlayingTiles()
            {
                return base.GetPlayingTiles().Union((newPlayedFlowerTiles ?? new List<FlowerTile>())
                    .Where(tile => PlayedFlowerTiles.Contains(tile))).ToList();
            }

            public override bool IsPlayingTile(Tile tile)
            {
                return tile is FlowerTile flower && PlayedFlowerTiles.Contains(flower)
                    ? newPlayedFlowerTiles?.Contains(flower) == true
                    : base.IsPlayingTile(tile);
            }

            public override void TriggerPostSettlePermutationEvent(Permutation permutation)
            {
                base.TriggerPostSettlePermutationEvent(permutation);
                newPlayedFlowerTiles?.Clear();
            }

            public override bool RemoveTileFromDiscarded(Tile toRemove, string message = "")
            {
                if (toRemove is not FlowerTile flower || !PlayedFlowerTiles.Contains(flower))
                    return base.RemoveTileFromDiscarded(toRemove, message);

                PlayerEvents.PreRemoveTileEvent evt = new(this, flower) { message = message };
                EventBus.Publish(evt);
                if (evt.canceled) return false;

                PlayedFlowerTiles.Remove(flower);
                newPlayedFlowerTiles?.Remove(flower);
                flower.UnsubscribeFromPlayer(this);
                EventBus.Publish(new PlayerEvents.PostRemoveTileEvent(this, flower));
                stats.RecordCustomStats("tile_destoryed", 1);
                return true;
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
                newPlayedFlowerTiles?.Clear();
            }

            protected override void AddExtraScoringEffects(List<IAnimationEffect> queue)
            {
                Permutation perm = GetCurrentSelectedPerm() ?? GetAccumulatedPermutation();
                foreach (FlowerTile flower in PlayedFlowerTiles)
                {
                    queue.Add(new TileScoringEffectAppendEffect(this, flower, perm, playHandEffectStack));
                    queue.Add(SimpleAppendEffect.Create(playHandEffectStack, () =>
                        GetPostScoreEffectsFromTile(perm, flower).Select(effect => effect.OnTile(flower))
                            .ToList<IAnimationEffect>()));
                }
            }

            public override void AppendAdditionalTileRoundEndEffects(List<IAnimationEffect> onRoundEndEffects, Permutation permutation)
            {
                base.AppendAdditionalTileRoundEndEffects(onRoundEndEffects, permutation);
                foreach (FlowerTile flower in PlayedFlowerTiles)
                {
                    flower.AppendOnRoundEndEffects(this, permutation, onRoundEndEffects);
                    if (flower.properties.mask is not TileMaskSuppressed)
                        flower.AppendRoundEndEffect(onRoundEndEffects, this, permutation);
                }
            }

            public override List<Destination> GenerateDestinations()
            {
                List<Destination> destinations = base.GenerateDestinations();
                if (CurrentLevel.IsChapterStart && Level < GameLevel.StandardRunCompletionLevel)
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
