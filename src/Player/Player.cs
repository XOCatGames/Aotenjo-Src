using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aotenjo.Console;
using Unity.VisualScripting;
using UnityEngine;
using static Aotenjo.Tile;
using Random = Unity.Mathematics.Random;

namespace Aotenjo
{
    [Serializable]
    public class Player
    {
        #region 变量
        
        /// <summary>
        /// 持有遗物
        /// </summary>
        [SerializeField] protected List<int> HeldArtifacts;

        [SerializeField] protected List<string> NewHeldArtifacts;

        /// <summary>
        /// 持有小道具
        /// </summary>
        [SerializeReference] protected List<Gadget> HeldGadgets;

        [SerializeReference] private Gadget lastUsedConsumableGadget;

        /// <summary>
        /// 牌库
        /// </summary>`
        [SerializeReference] protected List<Tile> TilePool;

        /// <summary>
        /// 手牌（待出牌）
        /// </summary>
        [SerializeReference] protected List<Tile> HandDeck;

        /// <summary>
        /// 弃牌堆
        /// </summary>
        [SerializeReference] protected List<Tile> Discarded;

        /// <summary>
        /// 在回合内选中的所有Tile的List
        /// </summary>
        [SerializeReference] protected List<Tile> CurrentSelectedTiles;

        /// <summary>
        /// 已组建牌型
        /// </summary>
        [SerializeReference] protected Permutation CurrentAccumulatedBlock;
        

        /// <summary>
        /// 所有待抽取遗物的List
        /// </summary>
        [SerializeReference] protected List<string> ArtifactBank;
        
        /// <summary>
        /// 不同雀头的选中牌型
        /// </summary>
        protected Permutation CachedSelectedPermutation;

        /// <summary>
        /// 当前关卡, 从1开始
        /// </summary>
        [SerializeField] public int Level;


        /// <summary>
        /// 剩余弃牌次数
        /// </summary>
        [SerializeField] public int DiscardLeft;

        /// <summary>
        /// 跳过次数
        /// </summary>
        [SerializeField] public int SkipCount;

        /// <summary>
        /// 玩家属性
        /// </summary>
        [SerializeField] protected PlayerProperties properties;

        /// <summary>
        /// 玩家番种等级
        /// </summary>
        [SerializeField] protected SkillSet skillSet;

        [SerializeField] public double levelTarget;

        /// <summary>
        /// 回合累计分数
        /// </summary>
        [SerializeField] public double CurrentAccumulatedScore;

        /// <summary>
        /// 当前番符
        /// </summary>
        [SerializeField] public Score RoundAccumulatedScore;

        /// <summary>
        /// 持有金钱
        /// </summary>
        [SerializeField] protected int money;

        /// <summary>
        /// 在本回合中的目前出牌阶段
        /// </summary>
        [SerializeField] public int CurrentPlayingStage;

        /// <summary>
        /// 是否处于出牌模式
        /// </summary>
        [SerializeField] public bool isPlayHandMode = true;

        [SerializeField] public YakuType[] nativeYakus;

        [SerializeField] public string currentBossName;

        [SerializeField] private string nextBossName;

        [SerializeField] public PlayerStats stats;

        [SerializeField] protected double targetMultiplier = 1.0;

        [SerializeField] public SerializableMap<Skill.SkillType, int> skillMap;

        [NonSerialized] public Boss currentBoss;

        [NonSerialized] private GameLevel currentLevel;

        [SerializeReference] private GameLevel savedLevel;
        [NonSerialized] private bool changingLevel;

        /// <summary>
        /// During Exit/Enter, returns the level whose callback is running without restoring it again.
        /// Use SetLevel to change the level number; direct writes are supported for legacy callers.
        /// </summary>
        public GameLevel CurrentLevel
        {
            get
            {
                if (!changingLevel && (currentLevel == null || currentLevel.Number != Level))
                    RestoreCurrentLevel();

                return currentLevel;
            }
        }

        private Boss nextBoss;

        [SerializeField] public List<string> generalBossesNamePool =
            Bosses.BossList.Where(b => !Bosses.FinalBossList.Contains(b)).Select(b => b.name).ToList();

        [SerializeField] public List<string> terminalBossesNamePool = Bosses.FinalBossList.Select(b => b.name).ToList();

        [SerializeField] public List<string> upcomingBosses = new List<string>();

        /// <summary>
        /// 出牌模式：0 - 正常，1 - 七对，2 - 十三幺
        /// </summary>
        [SerializeField] public int playMode;

        [SerializeReference] public MahjongDeck deck;

        [SerializeReference] public MaterialSet materialSet = MaterialSet.Basic;

        [SerializeField] public bool won;

        [SerializeField] public string randomSeed;

        [SerializeField] public int ascensionLevel;

        [SerializeField] public bool usedBoost;

        [SerializeField] public bool seededRun;

        /// <summary>
        /// 本局是否使用过控制台指令
        /// </summary>
        [SerializeField] public bool usedConsoleCommand;

        [SerializeField] public bool inRound = true;

        [SerializeField] public bool stillInTutorial;

        [SerializeField] public bool tutorialFirstDrawArtifact = true;
        
        //计分相关
        public Stack<IAnimationEffect> playHandEffectStack;
        public Stack<IAnimationEffect> roundEndEffectsStack;
        public Stack<IAnimationEffect> discardTileEffectsStack;

        [SerializeReference] public List<YakuType> pinnedYakus;
        
        #endregion

        public void SetArtifactLimit(int n)
        {
            properties.ArtifactLimit = n;
        }

        public void SetHandLimit(int n)
        {
            properties.HandLimit = n;
        }

        /// <summary>
        /// 小局内已经构建好的perm
        /// </summary>
        /// <returns>可能是null</returns>
        public Permutation GetAccumulatedPermutation()
        {
            if ((CurrentAccumulatedBlock == null) || (CurrentAccumulatedBlock.blocks == null)) return null;
            if (CurrentAccumulatedBlock.blocks.Length == 0) return null;
            if (CurrentAccumulatedBlock.jiang == null) return null;
            if (CurrentAccumulatedBlock.jiang.tile1 == null) return null;
            if (CurrentAccumulatedBlock.jiang.tile2 == null) return null;
            if (CurrentAccumulatedBlock.jiang.tile1.GetOrder() == 0) return null;
            return CurrentAccumulatedBlock;
        }

        public void SetCurrentAccumulatedBlock(Permutation perm)
        {
            CurrentAccumulatedBlock = perm;
        }

        public virtual List<Artifact> GetArtifacts()
        {
            var artifacts = NewHeldArtifacts.Select(Artifacts.GetArtifact).Where(a => a != null).ToList();
            if (!artifacts.Any() && HeldArtifacts != null && HeldArtifacts.Any())
            {
                artifacts = HeldArtifacts.Select(a => Artifacts.ArtifactList.First(ar => ar.GetNumberID() == a)).ToList();
                NewHeldArtifacts = artifacts.Select(a => a.GetRegName()).ToList();
            }
            return artifacts.ToList();
        }

        public virtual List<Tile> GetTilePool()
        {
            return new List<Tile>(TilePool);
        }

        public virtual String GetArtifactText()
        {
            var artifacts = GetArtifacts();
            StringBuilder sb = new StringBuilder();
            foreach (var artifact in artifacts)
            {
                sb.Append(artifact);
                sb.Append('\n');
            }

            return sb.ToString();
        }

        #region 初始化
        
        public Player(List<Tile> tilePool, string randomSeed, MahjongDeck deck, MaterialSet set, int ascensionLevel = 0)
            : this(tilePool, randomSeed, deck, PlayerProperties.DEFAULT, 0, SkillSet.StandardSkillSet(), set, ascensionLevel)
        {
        }

        protected Player(List<Tile> tilePool, string randomSeed, MahjongDeck deck, PlayerProperties properties,
            int initialMoney, SkillSet initialSkillSet, MaterialSet materialSet, int ascensionLevel = 0)
        {
            HeldArtifacts = new();
            NewHeldArtifacts = new();
            TilePool = new();
            HandDeck = new();
            Discarded = new();
            Level = 1;
            CurrentPlayingStage = 0;
            SkipCount = 0;
            this.properties = properties;

            if (ascensionLevel >= 4)
            {
                properties.HandLimit--;
            }

            if (ascensionLevel >= 7)
            {
                properties.GadgetLimit = 5;
            }

            if (ascensionLevel < 8)
            {
                generalBossesNamePool.RemoveAll(b => Bosses.HardBossList.Any(b2 => b2.name == b));
                terminalBossesNamePool.RemoveAll(b => Bosses.HardBossList.Any(b2 => b2.name == b));
            }

            DiscardLeft = properties.DiscardLimit;
            uint seed = InitializeSeed(randomSeed);

            random = new Random(seed);
            randomMap = new RandomMap();
            this.deck = deck;
            this.materialSet = materialSet;
            ArtifactBank = Artifacts.ArtifactList
                .Where(a => a.IsAvailableGlobally(this) && 
                                    a.GetUnlockRequirement()
                                        .IsUnlocked(ProfileManager.GetCurrentPlayerProfile().profileStats))
                .Select(a => a.GetRegName()).ToList();

            stats = PlayerStats.New();

            tilePool.ForEach(AddTileToPool);

            //Initialize all artifacts
            Artifacts.ArtifactList.ToList().ForEach(a =>
            {
                a.ResetArtifactState(this);
                a.PreGameInitialized(this);
            });
            
            playHandEffectStack = new Stack<IAnimationEffect>();
            roundEndEffectsStack = new Stack<IAnimationEffect>();
            discardTileEffectsStack = new Stack<IAnimationEffect>();

            CurrentSelectedTiles = new List<Tile>();
            money = initialMoney;
            RoundAccumulatedScore = Score.Base();
            skillSet = initialSkillSet;
            nativeYakus = skillSet.GetExtraLeveledYakus();
            SetCurrentAccumulatedBlock(null);
            levelTarget = GetBasicLevelTarget();
            HeldGadgets = new();
            this.ascensionLevel = ascensionLevel;
            skillMap = new SerializableMap<Skill.SkillType, int>();
            skillSet.SetPlayer(this);

            materialSet?.SubscribeToPlayerEvents(this);

            GenerateNewUpcomingBosses();

            savedLevel = currentLevel = new NormalLevel(Level);

            pinnedYakus = new List<YakuType>();
        }

        private uint InitializeSeed(string randomSeed)
        {
            uint seed = 0;
            if (string.IsNullOrEmpty(randomSeed))
            {
                seededRun = false;
                seed = (uint)new System.Random().Next(5000, int.MaxValue - 5000);
                this.randomSeed = seed.ToSafeString();
            }
            else
            {
                seededRun = true;
                this.randomSeed = randomSeed;
                seed = new Random(randomSeed[0]).NextUInt(5000, uint.MaxValue - 5000);
                foreach (char c in randomSeed)
                {
                    seed = seed * 2 + c;
                    if (seed == uint.MaxValue) seed--;
                }

                uint parseRes;
                uint.TryParse(randomSeed, out parseRes);
                if (parseRes != 0) seed = parseRes;
            }

            return seed;
        }

        public void InitPointers()
        {
            CachedSelectedPermutation = null;
            CurrentAccumulatedBlock = null;
            skillSet.SetPlayer(this);
        }

        #endregion


        public SkillSet GetSkillSet()
        {
            return skillSet;
        }

        public YakuType[] GetLearntYakus()
        {
            return skillSet.GetYakus();
        }

        public YakuType[] GetNonNativeYaku()
        {
            return skillSet.GetYakus().Where(y => !nativeYakus.Contains(y) && skillSet.GetLevel(y) > 0).ToArray();
        }

        public YakuPackConsumeResult ConsumeYakuPack(YakuPack pack)
        {
            return skillSet.Consume(pack, this);
        }

        /// <summary>
        /// 购买并消耗一个番种包
        /// </summary>
        /// <param name="pack">番种包</param>
        /// <param name="price">番种包价格</param>
        /// <returns>如果玩家所持金钱不够，返回Null</returns>
        public YakuPackConsumeResult BuyAndConsumeYakuPack(YakuPack pack, int price)
        {
            if (GetMoney() < price) return null;
            SpendMoney(price);
            return ConsumeYakuPack(pack);
        }

        public int GetHandLimit()
        {
            return properties.HandLimit;
        }

        public int GetDiscardLimit()
        {
            return properties.DiscardLimit;
        }

        public int GetArtifactLimit()
        {
            return properties.ArtifactLimit;
        }

        public List<Tile> GetHandDeckCopy()
        {
            return new(HandDeck);
        }

        public List<Tile> GetSelectedTilesCopy()
        {
            return new(CurrentSelectedTiles);
        }

        /// <summary>本次打出的牌，包含等待首次计分的已补花牌。</summary>
        public virtual List<Tile> GetPlayingTiles()
        {
            return GetSelectedTilesCopy();
        }

        /// <summary>参与牌效果结算的牌；花牌不加入番种和面子的判定。</summary>
        public virtual List<Tile> GetScoringTiles(Permutation permutation)
        {
            return permutation?.ToTiles() ?? new List<Tile>();
        }

        public List<Tile> GetUnusedTilesInHand()
        {
            return GetHandDeckCopy().Except(CurrentSelectedTiles).ToList();
        }

        public double GetLevelTarget()
        {
            return levelTarget;
        }

        public double GetBasicLevelTarget()
        {
            return GetBasicLevelTarget(Level);
        }

        public double GetBasicLevelTarget(int projectedLevel)
        {
            double roundNum = ((projectedLevel - 1) / 4) % 4;
            double roundBaseTarget = 150D * Math.Pow(GetRoundIncreMultiplier(), roundNum) *
                                     Math.Pow(GetRestartIncreMultiplier(), (projectedLevel - 1) / 16);
            if (projectedLevel > 32)
            {
                roundBaseTarget *= Math.Pow(10D, ((projectedLevel - 32) / 2));
            }

            if (projectedLevel > 48)
            {
                roundBaseTarget *= Math.Pow(3D + ((projectedLevel - 48) / 4), ((projectedLevel - 48) / 2));
            }

            if (projectedLevel > 64)
            {
                roundBaseTarget *= Math.Pow(10D + 2 * (projectedLevel - 64), ((projectedLevel - 64) / 4));
            }

            if (projectedLevel > 80)
            {
                roundBaseTarget = Math.Pow(roundBaseTarget, 1.2 + 0.1 * ((projectedLevel - 80) / 4));
            }

            if (projectedLevel > 96)
            {
                roundBaseTarget = Math.Pow(roundBaseTarget, 1.2);
            }

            if (projectedLevel >= 112)
            {
                roundBaseTarget = double.PositiveInfinity;
            }

            double roundMultiplier = GetRoundMultiplier(projectedLevel);
            return (roundBaseTarget * roundMultiplier * targetMultiplier);
        }

        private double GetRoundIncreMultiplier()
        {
            return GetAscensionLevel() switch
            {
                0 => 5D,
                1 => 7D,
                2 => 7D,
                3 => 7D,
                4 => 7D,
                _ => 10D
            };
        }

        private double GetRestartIncreMultiplier()
        {
            return GetAscensionLevel() switch
            {
                0 => 2000D,
                1 => 4000D,
                2 => 4000D,
                3 => 4000D,
                4 => 4000D,
                _ => 12000D
            };
        }

        private double GetRoundMultiplier()
        {
            return GetRoundMultiplier(Level);
        }

        private double GetRoundMultiplier(int projectedLevel)
        {
            int bigRound = ((projectedLevel - 1) / 4) % 4;
            int roundNum = ((projectedLevel - 1) % 4) + 1;
            double val = roundNum switch
            {
                1 => 3D,
                2 => projectedLevel > 17 ? 5D : 4D,
                3 => projectedLevel > 17 ? 8D : 6D,
                4 => projectedLevel > 17 ? 20D : 12,
                _ => throw new ArgumentException("illegal round num")
            };
            return bigRound == 1 ? 1.25 * val : val;
        }

        public void IncreaseTargetMultiplier(double v)
        {
            targetMultiplier += v;
        }

        public void ResetScore()
        {
            RoundAccumulatedScore = Score.Base();
        }

        public void ApplyEffect(Effect effect, Stack<IAnimationEffect> followingEffectStack = null)
        {
            var artifactsAtTrigger = GetArtifacts();
            effect.Ingest(this);

            if (effect.WillTrigger())
            {
                var triggered = new PlayerEvents.PostIngestEffectEvent(this,
                    GetCurrentSelectedPerm() ?? CurrentAccumulatedBlock, effect, artifactsAtTrigger);
                EventBus.Publish(triggered);
                var followingEffects = new List<IAnimationEffect>();
                foreach (Effect followingEffect in triggered.followingEffects)
                {
                    var neighbors = new List<IAnimationEffect> { followingEffect };
                    TriggerOnAddSingleAnimationEffectEvent(neighbors, followingEffect);
                    followingEffects.AddRange(neighbors);
                }
                if (followingEffectStack != null)
                {
                    // Stack order ensures reactions animate immediately after their source.
                    for (int i = followingEffects.Count - 1; i >= 0; i--)
                        followingEffectStack.Push(followingEffects[i]);
                }
                else
                {
                    foreach (IAnimationEffect followingEffect in followingEffects)
                        ApplyEffect(followingEffect.GetEffect());
                }
            }

            stats.SyncPlayer(this);
        }
        

        public List<Effect> GetPostScoreEffectsFromTile(Permutation permutation, Tile tile)
        {
            List<Effect> effects = new();
            GetArtifacts().ForEach(a => a.AddOnTileEffectsPostEvents(this, permutation, tile, effects));
            return effects;
        }

        public List<IAnimationEffect> GetScoreEffectsFromBlock(Permutation permutation, Block block)
        {
            List<IAnimationEffect> animationEffects = new();
            List<Effect> effects = new();
            GetArtifacts().ForEach(a => a.AddOnBlockEffects(this, permutation, block, effects));
            animationEffects.AddRange(effects.Select(e => new OnBlockAnimationEffect(block, e)));
            GetArtifacts().ForEach(a => a.AppendPostBlockAnimationEffects(this, permutation, block, animationEffects));
            return animationEffects;
        }

        public List<Effect> GetScoreEffectsFromArtifacts(Permutation permutation)
        {
            List<Effect> effects = new();
            GetArtifacts().ForEach(a => a.AppendOnSelfEffects(this, permutation, effects));
            return effects;
        }

        public List<Tile> DrawTilesFromPool(int n)
        {
            return DrawTilesFromPool(n, _ => true);
        }

        public List<Tile> DrawTilesFromPool(int n, Predicate<Tile> pred)
        {
            List<Tile> tiles = new();
            for (int i = 0; i < n; i++)
            {
                List<Tile> cands = TilePool.Where(t => pred(t)).ToList();
                if (cands.Count <= 0) cands = TilePool;
                if (cands.Count <= 0) break;
                Tile drawed = cands[GenerateRandomInt(cands.Count)];
                tiles.Add(drawed);
                TilePool.Remove(drawed);
            }

            TilePool.AddRange(tiles);
            return tiles;
        }
        
        public List<Tile> DrawPlainTilesFromPool(int n)
        {
            return DrawTilesFromPool(n, t => t.CompatWithMaterial(TileMaterial.PLAIN, this));
        }

        public int GetLevelBaseBonusMoney()
        {
            return CurrentLevel.BaseBonusMoney;
        }

        public int GetAotenjoBonusMoney()
        {
            double accScore = CurrentAccumulatedScore + RoundAccumulatedScore.GetScore();
            if (accScore / GetLevelTarget() <= 1) return 0;
            return (GetAscensionLevel() >= 2 ? 1 : 2) * Utils.GetAotenjoBonus(Math.Floor(accScore / GetLevelTarget()));
        }

        public int GetDiscardBonusMoney()
        {
            return (GetArtifacts().Contains(Artifacts.Teppoudama) ? 2 : 1) * DiscardLeft / 5;
        }

        public int GetInterestBonusMoney()
        {
            return Math.Max(0, Math.Min(GetMoney() / 5, 5));
        }

        public int GetRoundEndTotalMoney()
        {
            return GetLevelBaseBonusMoney() + GetDiscardBonusMoney() + GetInterestBonusMoney() + GetAotenjoBonusMoney();
        }

        public bool DetermineForceDiscard(Tile tile)
        {
            bool rawRes = false;
            PlayerEvents.DetermineForceDiscardTileEvent evt = new(this, tile, rawRes);
            EventBus.Publish(evt);
            return evt.res;
        }

        public bool CanDiscardTile(Tile tile, bool forceDiscard, bool consumeDiscardChance)
        {
            bool rawRes = !consumeDiscardChance || DiscardLeft > 0;
            PlayerEvents.DetermineDiscardTileEvent evt = new(this, tile, rawRes, forceDiscard, consumeDiscardChance);
            EventBus.Publish(evt);
            if (evt.canceled) return false;
            return evt.res;
        }

        public bool PreDiscardTile(Tile tile, bool forced)
        {
            PlayerEvents.PreDiscardTileEvent preEvent = new(this, tile, true);
            preEvent.forced = forced;
            EventBus.Publish(preEvent);
            if (preEvent.canceled) return false;
            return true;
        }

        /// <summary>
        /// 从手牌中弃一张牌放入弃牌堆
        /// </summary>
        /// <param name="tile">需要丢弃的手牌</param>
        /// <param name="forced">是否为强打</param>
        /// <returns>若已达到弃牌上限返回-1，如果事件被取消返回-2，否则返回弃牌的位置</returns>
        public virtual int DiscardTile(Tile tile, bool forced)
        {
            if (!GetHandDeckCopy().Contains(tile))
            {
                return -1;
            }

            stats.RecordCustomStats("discard", 1);
            if (tile.IsYaoJiu(this)) stats.RecordCustomStats("discard_yaojiu", 1);

            int Pos = HandDeck.IndexOf(tile);
            HandDeck.Remove(tile);
            Discarded.Add(tile);

            PlayerEvents.PostDiscardTileEvent postEvent = new(this, tile);
            EventBus.Publish(postEvent);
            if (postEvent.canceled) return -2;
            return Pos;
        }

        public int MoveFromHandToDiscard(Tile tile)
        {
            int pos = HandDeck.IndexOf(tile);
            HandDeck.Remove(tile);

            Discarded.Add(tile);

            return pos;
        }

        public int MoveFromDiscardToPool(Tile tile)
        {
            int pos = Discarded.IndexOf(tile);
            Discarded.Remove(tile);
            TilePool.Add(tile);
            return pos;
        }

        public int MoveFromHandToPool(Tile tile)
        {
            int pos = HandDeck.IndexOf(tile);
            HandDeck.Remove(tile);
            TilePool.Add(tile);
            return pos;
        }

        public List<Tile> priortizedDrawingList = new List<Tile>();

        public void AddPrioritizedDrawingTile(Tile tile)
        {
            Tile cand = GetTilePool().Except(priortizedDrawingList).FirstOrDefault(t => t.CompatWith(tile));
            if (cand != null)
            {
                priortizedDrawingList.Add(cand);
            }
        }

        /// <summary>
        /// 从牌库复制中随机抽一张牌放入手牌
        /// </summary>
        /// <returns>牌插入手中的位置，如果牌库没牌了，返回-1，如果事件被取消了，返回-2</returns>
        public int DrawTileToHandDeck(bool sortDeck = true)
        {
            if (TilePool.Count == 0) return -1;

            //Randomly get a cand pos from the pool
            int pos = GenerateRandomInt(TilePool.Count, "draw_tile");
            Tile toDraw = TilePool[pos];

            if (priortizedDrawingList.Count > 0)
            {
                toDraw = priortizedDrawingList[0];
                priortizedDrawingList.Remove(toDraw);
            }

            PlayerDrawTileEvent.Pre preEvent = new(this, toDraw);
            EventBus.Publish(preEvent);

            toDraw = preEvent.tile;
            
            TilePool.Remove(toDraw);

            HandDeck.Add(toDraw);
            if (sortDeck)
                SortDeck();

            PlayerDrawTileEvent.Post postEvent = new(this, toDraw);
            EventBus.Publish(postEvent);
            return HandDeck.IndexOf(toDraw);
        }

        /// <summary>
        /// 不排序手牌的弃牌函数
        /// </summary>
        /// <param name="tile">需要弃置的手牌</param>
        /// <returns>牌插入手中的位置，如果牌库没牌了，返回-1，如果事件被取消了，返回-2</returns>
        public int ReplaceTileAndKeepPosition(Tile tile)
        {
            PlayerEvents.PreDiscardTileEvent preDiscardEvent = new(this, tile, true);
            EventBus.Publish(preDiscardEvent);
            if (preDiscardEvent.canceled) return -2;

            stats.RecordCustomStats("discard", 1);
            if (tile.IsYaoJiu(this)) stats.RecordCustomStats("discard_yaojiu", 1);

            int Pos = HandDeck.IndexOf(tile);
            Discarded.Add(tile);

            EventBus.Publish(new PlayerEvents.PostDiscardTileEvent(this, tile));

            if (TilePool.Count == 0)
            {
                HandDeck[Pos] = null;
                return -1;
            }

            //Randomly get a cand pos from the pool
            int toDrawPos = GenerateRandomInt(TilePool.Count);
            Tile toDraw = TilePool[toDrawPos];

            PlayerDrawTileEvent.Pre preDrawEvent = new(this, toDraw);
            EventBus.Publish(preDrawEvent);
            // Only accept redirects to a tile still in the wall. Use the same
            // tile for removal, insertion and the post-draw notification.
            if (preDrawEvent.tile != null && TilePool.Contains(preDrawEvent.tile))
            {
                toDraw = preDrawEvent.tile;
            }
            TilePool.Remove(toDraw);

            HandDeck[Pos] = toDraw;

            PlayerDrawTileEvent.Post postDrawEvent = new(this, toDraw);
            EventBus.Publish(postDrawEvent);
            return Pos;
        }

        public void SortDeck()
        {
            HandDeck = HandDeck.Where(e => e != null).ToList();
            HandDeck.Sort();
        }

        public virtual bool RemoveTileFromDiscarded(Tile toRemove, string message = "")
        {
            PlayerEvents.PreRemoveTileEvent evt = new(this, toRemove);
            evt.message = message;

            EventBus.Publish(evt);
            if (evt.canceled) return false;
            bool res = Discarded.Remove(toRemove);
            toRemove.UnsubscribeFromPlayer(this);

            EventBus.Publish(new PlayerEvents.PostRemoveTileEvent(this, toRemove));

            stats.RecordCustomStats("tile_destoryed", 1);

            return res;
        }

        /// <summary>
        /// 从永久牌库中移除Tile
        /// </summary>
        /// <param name="toRemove">需要删除的Tile</param>
        /// <returns>移除是否成功</returns>
        public bool RemoveTileFromPool(Tile toRemove)
        {
            PlayerEvents.PreRemoveTileEvent evt = new(this, toRemove);
            EventBus.Publish(evt);

            if (evt.canceled) return false;

            bool res = TilePool.Remove(toRemove);
            toRemove.UnsubscribeFromPlayer(this);

            EventBus.Publish(new PlayerEvents.PostRemoveTileEvent(this, toRemove));
            stats.RecordCustomStats("tile_destoryed", 1);
            return res;
        }

        /// <summary>
        /// 从手牌中永久移除Tile
        /// </summary>
        /// <param name="toRemove">需要删除的Tile</param>
        /// <returns>移除是否成功</returns>
        public bool RemoveTileFromHand(Tile toRemove, bool forced = false, bool destroyed = false)
        {
            if (forced)
            {
                bool r = HandDeck.Remove(toRemove);
                return r;
            }

            PlayerEvents.PreRemoveTileEvent evt = new(this, toRemove);
            EventBus.Publish(evt);

            if (evt.canceled) return false;

            bool res = HandDeck.Remove(toRemove);
            toRemove.UnsubscribeFromPlayer(this);

            if (destroyed)
            {
                EventBus.Publish(new PlayerEvents.PostRemoveTileEvent(this, toRemove));
                stats.RecordCustomStats("tile_destoryed", 1);
            }
            
            return res;
        }

        /// <summary>
        /// 往永久牌库中添加Tile
        /// </summary>
        /// <param name="toAdd">添加的Tile</param>
        public void AddTileToPool(Tile toAdd)
        {
            toAdd.SubscribeToPlayerEvents(this);
            TilePool.Add(toAdd);
        }

        /// <summary>
        /// 往永久牌库中添加新Tile
        /// </summary>
        /// <param name="toAdd">添加的Tile</param>
        public bool AddNewTileToPool(Tile toAdd)
        {
            PlayerEvents.PreAddTileEvent preAddTileEvt = new(this, toAdd);
            EventBus.Publish(preAddTileEvt);
            if (preAddTileEvt.canceled) return false;
            toAdd.SubscribeToPlayerEvents(this);
            TilePool.Add(toAdd);
            PlayerEvents.PostAddTileEvent postAddTileEvt = new(this, toAdd);
            EventBus.Publish(postAddTileEvt);
            MessageManager.Instance.OnAddTileEvent(new List<Tile> { toAdd });
            return true;
        }

        public void AddTileToDiscarded(Tile newTile)
        {
            newTile.SubscribeToPlayerEvents(this);
            Discarded.Add(newTile);
        }

        /// <summary>
        /// 往手牌中添加Tile
        /// </summary>
        /// <param name="toAdd">添加的Tile</param>
        public void AddTileToHand(Tile toAdd)
        {
            toAdd.SubscribeToPlayerEvents(this);
            HandDeck.Add(toAdd);
        }

        public virtual void InitHandDeck()
        {
            if (GetTilePool().Any(t => GetTilePool().Count(t2 => t2 == t) == 2))
            {
                TilePool = TilePool.Distinct().ToList();
                Debug.LogError("ERROR: DUP TILE FOUND");
            }

            while (HandDeck.Count < GetHandLimit() && TilePool.Count != 0)
            {
                DrawTileToHandDeck();
            }
        }

        /// <summary>
        /// 结算一次出牌
        /// </summary>
        /// <param name="hand">有效的一手牌（5张）</param>
        /// <returns> 所有摸进手牌的牌 </returns>
        public List<Tile> Play(Hand hand)
        {
            EventBus.Publish(new PlayerEvents.PreSettlePermutationEvent(this, GetAccumulatedPermutation()));
            
            List<Tile> tiles = hand.tiles;

            if (tiles.Count == 0)
            {
                return new();
            }

            
            List<Tile> drawTileResults = new List<Tile>();
            foreach (var item in tiles)
            {
                MoveFromHandToDiscard(item);
                int drawTileRes = DrawTileToHandDeck();
                if (drawTileRes != -1)
                    drawTileResults.Add(GetHandDeckCopy()[drawTileRes]);
            }
            
            Permutation perm = GetCurrentSelectedPerm();
            List<Block> blocks = GetCurrentSelectedBlocks();
            DiscardLeft += 2 * blocks.Count(b => GetCombinator().IsKong(b));
            blocks.Where(b => GetCombinator().IsKong(b)).ToList().ForEach(b => OnKong(b, perm));
            SetCurrentAccumulatedBlock(perm ?? throw new ArgumentNullException());

            List<YakuType> yakuTypes = perm.GetYakus(this, true).Where(y => !nativeYakus.Contains(y)).ToList();

            yakuTypes.ForEach(yaku => skillSet.TryUnlockYakuIfLocked(yaku, perm.IsFullHand(this)));

            //Score is now calculated in animations, stacked in `RoundAccumulatedScore`
            Score score = RoundAccumulatedScore;
            CurrentAccumulatedScore += Math.Floor(score.GetScore());
            RecordCurrentHand(perm, score);

            stats.RecordPlay(perm, this, perm.GetYakus(this, true).Where(a => skillSet.GetLevel(a) > 0).ToList(),
                (Score) RoundAccumulatedScore.Clone());
            stats.SyncPlayer(this);
            TriggerPostSettlePermutationEvent(perm);
            ResetScore();
            CurrentPlayingStage++;
            DiscardLeft += properties.DiscardRefill;

            return drawTileResults;
        }

        private void RecordCurrentHand(Permutation perm, Score score)
        {
            List<YakuType> displayedYakus = perm.GetYakus(this);
            SerializableMap<YakuType, double> fanMap = new SerializableMap<YakuType, double>();
            displayedYakus.ForEach(yaku => fanMap.Add(yaku, skillSet.CalculateFan(yaku, perm.blocks.Length)));
            stats.recordHand(perm, this, fanMap, score);
        }

        public void SkipSettle()
        {
            EventBus.Publish(new PlayerRoundEvent.Skip.Pre(this));
            SkipCount++;
            DiscardLeft += 10;
            CurrentPlayingStage++;
            EventBus.Publish(new PlayerRoundEvent.Skip.Post(this));
        }

        public bool OnRoundEndButtonPressed()
        {
            PlayerRoundEvent.End.PrePre prePreRoundEndEvent = new(this);
            EventBus.Publish(prePreRoundEndEvent);
            if (prePreRoundEndEvent.canceled) return false;

            PlayerRoundEvent.End.PostPre postPreRoundEndEvent = new(this);
            EventBus.Publish(postPreRoundEndEvent);
            return true;
        }

        /// <summary>
        /// 结算当前牌型
        /// </summary>
        /// <returns>是否过关</returns>
        public bool OnRoundEnd()
        {
            stats.SyncPlayer(this);
            if (Math.Floor(CurrentAccumulatedScore) >= Math.Floor(GetLevelTarget()))
            {
                EventBus.Publish(new PlayerRoundEvent.End.Pre(this));
                SettleMoney();
                ResetTilePool();
                EventBus.Publish(new PlayerRoundEvent.End.Post(this));
                ResetScore();

                Boss completedBoss = (CurrentLevel as BossLevel)?.Boss;
                if (completedBoss != null)
                    stats.RecordCustomStats($"encounter_boss_{completedBoss.name}", 1);

                ExitCurrentLevel();
                Level++;

                if (Level == 5 && GetAscensionLevel() >= 3)
                {
                    properties.DiscardLimit -= 5;
                }

                RestoreCurrentLevel();

                if (CurrentLevel.IsChapterStart)
                {
                    while (upcomingBosses.Count <= Level / 4)
                        GenerateNewUpcomingBosses();
                }
            }
            else
            {
                GetArtifacts().ForEach(a => a.ResetArtifactState(this));
                return false;
            }

            DiscardLeft = properties.DiscardLimit;
            SetCurrentAccumulatedBlock(null);
            CurrentAccumulatedScore = 0;
            SkipCount = 0;
            CurrentPlayingStage = 0;
            isPlayHandMode = true;
            inRound = false;
            return true;
        }

        public virtual void ResetTilePool()
        {
            TilePool.AddRange(HandDeck);
            TilePool.AddRange(Discarded);
            HandDeck.Clear();
            Discarded.Clear();
            GetAllTiles().ForEach(t => t.ClearTransform(this));
        }

        protected virtual void ResetBoss()
        {
            if (currentLevel is BossLevel)
                SetCurrentLevel(new NormalLevel(Level));
            else
            {
                currentBoss = null;
                currentBossName = null;
            }
        }

        public void EncounterNextBoss(Boss nextB)
        {
            nextBoss = nextB;
            nextBossName = nextB.name;
        }

        public void SetCurrentBoss(Boss boss)
        {
            EnsureNotChangingLevel();
            if (boss == null)
                throw new ArgumentNullException(nameof(boss));

            if (currentLevel is BossLevel bossLevel && ReferenceEquals(bossLevel.Boss, boss) &&
                currentLevel.Number == Level)
                return;

            SetCurrentLevel(new BossLevel(Level, boss));
        }

        /// <summary>
        /// Restores the saved level, or infers it from legacy fields when no level was saved.
        /// Rebinds lifecycle callbacks after loading or clearing the event bus.
        /// </summary>
        public void RestoreCurrentLevel()
        {
            EnsureNotChangingLevel();
            if ((currentLevel != null && currentLevel.Number != Level) ||
                (savedLevel != null && savedLevel.Number != Level))
                ExitCurrentLevel();

            GameLevel level = savedLevel ?? CreateCurrentLevel();
            ValidateLevel(level);
            ReplaceCurrentLevel(level);
        }

        protected virtual GameLevel CreateCurrentLevel()
        {
            return GameLevelFactory.Create(this);
        }

        /// <summary>
        /// Replaces and saves the runtime level while keeping legacy boss fields in sync.
        /// Assigning the current instance does nothing. Use RestartCurrentLevel to re-enter it.
        /// </summary>
        public void SetCurrentLevel(GameLevel level)
        {
            EnsureNotChangingLevel();
            ValidateLevel(level);
            if (ReferenceEquals(currentLevel, level))
                return;

            ReplaceCurrentLevel(level);
        }

        /// <summary>
        /// Explicitly runs Exit/Enter again on the active level, including its encounter resets.
        /// </summary>
        public void RestartCurrentLevel()
        {
            EnsureNotChangingLevel();
            if (currentLevel == null || currentLevel.Number != Level)
                RestoreCurrentLevel();
            else
                ReplaceCurrentLevel(currentLevel);
        }

        private void ValidateLevel(GameLevel level)
        {
            if (level == null)
                throw new ArgumentNullException(nameof(level));
            if (level.Number != Level)
                throw new ArgumentException("Runtime level number must match the persisted player level.", nameof(level));
            if (!level.GetType().IsSerializable)
                throw new ArgumentException("GameLevel subclasses must be marked Serializable to survive saves.", nameof(level));
        }

        private void EnsureNotChangingLevel()
        {
            if (changingLevel)
                throw new InvalidOperationException("Cannot change levels from a level lifecycle callback.");
        }

        private void ReplaceCurrentLevel(GameLevel level)
        {
            EnsureNotChangingLevel();
            changingLevel = true;
            try
            {
                currentLevel?.Exit(this);

                savedLevel = currentLevel = level;
                currentBoss = (level as BossLevel)?.Boss;
                currentBossName = currentBoss?.name;

                if (currentBoss != null && GameLevelFactory.IsScheduledBossLevel(Level))
                {
                    int bossIndex = (Level / 4) - 1;
                    while (upcomingBosses.Count <= bossIndex)
                        GenerateNewUpcomingBosses();
                    upcomingBosses[bossIndex] = currentBoss.name;
                }

                currentLevel?.Enter(this);
            }
            finally
            {
                changingLevel = false;
            }
        }

        private void ExitCurrentLevel()
        {
            ReplaceCurrentLevel(null);
        }

        public string GetOrCreateBossNameForLevel(int levelNumber)
        {
            if (!GameLevelFactory.IsScheduledBossLevel(levelNumber))
                return null;

            int bossIndex = (levelNumber / 4) - 1;
            while (upcomingBosses.Count <= bossIndex)
                GenerateNewUpcomingBosses();

            return upcomingBosses[bossIndex];
        }

        public void GenerateNewUpcomingBosses()
        {
            for (int i = 0; i < 3; i++)
            {
                upcomingBosses.Add(DrawNewBoss(false).name);
            }

            upcomingBosses.Add(DrawNewBoss(true).name);
        }

        protected virtual Boss DrawNewBoss(bool finalRound)
        {
            int generalBossCount = generalBossesNamePool.Count();
            int terminalBossCount = terminalBossesNamePool.Count();

            bool InfiniteRun = Level > 16;

            bool harderBossesEnabled = HarderBossesEnabled();
            
            if (InfiniteRun)
            {
                if (finalRound)
                {
                    return Bosses.FinalBossList[GenerateRandomInt(Bosses.FinalBossList.Length, "boss")];
                }

                return Bosses.GetPreviewOrElseRedraw(generalBossesNamePool[GenerateRandomInt(generalBossCount, "boss")], harderBossesEnabled);
            }

            Boss result;
            if (finalRound)
            {
                result = Bosses.GetPreviewOrElseRedraw(
                    terminalBossesNamePool[GenerateRandomInt(terminalBossCount, "boss")], harderBossesEnabled);
            }
            else
            {
                result = Bosses.GetPreviewOrElseRedraw(generalBossesNamePool[GenerateRandomInt(generalBossCount, "boss")], harderBossesEnabled);
            }


            if (generalBossesNamePool.Contains(result.name))
                generalBossesNamePool.Remove(result.name);

            return result;
        }

        public void SettleMoney()
        {
            int money = 0;
            money += GetInterestBonusMoney();
            money += GetLevelBaseBonusMoney();
            money += GetAotenjoBonusMoney();
            money += GetDiscardBonusMoney();

            EarnMoney(money);
        }

        [AotenjoCommand("earn", "ToInt")]
        public void EarnMoney(int money)
        {
            if (money < 0)
            {
                SpendMoney(-money);
                return;
            }

            PlayerEvents.EarnMoneyEvent evt = new(this, money);
            EventBus.Publish(evt);
            if (evt.canceled) return;

            stats.MoneyEarned(evt.amount);
            this.money += evt.amount;
        }

        /// <summary>
        /// 用已搭建的牌型加上选中的手牌尝试组建新的牌型
        /// </summary>
        /// <returns>可能为Null的新牌型</returns>
        public Permutation GetCurrentSelectedPerm()
        {
            if (CurrentSelectedTiles.Count < 5) return null;
            Hand hand = new(CurrentSelectedTiles);
            
            //TODO: 兼容哩咕哩咕和带面子十三幺
            if (hand.GetPerms(this, playMode).Count == 0) return null;

            bool firstHand = GetAccumulatedPermutation() == null;

            if (!CanSettleMoreTile(firstHand)) return null;
            
            Permutation perm = CombineSelectedTilesToCurrentBlocks(hand, firstHand);

            if (CachedSelectedPermutation != null && perm.ToTiles()
                                                      .All(t => CachedSelectedPermutation.ToTiles().Contains(t))
                                                  && CachedSelectedPermutation.ToTiles()
                                                      .All(t => perm.ToTiles().Contains(t)))
                return CachedSelectedPermutation;

            CachedSelectedPermutation = perm;
            return perm;
        }

        protected virtual bool CanSettleMoreTile(bool firstHand)
        {
            return firstHand || playMode == 0;
        }

        public void ClearCachedSelectedPerm()
        {
            CachedSelectedPermutation = null;
        }

        public bool TrySetPair(Tile t1, Tile t2)
        {
            if (GetCurrentSelectedPerm() == null || GetCurrentSelectedPerm().ToTiles().Count < 2)
            {
                return false;
            }

            Hand hand = new(CurrentSelectedTiles);

            bool firstHand = GetAccumulatedPermutation() == null;

            if (!firstHand && playMode != 0) return false;

            Permutation newPerm = CombineSelectedTilesToCurrentBlocks(hand, firstHand, t1, t2);

            if (newPerm == null) return false;

            CachedSelectedPermutation = newPerm;
            return true;
        }

        /// <returns>可能会回传null，代表无对应结果</returns>
        private Permutation CombineSelectedTilesToCurrentBlocks(Hand hand, bool firstHand)
        {
            //TODO: 兼容哩咕哩咕和带面子十三幺
            return firstHand
                ? hand.GetHighestScoredPerm(this)
                : hand.GetHighestScoredPerm(GetAccumulatedPermutation().blocks.ToList(), this);
        }

        /// <returns>可能会回传null，代表无对应结果</returns>
        private Permutation CombineSelectedTilesToCurrentBlocks(Hand hand, bool firstHand, Tile p1, Tile p2)
        {
            //TODO: 兼容哩咕哩咕和带面子十三幺
            return firstHand
                ? hand.GetHighestScoredPerm(this, p1, p2)
                : hand.GetHighestScoredPerm(GetAccumulatedPermutation().blocks.ToList(), this, p1, p2);
        }


        /// <summary>
        /// 随机抽取n个遗物，若抽取失败则返回空List
        /// </summary>
        /// <param name="n"></param>
        /// <returns>n个可能为null的遗物列表</returns>
        public List<Artifact> TryDrawRandomArtifact(int n)
        {
            if (stillInTutorial && n == 3 && tutorialFirstDrawArtifact)
            {
                tutorialFirstDrawArtifact = false;
                return new List<Artifact> { Artifacts.CopperStatue, Artifacts.BaseOrizuru, Artifacts.PeachWoodSword };
            }

            Dictionary<int, int[]> weightMap = new()
            {
                { 1, new[] { 100, 16, 2 } },
                { 2, new[] { 65, 25, 4 } },
                { 3, new[] { 45, 30, 12 } },
                { 4, new[] { 30, 40, 20 } },
            };

            List<Artifact> artifactList = new List<Artifact>();
            int[] weights = weightMap[Math.Min(4, Level / 4 + 1)];

            for (int i = 0; i < n; i++)
            {
                LotteryPool<Artifact> pool = new();

                LotteryPool<Artifact> commonPool = GenerateEquatedPoolFromRarity(Rarity.COMMON);
                LotteryPool<Artifact> rarePool = GenerateEquatedPoolFromRarity(Rarity.RARE);
                LotteryPool<Artifact> epicPool = GenerateEquatedPoolFromRarity(Rarity.EPIC);
                epicPool.Add(rarePool, 1);

                pool.Add(commonPool, weights[0]);
                pool.Add(rarePool, weights[1]);
                pool.Add(epicPool, weights[2]);

                Artifact artifact = pool.Draw(max => GenerateRandomInt(max, "artifact_shop"));
                artifact.IsBroken = false;
                artifact.IsTemporary = false;
                artifactList.Add(artifact);
                ArtifactBank.Remove(artifact.GetRegName());
            }

            ArtifactBank.AddRange(artifactList.Select(a => a.GetRegName()));

            DrawArtifactInShopEvent.On onEvt = new DrawArtifactInShopEvent.On(this, artifactList);
            EventBus.Publish(onEvt);
            
            DrawArtifactInShopEvent.Post postEvt = new DrawArtifactInShopEvent.Post(this, new  List<Artifact>(artifactList));
            EventBus.Publish(postEvt);
            
            return artifactList;
        }

        private LotteryPool<Artifact> GenerateEquatedPoolFromRarity(Rarity rarity)
        {
            List<Artifact> bank = ArtifactBank.Select(Artifacts.GetArtifact).Where(a => a != null).ToList();
            LotteryPool<Artifact> pool = new();
            foreach (var artifact in bank.Where(artifact => artifact.GetRarity() == rarity && artifact.IsAvailableInShops(this)))
            {
                pool.Add(artifact, 100);
            }

            return pool;
        }

        /// <summary>
        /// 购买工艺品的函数，将购买成功的工艺品加入玩家的持有列表中
        /// </summary>
        /// <param name="artifact"></param>
        /// <returns>是否成功购买工艺品, 0:成功购买, -1:没有足够金钱, -2:无法购买</returns>
        public int BuyArtifact(Artifact artifact, bool reduced, int price)
        {
            if (reduced)
            {
                price = (int)(price * 0.75);
            }

            int moneyAvailable = GetMoney();
            if (moneyAvailable < price)
            {
                return -1;
            }

            if (!ObtainArtifact(artifact)) return -2;
            SpendMoney(price);
            stats.OnPurchaseArtifact(artifact);
            stats.RecordArtifactShopPurchase(artifact, Level, price, moneyAvailable);

            return 0;
        }
        
        

        [AotenjoCommand("give", "ToArtifact", "ToBool")]
        public bool ObtainArtifact(Artifact artifact, bool forced = false)
        {
            if (!forced && !TryObtainArtifact(artifact)) return false;

            if (artifact.IsTemporary) SetArtifactLimit(GetArtifactLimit() + 1);
            NewHeldArtifacts.Add(artifact.GetRegName());
            artifact.OnObtain(this);
            ArtifactBank.Remove(artifact.GetRegName());
            stats.OnObtainArtifact(artifact);

            EventBus.Publish(new PlayerEvents.PostObtainArtifactEvent(this, artifact));

            OnArtifactOrderChanged();

            return true;
        }

        private bool TryObtainArtifact(Artifact artifact)
        {
            PlayerArtifactEvent.DetermineGettability evt =
                new PlayerArtifactEvent.DetermineGettability(this, artifact, true);
            if (!artifact.CanObtainBy(this) || (!artifact.IsTemporary &&
                                                !artifact.CanBeBoughtWithoutSlotLimit(this) &&
                                                GetArtifacts().Count >= properties.ArtifactLimit))
            {
                evt.res = false;
            }

            bool res = ArtifactBank.Contains(artifact.GetRegName());
            if (!res) evt.res = false;


            var obtainEvent = new PlayerEvents.PreObtainArtifactEvent(this, artifact, evt.res);
            obtainEvent.canceled = evt.canceled;
            EventBus.Publish(obtainEvent);

            return obtainEvent.res;
        }

        public List<Tile> GetRiverTiles()
        {
            if (GetAccumulatedPermutation() == null) return new List<Tile>(Discarded);
            return Discarded.Except(GetAccumulatedPermutation().ToTiles()).ToList();
        }

        /// <summary>
        /// 售出一个拥有的遗物
        /// </summary>
        /// <param name="artifact"></param>
        /// <returns>玩家是否在售出的时候拥有这个遗物</returns>
        public bool SellArtifact(Artifact artifact)
        {
            int price = artifact.GetSellingPrice();
            bool res = RemoveArtifact(artifact, true);
            if (!res) return false;
            EarnMoney(price);
            return true;
        }

        public bool RemoveArtifact(Artifact artifact, bool resetArtifactState, bool reshuffleIntoPool = true)
        {
            EventBus.Publish(new PlayerEvents.PreRemoveArtifactEvent(this, artifact));
            NewHeldArtifacts.Remove(artifact.GetRegName());
            artifact.OnRemoved(this);
            if (resetArtifactState)
                artifact.ResetArtifactState(this);
            if (!(artifact.IsUnique()) && reshuffleIntoPool)
            {
                if (GetArtifacts().Contains(Artifacts.BlackHole) && Artifacts.BlackHole.Level > 0)
                {
                    Artifacts.BlackHole.Level--;
                    return true;
                }

                foreach (Artifact component in artifact.GetComponents())
                    ArtifactBank.Add(component.GetRegName());
            }

            return true;
        }

        #region RNG相关



        /// <summary>
        /// 玩家随机种子
        /// </summary>
        [SerializeField] public Random random;

        [SerializeReference] public RandomMap randomMap;

        [Serializable]
        public class RandomMap : SerializableMap<string, Random>
        {
        }

        public int GenerateRandomInt(int v, string category)
        {
            if (!randomMap.Contains(category))
            {
                randomMap.Add(category, new Random(random.NextUInt(1, uint.MaxValue - 1)));
            }

            Random respRandom = randomMap.Get(category);
            int v1 = respRandom.NextInt(v);
            Random newRandom = new Random(respRandom.state);
            randomMap.Add(category, newRandom);
            return v1;
        }
        
        public Func<int, int> GetRng(string category)
        {
            return (v) => GenerateRandomInt(v, category);
        }

        public int GenerateRandomInt(int v)
        {
            return GenerateRandomInt(v, "default");
        }

        public List<Tile> GenerateRandomTileWithEffects(int v, bool normal = false)
        {
            List<Tile> tiles = new();
            for (int i = 0; i < v; i++)
            {
                List<Tile> cand = GetUniqueFullDeck();
                Tile baseTile = new(cand[GenerateRandomInt(cand.Count)]);
                if(!normal)
                    baseTile.properties = GenerateRandomTileProperties(40, 9, 1, 25);
                tiles.Add(baseTile);
            }

            return tiles;
        }

        public List<Tile> GenerateRandomTileGroupWithEffects(int n, int normalWeight = 80, int commonWeight = 19,
            int epicWeight = 1, int fontedPercentage = 25, bool canGenerateHonorSeq = true, bool canBeMixed = true)
        {
            List<Tile> tiles = new();
            bool isAbc = GenerateRandomInt(4) <= 2;
            Tile initialTile = null;
            if (isAbc)
            {
                if (GenerateRandomInt(8) == 0 && GetUniqueFullDeck().Any(t => t.IsHonor(this)) && canGenerateHonorSeq)
                {
                    List<Tile> pool = GetUniqueFullDeck().Where(t => t.IsHonor(this)).ToList();
                    initialTile = pool[GenerateRandomInt(pool.Count)];
                    if (initialTile.GetCategory() == Category.Feng)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            tiles.Add(new Tile(initialTile).SetOrderForced((initialTile.GetOrder() + i - 1) % 4 + 1));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < n; i++)
                        {
                            tiles.Add(new Tile(initialTile).SetOrderForced((initialTile.GetOrder() + i - 5) % 3 + 5));
                        }
                    }
                }
                else
                {
                    List<Tile> pool = GetUniqueFullDeck().Where(t => t.IsNumbered() && t.GetOrder() <= (10 - n))
                        .ToList();
                    initialTile = pool[GenerateRandomInt(pool.Count)];
                    for (int i = 0; i < n; i++)
                    {
                        tiles.Add(new Tile(initialTile).SetOrderForced(initialTile.GetOrder() + i));
                    }
                }
            }
            else
            {
                initialTile = GetUniqueFullDeck()[GenerateRandomInt(GetUniqueFullDeck().Count)];
                for (int i = 0; i < n; i++)
                {
                    tiles.Add(new Tile(initialTile));
                }

                bool isMixed = GenerateRandomInt(2) == 0 && !canBeMixed;
                if (isMixed && initialTile.IsNumbered())
                {
                    for (int i = 1; i < n; i++)
                    {
                        tiles[i].SetCategoryForced((Category)((((int)tiles[i - 1].GetCategory()) + 1) % 3));
                    }
                }
            }

            foreach (Tile tile in tiles)
            {
                tile.properties =
                    GenerateRandomTileProperties(normalWeight, commonWeight, epicWeight, fontedPercentage);
            }

            return tiles;
        }

        public List<PropertiesPack> GeneratePropertyPacks(int commonWeight = 98, int rareWeight = 2)
        {
            List<PropertiesPack> packs = new List<PropertiesPack>();
            for (int i = 0; i < 3; i++)
            {
                PropertiesPack pack = new PropertiesPack(GenerateRandomTileProperties(0, 100, 0, 0), 3, 3);
                if (GenerateRandomInt(commonWeight + rareWeight) <= rareWeight)
                {
                    pack = new PropertiesPack(GenerateRandomTileProperties(0, 0, 100, 0), 1, 6);
                }

                if (pack.bluePrint.material.GetRegName().Equals(TileMaterial.Ore().GetRegName())
                    || pack.bluePrint.material.GetRegName().Equals(TileMaterial.COPPER.GetRegName())
                    || pack.bluePrint.material.GetRegName().Equals(TileMaterial.MysteriousColorPorcelain().GetRegName())
                    || pack.bluePrint.material.GetRegName().Equals(TileMaterial.Nest().GetRegName())
                    || pack.bluePrint.material.GetRegName().Equals(TileMaterial.MistWood().GetRegName()))
                {
                    pack.count++;
                }

                if (pack.bluePrint.material is TileMaterialMechPart &&
                    pack.bluePrint.material.GetRarity() == Rarity.COMMON)
                {
                    pack.count += 2;
                }

                packs.Add(pack);
            }

            return packs;
        }

        public TileProperties GenerateRandomTileProperties(int plainWeight, int commonWeight, int rareWeight,
            int fontedPercentage)
        {
            LotteryPool<TileFont> fontPool = new();

            LotteryPool<TileFont> fontedPool = new LotteryPool<TileFont>();
            fontedPool.Add(TileFont.RED, 6)
                .Add(TileFont.BLUE, 1);

            fontPool.Add(TileFont.PLAIN, 100 - fontedPercentage)
                .Add(fontedPool, fontedPercentage);

            TileFont font = fontPool.Draw(GenerateRandomInt);

            LotteryPool<TileMaterial> materialPool = new();
            LotteryPool<TileMaterial> rareMaterialPool = materialSet.GenerateRareMaterialPool();

            LotteryPool<TileMaterial> commonMaterialPool = materialSet.GenerateCommonMaterialPool();

            materialPool.Add(TileMaterial.PLAIN, plainWeight)
                .Add(rareMaterialPool, rareWeight)
                .Add(commonMaterialPool, commonWeight);

            TileMaterial material = materialPool.Draw(GenerateRandomInt);
            return TileProperties.Plain().ChangeMaterial(material).ChangeFont(font);
        }
        

        #endregion

        /// <summary>
        /// Boss计分效果触发前、所有手牌效果触发后扳机、目前用于幽魂木
        /// </summary>
        /// <param name="effects"></param>
        public void TriggerPrePostAddOnTileAnimationEffect(List<IAnimationEffect> effects)
        {
            EventBus.Publish(new PlayerEvents.OnPrePostAddOnTileAnimationEffectEvent(this,
                GetCurrentSelectedPerm(), effects));
        }

        /// <summary>
        /// 手牌计分效果触发后，观赏效果触发前扳机，一般用于Boss计分效果
        /// </summary>
        /// <param name="effects"></param>
        public void TriggerPostAddOnTileAnimationEffect(List<OnTileAnimationEffect> effects)
        {
            EventBus.Publish(new PlayerEvents.OnPostAddOnTileAnimationEffectEvent(this,
                GetCurrentSelectedPerm(), effects));
        }

        public void TriggerPostAddOnArtifactAnimationEffect(List<IAnimationEffect> effects)
        {
            EventBus.Publish(new PlayerEvents.OnPostAddScoringAnimationEffectEvent(this,
                GetCurrentSelectedPerm() ?? CurrentAccumulatedBlock, effects));
        }

        public void TriggerPostAddOnBlockAnimationEffect(List<IAnimationEffect> effects)
        {
            EventBus.Publish(new PlayerEvents.OnPostAddOnBlockAnimationEffectEvent(this,
                GetCurrentSelectedPerm(), effects));
        }

        public void TriggerOnAddSingleAnimationEffectEvent(List<IAnimationEffect> neighbors, IAnimationEffect effect)
        {
            EventBus.Publish(new PlayerEvents.OnAddSingleAnimationEffectEvent(this, neighbors, effect));
        }

        public virtual List<Tile> GetAllTiles()
        {
            List<Tile> tiles = new();
            tiles.AddRange(HandDeck);
            tiles.AddRange(Discarded);
            tiles.AddRange(TilePool);
            return tiles.Where(t => t != null).ToList();
        }

        public int GetSelectionCount()
        {
            return ascensionLevel >= 6 ? 7 : 8;
        }

        private void DecreaseMoney(int v)
        {
            money -= v;
        }

        public int GetMoney()
        {
            return money;
        }

        [AotenjoCommand("spend", "ToInt")]
        public void SpendMoney(int v)
        {
            EventBus.Publish(new PlayerEvents.SpendMoneyEvent(this, v));
            MessageManager.Instance.OnSpendMoney(v);
            DecreaseMoney(v);
            stats.SpendMoney(v);
        }

        public virtual void OnRoundStart()
        {
            EventBus.Publish(new PlayerRoundEvent.Start.Pre(this));

            lastUsedConsumableGadget = null;
            HeldGadgets.ForEach(g => g.OnRoundStart(this));
            InitHandDeck();
            levelTarget = GetBasicLevelTarget();

            EventBus.Publish(new PlayerRoundEvent.Start.Post(this));
            inRound = true;
            stats.SyncPlayer(this);
        }

        public int GetCurrentBlockCount()
        {
            if (GetCurrentSelectedPerm() == null)
            {
                return GetAccumulatedPermutation() == null ? 0 : GetAccumulatedPermutation().blocks.Length;
            }

            return GetCurrentSelectedPerm().blocks.Length;
        }

        public bool Selecting(Tile tile)
        {
            PlayerEvents.DetermineSelectingTileEvent evt = new(this, tile, GetSelectedTilesCopy().Contains(tile));
            EventBus.Publish(evt);
            return evt.res;
        }

        /// <summary>判断打出时触发的牌效果，不改变普通手牌的选择状态。</summary>
        public virtual bool IsPlayingTile(Tile tile)
        {
            return Selecting(tile);
        }

        public int GetGadgetLimit()
        {
            return properties.GadgetLimit;
        }

        internal List<Gadget> GetGadgets()
        {
            return new List<Gadget>(HeldGadgets);
        }

        [AotenjoCommand("giveGadget", nameof(ArgumentParsers.ToGadget), nameof(ArgumentParsers.ToBool))]
        public bool AddGadget(Gadget gadget, bool allowPartial = false)
        {
            bool partialTransfered = false;
            if (HeldGadgets.Count >= GetGadgetLimit())
            {
                foreach (Gadget heldG in GetGadgets())
                {
                    if (heldG.IsConsumable() && heldG.regName.Equals(gadget.regName))
                    {
                        if (heldG.GetStackLimit() - heldG.uses >= gadget.uses)
                        {
                            heldG.uses += gadget.uses;
                            EventBus.Publish(new PlayerEvents.ObtainGadgetEvent(this, gadget));
                            stats.OnBoughtGadget(gadget);
                            return true;
                        }

                        if (allowPartial && heldG.GetStackLimit() - heldG.uses > 0)
                        {
                            int transferAmount = Math.Min(heldG.GetStackLimit() - heldG.uses, gadget.uses);
                            heldG.uses += transferAmount;
                            gadget.uses -= transferAmount;
                            partialTransfered = true;
                        }
                    }
                }

                if (partialTransfered)
                {
                    EventBus.Publish(new PlayerEvents.ObtainGadgetEvent(this, gadget));
                    stats.OnBoughtGadget(gadget);
                    return true;
                }

                return false;
            }

            foreach (Gadget heldG in GetGadgets())
            {
                if (heldG.IsConsumable() && heldG.regName.Equals(gadget.regName))
                {
                    if (heldG.GetStackLimit() - heldG.uses > 0)
                    {
                        int transferAmount = Math.Min(heldG.GetStackLimit() - heldG.uses, gadget.uses);
                        heldG.uses += transferAmount;
                        gadget.uses -= transferAmount;
                        if (gadget.uses <= 0)
                        {
                            EventBus.Publish(new PlayerEvents.ObtainGadgetEvent(this, gadget));
                            stats.OnBoughtGadget(gadget);
                            return true;
                        }
                    }
                }
            }

            gadget.OnObtained(this);
            EventBus.Publish(new PlayerEvents.ObtainGadgetEvent(this, gadget));
            stats.OnBoughtGadget(gadget);
            HeldGadgets.Add(gadget);
            return true;
        }

        public void RemoveGadget(Gadget gadget)
        {
            HeldGadgets.Remove(gadget);
        }

        public void SetArtifactOrder(Artifact[] array)
        {
            NewHeldArtifacts =
                new List<string>(array.Where(a => a != null).Select(a => a.GetRegName()));
            OnArtifactOrderChanged();
        }

        public void OnArtifactOrderChanged()
        {
            if (GameManager.Instance == null || GameManager.Instance.onCounting) return;

            // 解绑所有遗物事件
            foreach (Artifact artifact in GetArtifacts())
            {
                artifact.UnsubscribeToPlayer(this);
            }

            //检查是否满足合成表
            foreach (ArtifactRecipe recipe in ArtifactRecipes.recipes)
            {
                if (recipe.CheckFulfillRecipeRequirement(this))
                {
                    recipe.OnFulfillRecipeResult(this);

                    OnArtifactOrderChanged();
                    UIArtifactAnimationController.Instance.StartRecipe(recipe);
                    return;
                }
            }

            // 新绑定当前所有遗物
            foreach (Artifact artifact in GetArtifacts())
            {
                artifact.SubscribeToPlayer(this);
            }
        }


        public void SetGadgets(List<Gadget> gadgets)
        {
            HeldGadgets = new List<Gadget>(gadgets);
        }

        public List<Gadget> GenerateFreeGadgets()
        {
            if (Level == 1 && randomSeed == "TUTORIAL")
            {
                return new List<Gadget> { Gadgets.Rice };
            }

            List<Gadget> gadgets = GenerateGadgets(1, false);
            if (gadgets[0].IsConsumable())
            {
                gadgets[0].uses = Math.Max(1, gadgets[0].uses - 1);
            }

            return gadgets;
        }

        public List<Gadget> GenerateGadgets(int n, bool inShop = true, int commonWeight = 10, int rareWeight = 2)
        {
            return GenerateGadgets(n, _ => true, inShop, commonWeight, rareWeight);
        }

        public List<Gadget> GenerateGadgets(int n, Predicate<Gadget> pred, bool inShop = true, int commonWeight = 9,
            int rareWeight = 2)
        {
            List<Gadget> gadgets = new();
            LotteryPool<Gadget> pool = new();

            pool.Add(GetRareGadgetPool(pred, inShop), rareWeight);
            pool.Add(GetCommonGadgetPool(pred, inShop), commonWeight);

            for (int i = 0; i < n; i++)
            {
                gadgets.Add(pool.Draw(t => GenerateRandomInt(t, "gadget"), false));
            }

            return gadgets;
        }


        private LotteryPool<Gadget> GetRareGadgetPool(Predicate<Gadget> pred, bool inShop = true)
        {
            LotteryPool<Gadget> pool = new();
            pool.AddRange(Gadgets.GadgetList(this, inShop).Where(g => pred(g) && g.GetRarity() == Rarity.RARE).ToList(),
                10);
            if (pool.IsEmpty())
            {
                return GetCommonGadgetPool(pred, inShop);
            }

            return pool;
        }

        private LotteryPool<Gadget> GetCommonGadgetPool(Predicate<Gadget> pred, bool inShop = true)
        {
            LotteryPool<Gadget> pool = new();
            pool.AddRange(
                Gadgets.GadgetList(this, inShop).Where(g => pred(g) && g.GetRarity() == Rarity.COMMON).ToList(), 10);
            return pool;
        }

        /// <param name="tile">加杠的牌</param>
        /// <param name="block">被加杠的刻子</param>
        /// <param name="perm">打出的Perm</param>
        /// <returns>杠牌在手中的位置</returns>
        /// <exception cref="ArgumentException">无法加杠</exception>
        public int KongTile(Tile tile, Block block, Permutation perm)
        {
            PlayerEvents.PreKongTileEvent eventData = new(this, tile, perm, block);
            EventBus.Publish(eventData);
            if (eventData.canceled) return -1;

            bool res = block.Kong(tile, GetCombinator());
            if (!res) throw new ArgumentException("INVALID KONG COMMAND RECEIVED");
            MoveFromHandToDiscard(tile);
            DiscardLeft += 2;
            OnKong(block, perm);
            return DrawTileToHandDeck();
        }

        /// <summary>
        /// 杠牌回调
        /// </summary>
        /// <param name="block">被杠面子</param>
        /// <param name="perm">当前牌型</param>
        public virtual void OnKong(Block block, Permutation perm)
        {
            EventBus.Publish(new PostKongTilesEvent(this, perm, block));
        }

        [AotenjoCommand("destroyYaku", nameof(ArgumentParsers.ToYakuType))]
        public void DestroyYaku(YakuType yakuTypeID)
        {
            int level = skillSet.GetLevel(yakuTypeID);
            PlayerEvents.DeleteYakuEvent yakuEvent = new(this, yakuTypeID, level);
            EventBus.Publish(yakuEvent);
            if (yakuEvent.canceled) return;
            skillSet.ClearLevel(yakuTypeID);
        }
        
        [AotenjoCommand("setLevel", nameof(ArgumentParsers.ToInt))]
        public void SetLevel(int level)
        {
            EnsureNotChangingLevel();
            if (level <= 0)
            {
                throw new ArgumentException("Level must be positive");
            }

            if (Level == level)
                return;

            ExitCurrentLevel();
            Level = level;
            RestoreCurrentLevel();
        }
        
        [AotenjoCommand("upgradeYaku", nameof(ArgumentParsers.ToYakuType), nameof(ArgumentParsers.ToInt))]
        public void UpgradeYaku(YakuType yaku, int level)
        {
            if (level <= 0)
            {
                throw new ArgumentException("Level must be positive");
            }
            GetSkillSet().AddLevel(yaku, level);
        }
        
        [AotenjoCommand("setAscension", nameof(ArgumentParsers.ToInt))]
        public void SetAscensionLevel(int level)
        {
            if (level < 0 || level > 15)
            {
                throw new ArgumentException("Ascension level must be between 0 and 15");
            }
            ascensionLevel = level;
        }
        
        [AotenjoCommand("listArtifacts", nameof(ArgumentParsers.ToInt))]
        public void ListAvailableArtifacts(int count)
        {
            if (count <= 0) count = 20;
            var artifacts = Artifacts.ArtifactList.Take(count).ToArray();
            Debug.Log($"Available artifacts (first {count}):");
            foreach (var artifact in artifacts)
            {
                Debug.Log($"- {artifact.GetNameID()} (Field: {artifact.GetType().Name})");
            }
        }

        public List<Block> GetCurrentSelectedBlocks()
        {
            var lst = new List<Block>();
            Permutation perm = GetCurrentSelectedPerm();
            if (perm == null) return null;
            foreach (var block in perm.blocks)
            {
                if (CurrentSelectedTiles.Any(t => block.tiles.Contains(t)))
                {
                    lst.Add(block);
                }
            }

            return lst;
        }

        public virtual BlockCombinator GetCombinator()
        {
            return BlockCombinator.Default;
        }

        public int GetPlayerWind()
        {
            return new RoundStatus(this).PlayerWind;
        }

        public int GetPrevalentWind()
        {
            return new RoundStatus(this).RoundWind;
        }

        /// <summary>
        /// 抽取番种包，返回抽取到的番种包列表
        /// </summary>
        /// <param name="drawCount">抽取数量</param>
        /// <param name="globalYakuPacks">总番种包集合，将从中筛选出玩家拥有的番种包池再进行抽取</param>
        /// <returns>抽取结果</returns>
        /// <exception cref="ArgumentOutOfRangeException">抽取数量大于可用番种包数量（通常为4）</exception>
        public virtual List<YakuPack> TryDrawYakuPack(int drawCount, List<YakuPack> globalYakuPacks)
        {
            List<YakuPack> bank = new(globalYakuPacks.Where(y => properties.YakuPacks.Contains(y.id)));
            if (bank.Count < drawCount) throw new ArgumentOutOfRangeException(nameof(drawCount));
            List<YakuPack> result = new();
            for (int i = 0; i < drawCount; i++)
            {
                LotteryPool<YakuPack> pool = new LotteryPool<YakuPack>();
                bank.ForEach(p => pool.Add(p, 1));
                YakuPack pollResult = pool.Draw(GenerateRandomInt);
                result.Add(pollResult);
                bank.Remove(pollResult);
            }

            return result;
        }

        /// <summary>
        /// 小道具使用后回调
        /// </summary>
        /// <param name="gadget">被使用完毕的小道具</param>
        /// <param name="tile">受体牌（可为null）</param>
        public void PostUsedGadget(Gadget gadget, Tile tile = null)
        {
            if (gadget.uses < 0) throw new ArgumentException("Gadget uses exhausted");
            if (gadget.IsConsumable())
            {
                lastUsedConsumableGadget = gadget.Copy().SetUses(Math.Max(1, gadget.uses));
            }
            gadget.uses--;
            if (gadget.uses == 0)
            {
                if (gadget.IsConsumable())
                {
                    HeldGadgets.Remove(gadget);
                }
            }

            var playerGadgetEvent = new PlayerEvents.PostUseGadgetEvent(this, gadget);
            playerGadgetEvent.tile = tile;
            EventBus.Publish(playerGadgetEvent);
        }

        public Gadget GetLastUsedConsumableGadget()
        {
            return lastUsedConsumableGadget;
        }

        /// <summary>
        /// 抽取可用地点
        /// </summary>
        /// <returns>抽取结果</returns>
        public virtual List<Destination> GenerateDestinations()
        {
            int v = Level > 8 ? 4 : 2;
            int saleIndex = GenerateRandomInt(v);
            LotteryPool<Destination> commonDestination = new();

            commonDestination.Add(GadgetsShopDestination.Create(this, Destination.DestinationEventType.COMMON), 10);
            commonDestination.Add(TileDeleteShopDestination.Create(this, Destination.DestinationEventType.COMMON), 9);
            commonDestination.Add(TileModifyShopDestination.Create(this, Destination.DestinationEventType.COMMON), 10);
            commonDestination.Add(TileAddShopDestination.Create(this, Destination.DestinationEventType.COMMON), 10);

            if (!CurrentLevel.IsPostBossChapterStart)
            {
                commonDestination.Add(new WastelandDestination(false, this), 10);
            }

            List<Destination> result = new();

            for (int i = 0; i < v; i++)
            {
                result.Add(commonDestination.Draw(range => GenerateRandomInt(range, "destination"), false));
            }

            if (CurrentLevel.IsPostBossChapterStart)
            {
                result[0] = result[0].GetRandomRedEventVariant(this);
                if (v == 4)
                {
                    result[2] = result[2].GetRandomRedEventVariant(this);
                }
                else
                {
                    result[1] = result[1].GetRandomRedEventVariant(this);
                }
            }
            else if (!result[saleIndex].IsOnEvent())
                result[saleIndex].SetOnSale();


            EventBus.Publish(new PlayerEvents.PostGenerateDestinationEvent(this, result));

            return result;
        }

        public List<Artifact> DrawRandomArtifact(Rarity rarity, int count)
        {
            List<Artifact> validArtifacts = ArtifactBank.Select(Artifacts.GetArtifact)
                .Where(a => a != null && a.IsAvailableInShops(this) && a.GetRarity() == rarity).ToList();
            LotteryPool<Artifact> pool = new LotteryPool<Artifact>();
            pool.AddRange(validArtifacts);
            List<Artifact> res = new();
            for (int i = 0; i < count; i++)
            {
                if (pool.IsEmpty()) break;
                res.Add(pool.Draw(max => GenerateRandomInt(max, "artifact_shop"), false));
            }

            return res;
        }

        public List<Gadget> DrawRandomGadget(Rarity rarity, int count, bool inShop = true)
        {
            List<Gadget> validArtifacts = Gadgets.GadgetList(this, inShop).Where(a => a.GetRarity() == rarity).ToList();
            LotteryPool<Gadget> pool = new LotteryPool<Gadget>();
            pool.AddRange(validArtifacts);
            List<Gadget> res = new();
            for (int i = 0; i < count; i++)
            {
                res.Add(pool.Draw(GenerateRandomInt, false));
            }

            return res;
        }

        public void RedrawHandTiles(List<Tile> hand)
        {
            TilePool.AddRange(HandDeck);
            HandDeck.Clear();
            foreach (var tile in hand)
            {
                TilePool.Remove(TilePool.First(t => t.CompatWith(tile)));
                HandDeck.Add(tile);
            }
        }


        public PermutationType[] GetAvailablePermTypes()
        {
            List<PermutationType> types = new();
            List<YakuType> availableYakus = deck.GetAvailableYakus().Select(y => y.GetYakuType()).ToList();
            if (availableYakus.Contains(FixedYakuType.Base))
            {
                types.Add(PermutationType.NORMAL);
            }

            if (availableYakus.Contains(FixedYakuType.QiDui))
            {
                types.Add(PermutationType.SEVEN_PAIRS);
            }

            if (availableYakus.Contains(FixedYakuType.ShiSanYao))
            {
                types.Add(PermutationType.THIRTEEN_ORPHANS);
            }
            if (availableYakus.Contains(FixedYakuType.LiGuLiGu))
            {
                types.Add(PermutationType.LIGULIGU);
            }
            if (availableYakus.Contains(FixedYakuType.YiShiSanYao))
            {
                types.Add(PermutationType.EXTENDED_THIRTEEN_ORPHANS);
            }
            return types.ToArray();
        }

        public static string GetLevelTitle(Func<string, string> loc, int Level)
        {
            RoundStatus roundStatus = new RoundStatus(Level);

            return string.Format(loc("wind_format"), loc(roundStatus.GetRoundWindKey()),
                loc(roundStatus.GetPlayerWindKey())) + ((Level - 1) / 16 == 0 ? "" : $"+{(Level - 1) / 16}");
        }

        public int GetAscensionLevel()
        {
            return ascensionLevel;
        }

        public List<Effect> GetUnusedEffectsFromTile(Permutation perm, Tile tile)
        {
            List<Effect> effects = new List<Effect>();
            tile.AppendToListUnusedEffect(this, perm, effects);
            GetArtifacts().ForEach(a => a.AppendOnUnusedTileEffects(this, perm, tile, effects));
            return effects;
        }

        [AotenjoCommand("setHand", nameof(ArgumentParsers.ToPlainTiles))]
        public void SetHandTiles(Tile[] tiles)
        {
            foreach (var tile in tiles)
            {
                tile.SubscribeToPlayerEvents(this);
            }

            HandDeck = new(tiles);
        }

        public PlayerYakuEvent.Upgrade OnPreUpgradeYaku(YakuType yaku, int level)
        {
            skillSet.GetLevel(yaku);
            PlayerEvents.OnPreUpgradeYakuEvent evt = new(this, yaku, level);
            EventBus.Publish(evt);
            return evt;
        }

        public bool DetermineMaterialCompatibility(Tile tile, TileMaterial mat)
        {
            PlayerEvents.DetermineMaterialCompatibilityEvent evt = new(this, tile, mat);
            EventBus.Publish(evt);
            return evt.res;
        }

        public bool DetermineFontCompatibility(Tile tile, TileFont font)
        {
            PlayerEvents.DetermineFontCompatibilityEvent evt = new(this, tile, font);
            EventBus.Publish(evt);
            return evt.res;
        }

        public bool DetermineTileCompatibility(Tile tile, int cat, int order)
        {
            PlayerEvents.DetermineTileCompatibilityEvent evt = new(this, tile, cat, order);
            EventBus.Publish(evt);
            return evt.res;
        }

        public List<Effect> GetOnOtherTileUnusedEffectsFromTile(Permutation perm, Tile tile, Tile onTile)
        {
            List<Effect> effects = new List<Effect>();
            tile.AppendToListOnTileUnusedEffect(this, perm, effects, onTile);
            return effects;
        }

        public void OnChoosePath(Direction direction, IEnumerable<Destination> destinations)
        {
            PlayerEvents.ChoosePathEvent evt = new(this, direction, destinations.ToArray());
            EventBus.Publish(evt);
        }

        public bool OnSetTransform(Tile tile, TileTransform tileTransform, Gadget gadget = null)
        {
            PlayerEvents.PreSetTransformEvent evt = new(this, gadget, tileTransform, tile);
            EventBus.Publish(evt);
            return !evt.canceled;
        }

        public List<Tile> GetInitialWall()
        {
            return GetFullDeck();
        }

        public int TileSettlingOrder(Tile t1, Permutation perm)
        {
            if (perm == null) return 0;
            int o1 = Selecting(t1) ? 1 : 0;
            if (perm.JiangFulfillAny(t => t1 == t))
            {
                o1 += 1;
            }

            return o1;
        }

        public void TriggerPreAddScoringEffectEvent(List<IAnimationEffect> inRoundAnimationQueue)
        {
            EventBus.Publish(new PlayerEvents.PreAddScoringAnimationEffectEvent(this,
                GetCurrentSelectedPerm() ?? CurrentAccumulatedBlock, inRoundAnimationQueue));
        }

        protected virtual void AddExtraScoringEffects(List<IAnimationEffect> inRoundAnimationQueue)
        {
        }
        
        public void TriggerAddExtraScoringEffects(List<IAnimationEffect> inRoundAnimationQueue)
        {
            AddExtraScoringEffects(inRoundAnimationQueue);
        }

        public List<Tile> GetSettledTiles()
        {
            if (GetCurrentSelectedPerm() == null)
            {
                return GetAccumulatedPermutation() == null ? new List<Tile>() : GetAccumulatedPermutation().ToTiles();
            }

            return GetCurrentSelectedPerm().ToTiles().Except(CurrentSelectedTiles).ToList();
        }

        public Tile RandomlyMergeTwoTile(Tile tile1, Tile tile2)
        {
            Category cat;
            int order = 0;
            if (tile1.IsNumbered() && tile2.IsNumbered())
            {
                cat = GenerateRandomInt(2) == 0 ? tile1.GetCategory() : tile2.GetCategory();
                order = GenerateRandomInt(2) == 0 ? tile1.GetOrder() : tile2.GetOrder();
            }
            else
            {
                Tile toBecome = GenerateRandomInt(2) == 0 ? tile1 : tile2;
                cat = toBecome.GetCategory();
                order = toBecome.GetOrder();
            }

            TileProperties prop = TileProperties.Plain();
            prop.material = GenerateRandomInt(2) == 0
                ? tile1.properties.material.Copy()
                : tile2.properties.material.Copy();
            prop.font = GenerateRandomInt(2) == 0 ? tile1.properties.font.Copy() : tile2.properties.font.Copy();
            prop.mask = GenerateRandomInt(2) == 0 ? tile1.properties.mask.Copy() : tile2.properties.mask.Copy();

            Tile tile = new Tile(cat, order, prop);

            tile.addonFu = Math.Max(tile1.addonFu, tile2.addonFu);

            return tile;
        }

        public List<Yaku> FindRelevantYakus(Skill.SkillType skill)
        {
            return deck.GetAvailableYakus()
                .Where(y => y.GetYakuRequiredSkills().Contains(skill)).ToList();
        }

        public int GetExtraLevel(Yaku yaku)
        {
            return skillSet.GetExtraLevel(yaku.GetYakuType());
        }

        internal void UpgradeSkill(Skill.SkillType skill)
        {
            List<Yaku> yakuList = FindRelevantYakus(skill);

            Dictionary<Yaku, int> lvBefore = new Dictionary<Yaku, int>();
            Dictionary<Yaku, int> lvAfter = new Dictionary<Yaku, int>();

            foreach (Yaku yaku in yakuList)
            {
                lvBefore.Add(yaku, skillSet.GetLevel(yaku.GetYakuType()));
            }

            skillMap.Add(skill, skillMap.Get(skill) + 1);
            foreach (Yaku yaku in yakuList)
            {
                lvAfter.Add(yaku, skillSet.GetLevel(yaku.GetYakuType()));
            }

            foreach (Yaku yaku in yakuList)
            {
                if (lvBefore[yaku] == lvAfter[yaku]) continue;
                MessageManager.Instance.OnUpgradeYakuEvent(yaku.GetYakuType(), lvBefore[yaku], lvAfter[yaku]);
            }
        }

        public virtual string GetExtraInformationFromTile(Tile tile, Func<string, string> loc)
        {
            return "";
        }

        public List<StarterBoostEffect> GetStarterBoosts()
        {
            LotteryPool<StarterBoostEffect> pool = new LotteryPool<StarterBoostEffect>();
            pool.AddRange(StarterBoostEffect.StarterBoostEffects.Where(es => es.All(e => e.IsAvailable(this)))
                .Select(es =>
                {
                    StarterBoostEffect main = LotteryPool<StarterBoostEffect>.DrawFromCollection(es, GenerateRandomInt);
                    if (main.tier == StarterBoostEffect.Tier.A) return main;
                    StarterBoostEffect side =
                        LotteryPool<StarterBoostEffect>.DrawFromCollection(StarterBoostEffect.SideEffects[main.tier],
                            GenerateRandomInt);
                    return new CombinationStarterPlayerEffect(main, side);
                }));
            List<StarterBoostEffect> res = pool.DrawRange(GenerateRandomInt, 3, false);
            
            return res;
        }

        public bool CanInsertGadgets(List<Gadget> gadgetsToInsert)
        {
            Gadget[] fakeInv = new Gadget[properties.GadgetLimit];
            for (int i = 0; i < GetGadgets().Count; i++)
            {
                fakeInv[i] = GetGadgets()[i].Copy();
            }

            foreach (var toAdd in gadgetsToInsert.Select(g => g.Copy()))
            {
                for (int i = 0; i < fakeInv.Length; i++)
                {
                    Gadget existing = fakeInv[i];
                    if (existing == null)
                    {
                        fakeInv[i] = toAdd;
                        break;
                    }

                    if (existing.regName.Equals(toAdd.regName) && existing.uses < existing.GetStackLimit())
                    {
                        int transferAmount = Math.Min(existing.GetStackLimit() - existing.uses, toAdd.uses);
                        existing.uses += transferAmount;
                        toAdd.uses -= transferAmount;
                        return true;
                    }

                    if (toAdd.uses <= 0)
                    {
                        break;
                    }

                    if (i == fakeInv.Length - 1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CanInsertArtifacts(List<Artifact> artifactsToInsert)
        {
            int insertAmount = properties.ArtifactLimit - GetArtifacts().Count;
            return insertAmount >= artifactsToInsert.Count;
        }

        public void OnChangeMaterial(Tile tile, TileMaterial newMaterial, bool isCopy)
        {
            EventBus.Publish(new PlayerEvents.PreSetMaterialEvent(this, tile, newMaterial, isCopy));
        }

        public void OnChangeFont(Tile tile, TileFont newFont, bool isCopy)
        {
            EventBus.Publish(new PlayerEvents.PreSetFontEvent(this, tile, newFont, isCopy));
        }

        public bool OnChangeMask(Tile tile, TileMask newMask, bool isCopy)
        {
            PlayerEvents.PreSetMaskEvent evt = new(this, tile, newMask, isCopy);
            EventBus.Publish(evt);
            return evt.canceled;
        }

        /// <summary>
        /// SetMat不触发
        /// </summary>
        public void OnchangeProperties(Tile tile, TileProperties toBecome, bool isCopy)
        {
            EventBus.Publish(new PlayerEvents.PreSetPropertiesEvent(this, tile, toBecome, isCopy));
        }
        
        /// <summary>
        /// SetMat、SetFont等都会触发
        /// </summary>
        public void PreChangedProperties(Tile tile, TileProperties newProperties)
        {
            EventBus.Publish(new PlayerEvents.PreSetTilePropertiesEvent(this, tile, newProperties, false));
        }

        public virtual int GetMaxPlayingStage()
        {
            return 4;
        }

        public Block GenerateRandomBlock()
        {
            List<Tile> tiles = GetUniqueFullDeck();
            int Rand(int max) => GenerateRandomInt(max, "random_block");
            Tile generator = tiles[Rand(tiles.Count)];

            bool isSequence = generator.IsNumbered() && Rand(4) <= 2;

            Category category = generator.GetCategory();
            int order = generator.GetOrder();

            if (isSequence)
            {
                List<Block> candBlocks = new List<Block>();
                for (int i = 0; i < 3; i++)
                {
                    if (order - 2 + i < 1 || order + i > 9)
                    {
                        continue;
                    }

                    candBlocks.Add(new Block(new[]
                    {
                        new Tile(category, order - 2 + i), new Tile(category, order - 1 + i),
                        new Tile(category, order + i)
                    }));
                }

                return candBlocks[Rand(candBlocks.Count)];
            }

            return new Block(new[] { new Tile(category, order), new Tile(category, order), new Tile(category, order) });
        }

        public virtual void AppendOnRoundEndEffect(List<IAnimationEffect> onRoundEndEffects)
        {
            foreach (Artifact artifact in GetArtifacts())
            {
                artifact.AddOnRoundEndEffects(this, GetAccumulatedPermutation(), onRoundEndEffects);
            }

            AppendOnTileRoundEndEffect(onRoundEndEffects);
            AppendAdditionalTileRoundEndEffects(onRoundEndEffects, GetAccumulatedPermutation());
            TriggerOnAddRoundEndAnimationEffectEvent(onRoundEndEffects);
        }

        // Shared by the legacy and deferred round-end pipelines. Deck-specific
        // tiles (such as played flowers) live outside the hand and permutation.
        public virtual void AppendAdditionalTileRoundEndEffects(List<IAnimationEffect> effects, Permutation permutation)
        {
        }

        public void TriggerOnAddRoundEndAnimationEffectEvent(List<IAnimationEffect> onRoundEndEffects)
        {
            EventBus.Publish(new PlayerEvents.OnPostAddRoundEndAnimationEffectEvent(this,
                GetCurrentSelectedPerm() ?? CurrentAccumulatedBlock, onRoundEndEffects));
        }

        protected virtual void AppendOnTileRoundEndEffect(List<IAnimationEffect> onRoundEndEffects)
        {
            Permutation perm = GetAccumulatedPermutation();
            foreach (Tile tile in GetHandDeckCopy())
            {
                tile.AppendOnRoundEndEffects(this, perm, onRoundEndEffects);
            }

            if (perm == null) return;
            {
                foreach (Tile tile in perm.ToTiles())
                {
                    tile.AppendOnRoundEndEffects(this, perm, onRoundEndEffects);
                }
            }
        }

        public virtual void AppendDiscardTileEffect(List<IAnimationEffect> onDiscardTileEffects, Tile tile,
            bool withForce, bool isClone)
        {
            tile.AppendDiscardEffects(this, GetAccumulatedPermutation(), onDiscardTileEffects, withForce, tile,
                isClone);
            foreach (Artifact artifact in GetArtifacts())
            {
                artifact.AppendDiscardTileEffects(this, tile, onDiscardTileEffects, withForce, isClone);
            }

            TriggerOnAddDiscardTileAnimationEffectEvent(onDiscardTileEffects, tile, withForce);
        }

        public void TriggerOnAddDiscardTileAnimationEffectEvent(List<IAnimationEffect> onDiscardTileEffects, Tile tile, bool withForce)
        {
            EventBus.Publish(new PlayerEvents.OnAddSingleDiscardTileAnimationEffectEvent(this,
                onDiscardTileEffects, tile, withForce));
        }

        public bool OnPreModifyCarvedDesign(Tile t, Category newCat, int newOrd)
        {
            var evt = new PlayerModifyCarvedDesignEvent.Pre(t, newCat, newOrd, this);
            EventBus.Publish(evt);
            return !evt.canceled;
        }

        public void OnPostModifyCarvedDesign(Tile tile, Category cat, int order)
        {
            EventBus.Publish(new PlayerModifyCarvedDesignEvent.Post(tile, cat, order, this));
        }

        public void TriggerDessertTileConsumedEvent(Tile tile, TileMaterialDessert dessert)
        {
            EventBus.Publish(new PlayerEvents.OnDessertTileConsumedEvent(this, tile, dessert));
        }

        public bool TriggerDessertTileConsumeAttemptEvent(Tile tile, TileMaterialDessert dessert)
        {
            var evt = new PlayerEvents.OnDessertTileConsumeAttemptEvent(this, tile, dessert);
            EventBus.Publish(evt);
            return !evt.canceled;
        }

        public bool IsArtifactDebuffed(Artifact artifact)
        {
            if (artifact.IsDebuffed) return true;
            if (!(GetArtifacts().Contains(Artifacts.ClimbingKit) || GetArtifacts().Contains(Artifacts.TravelBag)))
                return false;
            return GetArtifacts().IndexOf(artifact) <= 2;
        }

        public Boss GetNextBoss()
        {
            int index = (Level / 4);
            while (index >= upcomingBosses.Count) GenerateNewUpcomingBosses();
            return Bosses.GetPreviewOrElseRedraw(upcomingBosses[index], HarderBossesEnabled());
        }

        public Boss GetBossAtRound(int prevalentWind)
        {
            int pluses = (Level - 1) / 16;
            int index = 4 * pluses + prevalentWind - 1;
            while (index >= upcomingBosses.Count) GenerateNewUpcomingBosses();
            return Bosses.GetPreviewOrElseRedraw(upcomingBosses[index], HarderBossesEnabled());
        }

        public void SyncSelectingTiles(List<Tile> tiles)
        {
            CurrentSelectedTiles = new(tiles);
        }

        public void SetGadgetLimit(int v)
        {
            properties.GadgetLimit = v;
        }

        public void ReplaceYakuPack(int from, int to)
        {
            var oldList = properties.YakuPacks;
            int[] newList = oldList.Select(x => x == from ? to : x).ToArray();
            properties.YakuPacks = newList;
        }

        public void UpgradeYakuPack(YakuPack from, YakuPack to)
        {
            ReplaceYakuPack(from.id, to.id);
            foreach (var yaku in to.GetYakuPool(this).Select(y => y.GetYakuType()).Distinct())
            {
                UpgradeYaku(yaku, 1);
            }
        }

        public bool DetermineYaojiu(Tile tile)
        {
            PlayerEvents.DetermineYaojiuTileEvent evt = new(this, tile);
            evt.canceled =
                !(tile.IsHonor(this) || (tile.IsNumbered() && (tile.GetOrder() == 1 || tile.GetOrder() == 9)));
            EventBus.Publish(evt);
            return !evt.canceled;
        }

        public bool DetermineHonor(Tile tile)
        {
            return tile.GetCategory() == Category.Jian || tile.GetCategory() == Category.Feng;
        }

        public bool DetermineShiftedPair(Block b1, Block b2, int step, bool categorySensitive)
        {
            PlayerEvents.DetermineShiftedPairEvent evt = new(this, b1, b2, step, categorySensitive,
                GetCombinator().ASuccB(b2, b1, categorySensitive, step));
            EventBus.Publish(evt);
            return evt.res;
        }

        public bool IsPlayerWind(int v)
        {
            PlayerEvents.DeterminePlayerWindEvent evt = new(this);
            evt.message = v.ToString();
            evt.canceled = v != GetPlayerWind();
            EventBus.Publish(evt);
            return !evt.canceled;
        }

        public bool IsPrevalentWind(int v)
        {
            PlayerEvents.DeterminePrevalentWindEvent evt = new(this);
            evt.message = v.ToString();
            evt.canceled = v != GetPrevalentWind();
            EventBus.Publish(evt);
            return !evt.canceled;
        }

        public virtual bool GenerateRandomDeterminationResult(int v)
        {
            return GenerateRandomInt(v) == 0;
        }

        public virtual List<Tile> GetUniqueFullDeck()
        {
            return Hand.PlainUniqueHand().tiles;
        }

        public virtual List<Tile> GetFullDeck()
        {
            return Hand.PlainFullHand().tiles;
        }

        public bool HarderBossesEnabled()
        {
            return ascensionLevel >= 8;
        }

        public virtual bool CanSelectTile(Tile tile)
        {
            PlayerEvents.DetermineTileSelectivityEvent evt = new(this, tile);
            EventBus.Publish(evt);
            return !evt.canceled;
        }

        public void TriggerPreSettlePermutationEvent()
        {
            EventBus.Publish(new PlayerEvents.PreSettlePermutationEvent(this, GetCurrentSelectedPerm()));
        }

        public virtual void TriggerPostSettlePermutationEvent(Permutation permutation)
        {
            EventBus.Publish(new PlayerEvents.PostSettlePermutationEvent(this, permutation));
        }
        
        public void TriggerPreAppendSettleScoringEffectsEvent()
        {
            EventBus.Publish(new PlayerEvents.PreAppendSettleScoringEffectsEvent(this, GetCurrentSelectedPerm()));
        }

        public void TriggerOnAddSingleTileAnimationEffectEvent(Permutation perm, List<OnTileAnimationEffect> tileAnimationQueue, OnTileAnimationEffect eff, Tile tile)
        {
            EventBus.Publish(new PlayerEvents.PostAddSingleTileAnimationEffectEvent(this, perm,
                tileAnimationQueue, eff, tile));
        }
      
        #region 指令

        [AotenjoCommand("setMat", nameof(ArgumentParsers.ToHandTiles), nameof(ArgumentParsers.ToTileMaterial))]
        public void SetMaterial(Tile[] tiles, TileMaterial material)
        {
            foreach (var tile in tiles)
            {
                tile.SetMaterial(material.Copy(),this);
            }
        }
        
        [AotenjoCommand("destroyWall")]
        public void DestroyWall() => GetTilePool().ForEach(t => RemoveTileFromPool(t));
        
        [AotenjoCommand("setFont", nameof(ArgumentParsers.ToHandTiles), nameof(ArgumentParsers.ToTileFont))]
        public void SetFont(Tile[] tiles, TileFont font)
        {
            foreach (var tile in tiles)
            {
                tile.SetFont(font.Copy(),this);
            }
        }
        
        [AotenjoCommand("copyTile", nameof(ArgumentParsers.ToHandTiles))]
        public void CopyTile(Tile[] tiles)
        {
            foreach (var tile in tiles)
            {
                AddTileToPool(tile.Copy());
            }
        }
        
        [AotenjoCommand("addTiles", nameof(ArgumentParsers.ToPlainTiles))]
        public void AddTiles(Tile[] tiles)
        {
            foreach (var tile in tiles)
            {
                AddTileToPool(tile);
            }
        }

        #endregion

        public double GetFanForYaku(YakuType yakuType, Permutation permutation)
        {
            double baseFan = YakuTester.GetFan(yakuType, permutation.IsFullHand(this) ? GetHandLimit() : permutation.blocks.Count(), GetSkillSet());
            double multiplier = GetYakuMultiplier(yakuType);
            return baseFan * multiplier;
        }

        public virtual double GetYakuMultiplier(YakuType yakuType)
        {
            PlayerEvents.RetrieveYakuMultiplierEvent evt = new(this, yakuType, 1.0D);
            EventBus.Publish(evt);
            return evt.canceled ? 0.0D : evt.multiplier;
        }

        public void UpgradeYakuFromYakuPack(YakuPackConsumeResult res)
        {
            foreach (var yaku in res.yakus)
            {
                UpgradeYaku(yaku, 1);
            }
            PostReadIBook(res.yakuPack, res.yakus.Select(y => YakuTester.InfoMap[y]).ToList());
        }

        public void PostReadIBook(IBook book, List<Yaku> drawnYakus)
        {
            var evt = new PlayerEvents.PostUpgradeYakuFromIBookEvent(this, drawnYakus.ToArray(), book);
            EventBus.Publish(evt);
        }

        public int GetYakuPackResultCount()
        {
            //TODO: 改
            if (GetArtifacts().Contains(Artifacts.Magnifier))
            {
                return 5;
            }
            return 3;
        }

        public void PostUpgradeJade(IJade jade)
        {
            throw new NotImplementedException();
        }

        public int GetEffectiveJadeStack(IJade jade)
        {
            PlayerEvents.RetrieveEffectiveJadeStackEvent evt = new(this, jade, jade.GetLevel(this));
            EventBus.Publish(evt);
            return evt.effectiveStack;
        }

        public bool DetermineNeighborArtifacts(Artifact artifactLeft, Artifact right)
        {
            var artifacts = GetArtifacts();
            if (!artifacts.Contains(artifactLeft) || !artifacts.Contains(right)) return false;
            return artifacts.IndexOf(right) - 1 == artifacts.IndexOf(artifactLeft);
        }

        public void TriggerOnAddSingleTileScoringEffectEvent(List<IAnimationEffect> effects, Tile tile, Permutation permutation)
        {
            EventBus.Publish(new PlayerEvents.OnAddSingleTileScoringEffectEvent(this, permutation, effects, tile));
        }

        public int GetYakuPackPrice(IBook yakuPack)
        {
            int basePrice = 3;
            if(yakuPack is YakuPack pack && pack.id >= 4)
            {
                basePrice += 2;
            }
            if (GetArtifacts().Contains(Artifacts.Magnifier))
            {
                return basePrice + 1;
            }
            return basePrice;
        }

        public double GetBaseFuOfTile(Tile tile)
        {
            double fu = tile.GetBaseFu();
            var evt = new PlayerTileEvent.RetrieveBaseFu(this, tile, fu);
            EventBus.Publish(evt);
            return evt.baseFu;
        }

        public virtual List<IAnimationEffect> GetBaseEffectFromTile(Tile tile)
        {
            return new List<IAnimationEffect>()
                { ScoreEffect.AddFu(() => GetBaseFuOfTile(tile), null).HideWhenZero().OnTile(tile) };
        }

        public virtual bool EraseBlock(Block block)
        {
            Permutation perm = GetAccumulatedPermutation();
            if(perm == null) return false;
            perm.blocks = perm.blocks.Except(new[] { block }).ToArray();
            if (!perm.blocks.Any() || perm.GetPermType() == PermutationType.SEVEN_PAIRS || perm.GetPermType() == PermutationType.EXTENDED_THIRTEEN_ORPHANS || perm.GetPermType() == PermutationType.LIGULIGU)
                SetCurrentAccumulatedBlock(null);
            bool needUnfreeze = CurrentPlayingStage == GetMaxPlayingStage();
            CurrentPlayingStage--;
            if (needUnfreeze)
            {
                MessageManager.Instance.OnUnfreezeEvent(this);
            }

            return true;
        }

        public virtual void PostRoundStart()
        {
        }

        public List<(YakuPack, YakuPack)> GenerateYakuPackUpgradeOptions(YakuPack[] globalTable)
        {
            var nativePacks = properties.YakuPacks.Where(id => id < 4).Select(id => globalTable[id]).ToList();
            var upgradedPacks = properties.YakuPacks.Where(id => id >= 4).Select(id => globalTable[id]).ToList();
            var upgradedPacksPool = new LotteryPool<YakuPack>();
            upgradedPacksPool.AddRange(upgradedPacks);
            var fromPool = new HasFallbackLotteryPool<YakuPack>(upgradedPacksPool);
            fromPool.AddRange(nativePacks);
            
            var res = new List<(YakuPack, YakuPack)>();
            
            for (int i = 0; i < 2; i++)
            {
                if (fromPool.IsEmpty()) throw new ArgumentException("No available YakuPack to upgrade");
                YakuPack from = fromPool.Draw(GetRng("YakuPackUpgrade"));
                var toPool = new LotteryPool<YakuPack>();
                
                //若为初始番种包，只能升级为相同种类的升级包；否则可以升级为任意非初始番种包
                bool fromNativePack = from.id < 4;
                toPool.AddRange(globalTable.Where(pack =>
                    pack.GetYakuPool(this).Any() &&
                    pack.id >= 4 && 
                    (!fromNativePack || pack.id % 4 == from.id))
                );

                if (toPool.IsEmpty()) throw new ArgumentException("No available YakuPack to upgrade");
                YakuPack to = toPool.Draw(GetRng("YakuPackUpgrade"));
                res.Add((from, to));
            }

            return res;
        }
    }
}
