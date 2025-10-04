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

        public Boss currentBoss;

        private Boss nextBoss;

        [SerializeField] public List<string> generalBossesNamePool =
            Bosses.BossList.Where(b => !Bosses.FinalBossList.Contains(b)).Select(b => b.name).ToList();

        [SerializeField] public List<string> terminalBossesNamePool = Bosses.FinalBossList.Select(b => b.name).ToList();

        [SerializeField] public List<string> upcomingBosses = new List<string>();

        [SerializeField] public int playMode;

        [SerializeReference] public MahjongDeck deck;

        [SerializeReference] public MaterialSet materialSet = MaterialSet.Basic;

        [SerializeField] public bool won;

        [SerializeField] public string randomSeed;

        [SerializeField] public int ascensionLevel;

        [SerializeField] public bool usedBoost;

        [SerializeField] public bool seededRun;

        [SerializeField] public bool inRound = true;

        [SerializeField] public bool stillInTutorial;


        [SerializeField] public bool tutorialFirstDrawArtifact = true;
        
        //计分相关
        public Stack<IAnimationEffect> playHandEffectStack;
        public Stack<IAnimationEffect> roundEndEffectsStack;
        public Stack<IAnimationEffect> discardTileEffectsStack;

        #endregion

        #region 事件

        public delegate void PlayerEventListener(PlayerEvent playerEvent);

        public delegate void PlayerPermutationEventListener(PlayerPermutationEvent permutationEvent);

        public delegate void PlayerTileEventListener(PlayerTileEvent tileEvent);

        public delegate void PlayerPreDrawTileEventListener(PlayerDrawTileEvent.Pre drawTileEvent);

        public delegate void PlayerPostDrawTileEventListener(PlayerDrawTileEvent.Post drawTileEvent);

        public delegate void PlayerPreDiscardTileEventListener(PlayerDiscardTileEvent.Pre discardTileEvent);

        public delegate void PlayerPostDiscardTileEventListener(PlayerDiscardTileEvent.Post discardTileEvent);

        public delegate void PlayerYakuObtainEventListener(PlayerYakuEvent.Obtain yakuEvent);

        public delegate void PlayerYakuUpgradeEventListener(PlayerYakuEvent.Upgrade yakuEvent);

        public delegate void PlayerYakuDeleteEventListener(PlayerYakuEvent.Delete yakuEvent);
        
        public delegate void PlayerRetrieveYakuMultiplierEventListener(PlayerYakuEvent.RetrieveMultiplier yakuEvent);

        public delegate void PlayerObtainGadgetEvent(Player player, Gadget gadget);

        public delegate void PlayerArtifactEventListener(PlayerArtifactEvent evt);

        public delegate void PlayerUpgradeYakuEventListener(PlayerYakuEvent.Upgrade evt);

        public delegate void DetermineMaterialCompactbilityEventListener(PlayerDetermineMaterialCompactibilityEvent evt);

        public delegate void DetermineFontCompactbilityEventListener(PlayerDetermineFontCompactbilityEvent evt);

        public delegate void PlayerGadgetEventListener(PlayerGadgetEvent evt);

        public delegate void PlayerSetTransformEventListener(PlayerSetTransformEvent evt);

        public delegate void PlayerSetAttributeEventListener(PlayerSetAttributeEvent evt);

        public delegate void PlayerSetPropertiesEventListener(PlayerSetPropertiesEvent evt);

        public delegate void PlayerSpendMoneyEventListener(PlayerMoneyEvent evt);

        public delegate void PlayerEarnMoneyEventListener(PlayerMoneyEvent evt);

        public delegate void DetermineTileCompactbilityEventListener(PlayerDetermineTileFaceCompactbilityEvent evt);

        public delegate void ChoosePathEventListener(PlayerChoosePathEvent evt);

        #region 游戏生命周期事件
        
        public event PlayerEventListener PreRoundStartEvent;
        public event PlayerEventListener PostRoundStartEvent;
        public event PlayerEventListener PrePreRoundEndEvent;
        public event PlayerEventListener PostPreRoundEndEvent;
        public event PlayerEventListener PreRoundEndEvent;
        public event PlayerEventListener PostRoundEndEvent;
        public event PlayerEventListener PreSkipRoundEvent;
        public event PlayerEventListener PostSkipRoundEndEvent;
        
        //Player, Winning
        public event Action<Player, bool> OnEndRunEvent;
        public event Action<Player, bool, PlayerStats> PostRunEndEvent;

        #endregion


        public event PlayerPermutationEventListener PreSettlePermutationEvent;
        public event PlayerPermutationEventListener PreAppendSettleScoringEffectsEvent;
        public event PlayerPermutationEventListener PostSettlePermutationEvent;
        public event Action<Permutation, Player, List<IAnimationEffect>> OnPrePostAddOnTileAnimationEffectEvent;
        public event Action<Permutation, Player, List<OnTileAnimationEffect>> OnPostAddOnTileAnimationEffectEvent;
        public event Action<Permutation, Player, List<IAnimationEffect>, Tile> OnAddSingleTileScoringEffectEvent;

        public event System.Action<Permutation, Player, List<OnTileAnimationEffect>, OnTileAnimationEffect, Tile> PostAddSingleTileAnimationEffectEvent;

        public event Action<Permutation, Player, List<IAnimationEffect>> PreAddScoringAnimationEffectEvent;
        public event Action<Permutation, Player, List<IAnimationEffect>> OnPostAddOnBlockAnimationEffectEvent;
        public event Action<Permutation, Player, List<IAnimationEffect>> OnPostAddScoringAnimationEffectEvent;
        public event Action<Permutation, Player, List<IAnimationEffect>> OnPostAddRoundEndAnimationEffectEvent;
        public event Action<Player, List<IAnimationEffect>, IAnimationEffect> OnAddSingleAnimationEffectEvent;
        public event Action<Player, List<IAnimationEffect>, Tile, bool> OnAddSingleDiscardTileAnimationEffectEvent;
        public event Action<Permutation, Player, Effect> PostIngestEffect;
        public event PlayerUpgradeYakuEventListener OnPreUpgradeYakuEvent;

        public event PlayerSpendMoneyEventListener SpendMoneyEvent;
        public event PlayerEarnMoneyEventListener EarnMoneyEvent;

        public event PlayerArtifactEventListener PreRemoveArtifact;

        public event PlayerTileEventListener PreAddTileEvent;
        public event PlayerTileEventListener PostAddTileEvent;

        public event PlayerYakuObtainEventListener ObtainYakuEvent;
        public event PlayerYakuUpgradeEventListener UpgradeYakuEvent;
        public event PlayerYakuDeleteEventListener DeleteYakuEvent;
        public event PlayerRetrieveYakuMultiplierEventListener RetrieveYakuMultiplierEvent;

        public event PlayerObtainGadgetEvent ObtainGadgetEvent;
        public event Action<PlayerArtifactEvent.DetermineGettability> PreObtainArtifactEvent;
        public event PlayerArtifactEventListener PostObtainArtifactEvent;
        public event PlayerGadgetEventListener PostUseGadgetEvent;
        public event PlayerSetTransformEventListener PreSetTransformEvent;

        public event PlayerSetAttributeEventListener PreSetMaterialEvent;
        public event PlayerSetAttributeEventListener PreSetFontEvent;
        public event PlayerSetAttributeEventListener PreSetMaskEvent;
        public event Action<Player, Tile, Category, int> PostModifyTileCarvatureEvent;
        public event PlayerSetPropertiesEventListener PreSetPropertiesEvent;
        public event PlayerSetPropertiesEventListener PreSetTilePropertiesEvent;

        public event PlayerPreDrawTileEventListener PreDrawTileEvent;
        public event PlayerPostDrawTileEventListener PostDrawTileEvent;

        public event PlayerTileEventListener DetermineTileSelectivityEvent;
        public event Action<PlayerKongTileEvent> PreKongTileEvent;
        public event DetermineMaterialCompactbilityEventListener DetermineMaterialCompactbilityEvent;
        public event DetermineFontCompactbilityEventListener DetermineFontCompactbilityEvent;
        public event DetermineTileCompactbilityEventListener DetermineTileCompactbilityEvent;

        public event Action<PlayerDiscardTileEvent.Determine> DetermineDiscardTileEvent;
        public event Action<PlayerDiscardTileEvent.DetermineForce> DetermineForceDiscardTileEvent;
        public event Action<DeterminePlayerSelectingTileEvent> DetermineSelectingTileEvent;
        public event Action<PlayerDiscardTileEvent.Pre> PreDiscardTileEvent;
        public event PlayerPostDiscardTileEventListener PostDiscardTileEvent;

        public event PlayerTileEventListener PreRemoveTileEvent;
        public event PlayerTileEventListener PostRemoveTileEvent;
        public event PlayerTileEventListener DetermineYaojiuTileEvent;
        public event Action<PlayerDetermineShiftedPairEvent> DetermineShiftedPairEvent;

        public event Action<Player, List<Destination>> PostGenerateDestinationEvent;
        public event ChoosePathEventListener ChoosePathEvent;

        public event PlayerEventListener DeterminePlayerWindEvent;
        public event PlayerEventListener DeterminePrevalentWindEvent;

        // 甜品牌被消耗完毕事件
        public event Action<Player, Tile, TileMaterialDessert> OnDessertTileConsumedEvent;
        
        // 甜品牌消耗尝试事件（可以被阻止）
        public event PlayerEventListener OnDessertTileConsumeAttemptEvent;
        public event Action<PlayerYakuEvent.ReadBookResult> PostUpgradeYakuFromIBookEvent;
        public event Action<PlayerJadeEvent.RetrieveEffectiveStack> RetrieveEffectiveJadeStackEvent;

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
            if (!artifacts.Any())
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

        public void OnEnterGame()
        {
            CachedSelectedPermutation = null;
            CurrentAccumulatedBlock = null;
            skillSet.SetPlayer(this);
        }

        #endregion

        public virtual bool HasExtraInfo()
        {
            return false;
        }

        public SkillSet GetSkillSet()
        {
            return skillSet;
        }

        public YakuType[] GetYakus()
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

        public void ApplyEffect(Effect effect)
        {
            effect.Ingest(this);

            if(effect.WillTrigger())
                PostIngestEffect?.Invoke(GetCurrentSelectedPerm() ?? CurrentAccumulatedBlock, this, effect);

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
            GetArtifacts().ForEach(a => a.AddOnSelfEffects(this, permutation, effects));
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

        public int GetLevelBaseBonusMoney()
        {
            return Level % 4 == 0 ? 12 : 4;
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

        /// <summary>
        /// 已弃用
        /// </summary>
        /// <returns></returns>
        public int GetGoldenOrizuruBonusMoney()
        {
            return 0;
        }

        public bool DetermineForceDiscard(Tile tile)
        {
            bool rawRes = false;
            PlayerDiscardTileEvent.DetermineForce evt = new(this, tile, rawRes);
            DetermineForceDiscardTileEvent?.Invoke(evt);
            return evt.res;
        }

        public bool CanDiscardTile(Tile tile, bool forceDiscard, bool consumeDiscardChance)
        {
            bool rawRes = !consumeDiscardChance || DiscardLeft > 0;
            PlayerDiscardTileEvent.Determine evt = new(this, tile, rawRes, forceDiscard, consumeDiscardChance);
            DetermineDiscardTileEvent?.Invoke(evt);
            if (evt.canceled) return false;
            return evt.res;
        }

        public bool PreDiscardTile(Tile tile, bool forced)
        {
            PlayerDiscardTileEvent.Pre preEvent = new(this, tile, true);
            preEvent.forced = forced;
            PreDiscardTileEvent?.Invoke(preEvent);
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

            PlayerDiscardTileEvent.Post postEvent = new(this, tile);
            PostDiscardTileEvent?.Invoke(postEvent);
            if (postEvent.canceled) return -2;
            return Pos;
        }

        public int MoveFromHandToDiscard(Tile tile)
        {
            int Pos = HandDeck.IndexOf(tile);
            HandDeck.Remove(tile);

            Discarded.Add(tile);

            return Pos;
        }

        public int MoveFromDiscardToPool(Tile tile)
        {
            int Pos = Discarded.IndexOf(tile);
            Discarded.Remove(tile);
            TilePool.Add(tile);
            return Pos;
        }

        public int MoveFromHandToPool(Tile tile)
        {
            int Pos = HandDeck.IndexOf(tile);
            HandDeck.Remove(tile);
            TilePool.Add(tile);
            return Pos;
        }

        public List<Tile> priortizedDrawingList = new List<Tile>();

        public void AddPrioritizedDrawingTile(Tile tile)
        {
            Tile cand = GetTilePool().Except(priortizedDrawingList).FirstOrDefault(t => t.CompactWith(tile));
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
            int Pos = GenerateRandomInt(TilePool.Count, "draw_tile");
            Tile tile = TilePool[Pos];

            if (priortizedDrawingList.Count > 0)
            {
                tile = priortizedDrawingList[0];
                priortizedDrawingList.Remove(tile);
            }

            PlayerDrawTileEvent.Pre preEvent = new(this, tile);
            PreDrawTileEvent?.Invoke(preEvent);
            TilePool.Remove(tile);

            HandDeck.Add(tile);
            if (sortDeck)
                SortDeck();

            PlayerDrawTileEvent.Post postEvent = new(this, tile);
            PostDrawTileEvent?.Invoke(postEvent);
            return HandDeck.IndexOf(tile);
        }

        /// <summary>
        /// 不排序手牌的弃牌函数
        /// </summary>
        /// <param name="tile">需要弃置的手牌</param>
        /// <returns>牌插入手中的位置，如果牌库没牌了，返回-1，如果事件被取消了，返回-2</returns>
        public int ReplaceTileAndKeepPosition(Tile tile)
        {
            PlayerDiscardTileEvent.Pre preEvent = new(this, tile, true);
            PreDiscardTileEvent?.Invoke(preEvent);
            if (preEvent.canceled) return -2;

            stats.RecordCustomStats("discard", 1);
            if (tile.IsYaoJiu(this)) stats.RecordCustomStats("discard_yaojiu", 1);

            int Pos = HandDeck.IndexOf(tile);
            Discarded.Add(tile);

            PostDiscardTileEvent?.Invoke(new(this, tile));

            if (TilePool.Count == 0)
            {
                HandDeck[Pos] = null;
                return -1;
            }

            //Randomly get a cand pos from the pool
            int toDrawPos = GenerateRandomInt(TilePool.Count);
            Tile toDraw = TilePool[toDrawPos];

            PlayerDrawTileEvent.Pre preDrawEvent = new(this, tile);
            PreDrawTileEvent?.Invoke(preDrawEvent);
            TilePool.Remove(toDraw);

            HandDeck[Pos] = toDraw;

            PlayerDrawTileEvent.Post postDrawEvent = new(this, tile);
            PostDrawTileEvent?.Invoke(postDrawEvent);
            return Pos;
        }

        public void SortDeck()
        {
            HandDeck = HandDeck.Where(e => e != null).ToList();
            HandDeck.Sort();
        }

        public bool RemoveTileFromDiscarded(Tile toRemove, string message = "")
        {
            PlayerTileEvent evt = new PlayerTileEvent(this, toRemove);
            evt.message = message;

            PreRemoveTileEvent?.Invoke(evt);
            if (evt.canceled) return false;
            bool res = Discarded.Remove(toRemove);
            toRemove.UnsubscribeFromPlayer(this);

            PostRemoveTileEvent?.Invoke(evt);

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
            PlayerTileEvent evt = new PlayerTileEvent(this, toRemove);
            PreRemoveTileEvent?.Invoke(evt);

            if (evt.canceled) return false;

            bool res = TilePool.Remove(toRemove);
            toRemove.UnsubscribeFromPlayer(this);

            PostRemoveTileEvent?.Invoke(evt);
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

            PlayerTileEvent evt = new PlayerTileEvent(this, toRemove);
            PreRemoveTileEvent?.Invoke(evt);

            if (evt.canceled) return false;

            bool res = HandDeck.Remove(toRemove);
            toRemove.UnsubscribeFromPlayer(this);

            if (destroyed)
            {
                PostRemoveTileEvent?.Invoke(evt);
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
            PlayerTileEvent preAddTileEvt = new PlayerTileEvent(this, toAdd);
            PreAddTileEvent?.Invoke(preAddTileEvt);
            if (preAddTileEvt.canceled) return false;
            toAdd.SubscribeToPlayerEvents(this);
            TilePool.Add(toAdd);
            PlayerTileEvent postAddTileEvt = new PlayerTileEvent(this, toAdd);
            PostAddTileEvent?.Invoke(postAddTileEvt);
            EventManager.Instance.OnAddTileEvent(new List<Tile> { toAdd });
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
            PreSettlePermutationEvent?.Invoke(new PlayerPermutationEvent(this, GetAccumulatedPermutation()));
            
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
            DiscardLeft += 2 * blocks.Count(b => b.IsAAAA());
            blocks.Where(b => b.IsAAAA()).ToList().ForEach(b => OnKong(b, perm));
            SetCurrentAccumulatedBlock(perm ?? throw new ArgumentNullException());

            List<YakuType> yakuTypes = perm.GetYakus(this, true).Where(y => !nativeYakus.Contains(y)).ToList();

            yakuTypes.ForEach(yaku => skillSet.TryUnlockYakuIfLocked(yaku, perm.IsFullHand()));

            //Score is now calculated in animations, stacked in `RoundAccumulatedScore`
            Score score = RoundAccumulatedScore;
            CurrentAccumulatedScore += Math.Floor(score.GetScore());
            RecordCurrentHand(perm, score);

            stats.RecordPlay(perm, this, perm.GetYakus(this, true).Where(a => skillSet.GetLevel(a) > 0).ToList(),
                (Score)RoundAccumulatedScore.Clone());
            stats.SyncPlayer(this);
            PostSettlePermutationEvent?.Invoke(new(this, perm));
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
            PreSkipRoundEvent?.Invoke(new(this));
            SkipCount++;
            DiscardLeft += 10;
            CurrentPlayingStage++;
            PostSkipRoundEndEvent?.Invoke(new(this));
        }

        public bool OnRoundEndButtonPressed()
        {
            PlayerEvent prePreRoundEndEvent = new(this);
            PrePreRoundEndEvent?.Invoke(prePreRoundEndEvent);

            if (prePreRoundEndEvent.canceled) return false;

            PostPreRoundEndEvent?.Invoke(new(this));
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
                PreRoundEndEvent?.Invoke(new PlayerEvent(this));
                SettleMoney();
                ResetTilePool();

                PostRoundEndEvent?.Invoke(new PlayerEvent(this));
                ResetScore();
                Level++;

                if (Level == 5 && GetAscensionLevel() >= 3)
                {
                    properties.DiscardLimit -= 5;
                }

                if (Level % 4 == 0)
                {
                    while (upcomingBosses.Count < Level / 4)
                        GenerateNewUpcomingBosses();
                    SetCurrentBoss(Bosses.GetBossOrElseRedraw(upcomingBosses[(Level / 4) - 1], HarderBossesEnabled()));
                }
                else if (currentBoss != null)
                {
                    stats.RecordCustomStats($"encounter_boss_{currentBoss.name}", 1);
                    ResetBoss();
                }

                if (Level % 4 == 1)
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
            if (currentBoss != null)
                currentBoss.UnsubscribeFromPlayerEvents(this);
            currentBoss = null;
            currentBossName = null;
        }

        public void EncounterNextBoss(Boss nextB)
        {
            nextBoss = nextB;
            nextBossName = nextB.name;
        }

        public void SetCurrentBoss(Boss boss)
        {
            ResetBoss();
            currentBoss = boss;
            currentBossName = boss.name;
            upcomingBosses[(Level / 4) - 1] = boss.name;
            boss.SubscribeToPlayerEvents(this);
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

                return Bosses.GetBossOrElseRedraw(generalBossesNamePool[GenerateRandomInt(generalBossCount, "boss")], harderBossesEnabled);
            }

            Boss result;
            if (finalRound)
            {
                result = Bosses.GetBossOrElseRedraw(
                    terminalBossesNamePool[GenerateRandomInt(terminalBossCount, "boss")], harderBossesEnabled);
            }
            else
            {
                result = Bosses.GetBossOrElseRedraw(generalBossesNamePool[GenerateRandomInt(generalBossCount, "boss")], harderBossesEnabled);
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

            PlayerMoneyEvent evt = new(this, money);
            EarnMoneyEvent?.Invoke(evt);
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
            if (hand.GetPerms(this, playMode).Count == 0) return null;

            bool firstHand = GetAccumulatedPermutation() == null;

            if (!firstHand && playMode != 0) return null;
            Permutation perm = CombineSelectedTilesToCurrentBlocks(hand, firstHand);

            if (CachedSelectedPermutation != null && perm.ToTiles()
                                                      .All(t => CachedSelectedPermutation.ToTiles().Contains(t))
                                                  && CachedSelectedPermutation.ToTiles()
                                                      .All(t => perm.ToTiles().Contains(t)))
                return CachedSelectedPermutation;

            CachedSelectedPermutation = perm;
            return perm;
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
            return firstHand
                ? hand.GetHighestScoredPerm(this)
                : hand.GetHighestScoredPerm(GetAccumulatedPermutation().blocks.ToList(), this);
        }

        /// <returns>可能会回传null，代表无对应结果</returns>
        private Permutation CombineSelectedTilesToCurrentBlocks(Hand hand, bool firstHand, Tile p1, Tile p2)
        {
            return firstHand
                ? hand.GetHighestScoredPerm(this, p1, p2)
                : hand.GetHighestScoredPerm(GetAccumulatedPermutation().blocks.ToList(), this, p1, p2);
        }


        /// <summary>
        /// 随机抽取n个工艺品，若抽取失败则返回空List
        /// </summary>
        /// <param name="n"></param>
        /// <returns>n个可能为null的工艺品列表</returns>
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
            return artifactList;
        }

        private LotteryPool<Artifact> GenerateEquatedPoolFromRarity(Rarity rarity)
        {
            List<Artifact> bank = ArtifactBank.Select(Artifacts.GetArtifact).Where(a => a != null).ToList();
            LotteryPool<Artifact> pool = new();
            foreach (var artifact in bank)
            {
                if (artifact.GetRarity() == rarity && artifact.IsAvailableInShops(this))
                {
                    pool.Add(artifact, 100);
                }
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

            if (GetMoney() < price)
            {
                return -1;
            }

            if (!ObtainArtifact(artifact)) return -2;
            SpendMoney(price);

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

            PostObtainArtifactEvent?.Invoke(new(this, artifact));

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


            PreObtainArtifactEvent?.Invoke(evt);

            return evt.res;
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
            PreRemoveArtifact?.Invoke(new(this, artifact));
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
            int epicWeight = 1, int fontedPercentage = 25, bool canGenerateHonorSeq = true)
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

                bool IsMixed = GenerateRandomInt(2) == 0;
                if (IsMixed && initialTile.IsNumbered())
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
            OnPrePostAddOnTileAnimationEffectEvent?.Invoke(GetCurrentSelectedPerm(), this, effects);
        }

        /// <summary>
        /// 手牌计分效果触发后，观赏效果触发前扳机，一般用于Boss计分效果
        /// </summary>
        /// <param name="effects"></param>
        public void TriggerPostAddOnTileAnimationEffect(List<OnTileAnimationEffect> effects)
        {
            OnPostAddOnTileAnimationEffectEvent?.Invoke(GetCurrentSelectedPerm(), this, effects);
        }

        public void TriggerPostAddOnArtifactAnimationEffect(List<IAnimationEffect> effects)
        {
            OnPostAddScoringAnimationEffectEvent?.Invoke(GetCurrentSelectedPerm() ?? CurrentAccumulatedBlock, this,
                effects);
        }

        public void TriggerPostAddOnBlockAnimationEffect(List<IAnimationEffect> effects)
        {
            OnPostAddOnBlockAnimationEffectEvent?.Invoke(GetCurrentSelectedPerm(), this, effects);
        }

        public void TriggerOnAddSingleAnimationEffectEvent(List<IAnimationEffect> neighbors, IAnimationEffect effect)
        {
            OnAddSingleAnimationEffectEvent?.Invoke(this, neighbors, effect);
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
            SpendMoneyEvent?.Invoke(new(this, v));
            EventManager.Instance.OnSpendMoney(v);
            DecreaseMoney(v);
            stats.SpendMoney(v);
        }

        public virtual void OnRoundStart()
        {
            PreRoundStartEvent?.Invoke(new(this));
            HeldGadgets.ForEach(g => g.ResetState(this));
            InitHandDeck();
            levelTarget = GetBasicLevelTarget();

            PostRoundStartEvent?.Invoke(new(this));
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
            DeterminePlayerSelectingTileEvent evt = new(this, tile, GetSelectedTilesCopy().Contains(tile));
            DetermineSelectingTileEvent?.Invoke(evt);
            return evt.res;
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
                            ObtainGadgetEvent?.Invoke(this, gadget);
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
                    ObtainGadgetEvent?.Invoke(this, gadget);
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
                            ObtainGadgetEvent?.Invoke(this, gadget);
                            stats.OnBoughtGadget(gadget);
                            return true;
                        }
                    }
                }
            }

            gadget.ResetState(this);
            ObtainGadgetEvent?.Invoke(this, gadget);
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
            PlayerKongTileEvent eventData = new(this, tile, perm, block);
            PreKongTileEvent?.Invoke(eventData);
            if (eventData.canceled) return -1;

            bool res = block.Kong(tile);
            if (!res) throw new ArgumentException("INVALID KONG COMMAND RECEIVED");
            MoveFromHandToDiscard(tile);
            DiscardLeft += 2;
            OnKong(block, perm);
            return DrawTileToHandDeck();
        }

        public virtual void OnKong(Block block, Permutation perm)
        {
        }

        [AotenjoCommand("destroyYaku", nameof(ArgumentParsers.ToYakuType))]
        public void DestoryYaku(YakuType yakuTypeID)
        {
            int level = skillSet.GetLevel(yakuTypeID);
            PlayerYakuEvent.Delete yakuEvent = new(this, yakuTypeID, level);
            DeleteYakuEvent?.Invoke(yakuEvent);
            if (yakuEvent.canceled) return;
            skillSet.ClearLevel(yakuTypeID);
        }
        
        [AotenjoCommand("setLevel", nameof(ArgumentParsers.ToInt))]
        public void SetLevel(int level)
        {
            if (level <= 0)
            {
                throw new ArgumentException("Level must be positive");
            }
            Level = level;
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
            UnityEngine.Debug.Log($"Available artifacts (first {count}):");
            foreach (var artifact in artifacts)
            {
                UnityEngine.Debug.Log($"- {artifact.GetNameID()} (Field: {artifact.GetType().Name})");
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

        public virtual List<YakuPack> TryDrawYakuPack(int v, List<YakuPack> yakuPacks)
        {
            if (yakuPacks.Count < v) throw new ArgumentOutOfRangeException("NOT ENOUGH YAKUPACKS TO POLL");

            List<YakuPack> bank = new(yakuPacks);
            List<YakuPack> result = new();
            for (int i = 0; i < v; i++)
            {
                LotteryPool<YakuPack> pool = new LotteryPool<YakuPack>();
                bank.ForEach(p => pool.Add(p, 1));
                YakuPack pollResult = pool.Draw(GenerateRandomInt);
                result.Add(pollResult);
                bank.Remove(pollResult);
            }

            return result;
        }

        public void PostUsedGadget(Gadget gadget)
        {
            if (gadget.uses < 0) throw new ArgumentException("GADGET USES EXHAUSTED");
            gadget.uses--;
            if (gadget.uses == 0)
            {
                if (gadget.IsConsumable())
                {
                    HeldGadgets.Remove(gadget);
                }
            }

            PostUseGadgetEvent?.Invoke(new(this, gadget));
        }

        public virtual List<Destination> GenerateDestinations()
        {
            int v = Level > 8 ? 4 : 2;
            int saleIndex = GenerateRandomInt(v);
            LotteryPool<Destination> commonDestination = new();

            commonDestination.Add(GadgetsShopDestination.Create(this, Destination.DestinationEventType.COMMON), 10);
            commonDestination.Add(TileDeleteShopDestination.Create(this, Destination.DestinationEventType.COMMON), 9);
            commonDestination.Add(TileModifyShopDestination.Create(this, Destination.DestinationEventType.COMMON), 10);
            commonDestination.Add(TileAddShopDestination.Create(this, Destination.DestinationEventType.COMMON), 10);

            if (!(Level % 4 == 1 && Level > 1))
            {
                commonDestination.Add(new WastelandDestination(false, this), 10);
            }

            List<Destination> result = new();

            for (int i = 0; i < v; i++)
            {
                result.Add(commonDestination.Draw(range => GenerateRandomInt(range, "destination"), false));
            }

            if (Level % 4 == 1 && Level > 1)
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


            PostGenerateDestinationEvent?.Invoke(this, result);

            return result;
        }

        public List<Artifact> DrawRandomArtifact(Rarity rarity, int count)
        {
            List<Artifact> validArtifacts = ArtifactBank.Select(Artifacts.GetArtifact)
                .Where(a => a.IsAvailableInShops(this) && a.GetRarity() == rarity).ToList();
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
                TilePool.Remove(TilePool.First(t => t.CompactWith(tile)));
                HandDeck.Add(tile);
            }
        }


        public PermutationType[] GetAvailablePermTypes()
        {
            List<PermutationType> types = new();

            if (skillSet.GetYakus().Contains(YakuType.Base))
            {
                types.Add(PermutationType.NORMAL);
            }

            if (skillSet.GetYakus().Contains(YakuType.QiDui))
            {
                types.Add(PermutationType.SEVEN_PAIRS);
            }

            if (skillSet.GetYakus().Contains(YakuType.ShiSanYao))
            {
                types.Add(PermutationType.THIRTEEN_ORPHANS);
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
            PlayerYakuEvent.Upgrade evt = new PlayerYakuEvent.Upgrade(this, yaku, level);
            OnPreUpgradeYakuEvent?.Invoke(evt);
            return evt;
        }

        public bool DetermineMaterialCompactbility(Tile tile, TileMaterial mat)
        {
            PlayerDetermineMaterialCompactibilityEvent evt = new(this, tile, mat);
            DetermineMaterialCompactbilityEvent?.Invoke(evt);
            return evt.res;
        }

        public bool DetermineFontCompactbility(Tile tile, TileFont font)
        {
            PlayerDetermineFontCompactbilityEvent evt = new(this, tile, font);
            DetermineFontCompactbilityEvent?.Invoke(evt);
            return evt.res;
        }

        public bool DetermineTileCompactbility(Tile tile, int cat, int order)
        {
            PlayerDetermineTileFaceCompactbilityEvent evt = new(this, tile, cat, order);
            DetermineTileCompactbilityEvent?.Invoke(evt);
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
            PlayerChoosePathEvent evt = new PlayerChoosePathEvent(this, direction, destinations.ToArray());
            ChoosePathEvent?.Invoke(evt);
        }

        public bool OnSetTransform(Tile tile, TileTransform tileTransform, Gadget gadget = null)
        {
            PlayerSetTransformEvent evt = new PlayerSetTransformEvent(this, gadget, tileTransform, tile);
            PreSetTransformEvent?.Invoke(evt);
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
            PreAddScoringAnimationEffectEvent?.Invoke(GetCurrentSelectedPerm() ?? CurrentAccumulatedBlock, this,
                inRoundAnimationQueue);
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
                EventManager.Instance.OnUpgradeYakuEvent(yaku.GetYakuType(), lvBefore[yaku], lvAfter[yaku]);
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
            PreSetMaterialEvent?.Invoke(new PlayerSetAttributeEvent(this, tile, newMaterial, isCopy));
        }

        public void OnChangeFont(Tile tile, TileFont newFont, bool isCopy)
        {
            PreSetFontEvent?.Invoke(new PlayerSetAttributeEvent(this, tile, newFont, isCopy));
        }

        public bool OnChangeMask(Tile tile, TileMask newMask, bool isCopy)
        {
            PlayerSetAttributeEvent evt = new PlayerSetAttributeEvent(this, tile, newMask, isCopy);
            PreSetMaskEvent?.Invoke(evt);
            return evt.canceled;
        }

        /// <summary>
        /// SetMat不触发
        /// </summary>
        public void OnchangeProperties(Tile tile, TileProperties toBecome, bool isCopy)
        {
            PreSetPropertiesEvent?.Invoke(new PlayerSetPropertiesEvent(this, tile, toBecome, isCopy));
        }
        
        /// <summary>
        /// SetMat、SetFont等都会触发
        /// </summary>
        public void PreChangedProperties(Tile tile, TileProperties newProperties)
        {
            PreSetTilePropertiesEvent?.Invoke(new PlayerSetPropertiesEvent(this, tile, newProperties, false));
        }

        public int GetMaxPlayingStage()
        {
            return 4;
        }

        public Block GenerateRandomBlock()
        {
            List<Tile> tiles = GetUniqueFullDeck();
            Tile generator = tiles[GenerateRandomInt(tiles.Count)];

            bool isSequence = generator.IsNumbered() && GenerateRandomInt(4) <= 2;

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

                return candBlocks[GenerateRandomInt(candBlocks.Count)];
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
            TriggerOnAddRoundEndAnimationEffectEvent(onRoundEndEffects);
        }

        public void TriggerOnAddRoundEndAnimationEffectEvent(List<IAnimationEffect> onRoundEndEffects)
        {
            OnPostAddRoundEndAnimationEffectEvent?.Invoke(GetCurrentSelectedPerm() ?? CurrentAccumulatedBlock, this,
                onRoundEndEffects);
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
                artifact.AddDiscardTileEffects(this, tile, onDiscardTileEffects, withForce, isClone);
            }

            TriggerOnAddDiscardTileAnimationEffectEvent(onDiscardTileEffects, tile, withForce);
        }

        public void TriggerOnAddDiscardTileAnimationEffectEvent(List<IAnimationEffect> onDiscardTileEffects, Tile tile, bool withForce)
        {
            OnAddSingleDiscardTileAnimationEffectEvent?.Invoke(this, onDiscardTileEffects, tile, withForce);
        }

        public void OnModifyCarvedDesign(Tile tile, Category cat, int order)
        {
            PostModifyTileCarvatureEvent?.Invoke(this, tile, cat, order);
        }

        public void TriggerDessertTileConsumedEvent(Tile tile, TileMaterialDessert dessert)
        {
            OnDessertTileConsumedEvent?.Invoke(this, tile, dessert);
        }

        public bool TriggerDessertTileConsumeAttemptEvent(Tile tile, TileMaterialDessert dessert)
        {
            var evt = new PlayerConsumeDessertEvent(this, tile, dessert);
            OnDessertTileConsumeAttemptEvent?.Invoke(evt);
            return !evt.canceled;
        }

        public void OnEndRun()
        {
            OnEndRunEvent?.Invoke(this, won);
        }

        public void PostEndRun(PlayerStats globalStats)
        {
            PostRunEndEvent?.Invoke(this, won, globalStats);
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
            return Bosses.GetBossOrElseRedraw(upcomingBosses[index], HarderBossesEnabled());
        }

        public Boss GetBossAtRound(int prevalentWind)
        {
            int pluses = (Level - 1) / 16;
            int index = 4 * pluses + prevalentWind - 1;
            while (index >= upcomingBosses.Count) GenerateNewUpcomingBosses();
            return Bosses.GetBossOrElseRedraw(upcomingBosses[index], HarderBossesEnabled());
        }

        public void SyncSelectingTiles(List<Tile> tiles)
        {
            CurrentSelectedTiles = new(tiles);
        }

        public void SetGadgetLimit(int v)
        {
            properties.GadgetLimit = v;
        }

        public bool DetermineYaojiu(Tile tile)
        {
            PlayerTileEvent evt = new(this, tile);
            evt.canceled =
                !(tile.IsHonor(this) || (tile.IsNumbered() && (tile.GetOrder() == 1 || tile.GetOrder() == 9)));
            DetermineYaojiuTileEvent?.Invoke(evt);
            return !evt.canceled;
        }

        public bool DetermineHonor(Tile tile)
        {
            return tile.GetCategory() == Category.Jian || tile.GetCategory() == Category.Feng;
        }

        public bool DetermineShiftedPair(Block b1, Block b2, int step, bool categorySensitive)
        {
            PlayerDetermineShiftedPairEvent evt = new(this, b1, b2, step, categorySensitive,
                GetCombinator().ASuccB(b2, b1, categorySensitive, step));
            DetermineShiftedPairEvent?.Invoke(evt);
            return evt.res;
        }

        public bool IsPlayerWind(int v)
        {
            PlayerEvent evt = new PlayerEvent(this);
            evt.message = v.ToString();
            evt.canceled = v != GetPlayerWind();
            DeterminePlayerWindEvent?.Invoke(evt);
            return !evt.canceled;
        }

        public bool IsPrevalentWind(int v)
        {
            PlayerEvent evt = new PlayerEvent(this);
            evt.message = v.ToString();
            evt.canceled = v != GetPrevalentWind();
            DeterminePrevalentWindEvent?.Invoke(evt);
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
            PlayerTileEvent evt = new(this, tile);
            DetermineTileSelectivityEvent?.Invoke(evt);
            return !evt.canceled;
        }

        public void TriggerPreSettlePermutationEvent()
        {
            PreSettlePermutationEvent?.Invoke(new(this, GetCurrentSelectedPerm()));
        }
        
        public void TriggerPreAppendSettleScoringEffectsEvent()
        {
            PreAppendSettleScoringEffectsEvent?.Invoke(new(this, GetCurrentSelectedPerm()));
        }

        public void TriggerOnAddSingleTileAnimationEffectEvent(Permutation perm, List<OnTileAnimationEffect> tileAnimationQueue, OnTileAnimationEffect eff, Tile tile)
        {
            PostAddSingleTileAnimationEffectEvent?.Invoke(perm, this, tileAnimationQueue, eff, tile);
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
                AddTileToPool(new Tile(tile));
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
            double baseFan = YakuTester.GetFan(yakuType, permutation.IsFullHand() ? 4 : permutation.blocks.Count(), GetSkillSet());
            double multiplier = GetYakuMultiplier(yakuType);
            return baseFan * multiplier;
        }

        public double GetYakuMultiplier(YakuType yakuType)
        {
            PlayerYakuEvent.RetrieveMultiplier evt = new(this, yakuType, 1.0D);
            RetrieveYakuMultiplierEvent?.Invoke(evt);
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
            var evt = new PlayerYakuEvent.ReadBookResult(this, drawnYakus.ToArray(), book);
            PostUpgradeYakuFromIBookEvent?.Invoke(evt);
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
            PlayerJadeEvent.RetrieveEffectiveStack evt = new(this, jade, jade.GetLevel(this));
            RetrieveEffectiveJadeStackEvent?.Invoke(evt);
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
            OnAddSingleTileScoringEffectEvent?.Invoke(permutation, this, effects, tile);
        }

        public int GetYakuPackPrice(IBook yakuPack)
        {
            //TODO: 改
            if (GetArtifacts().Contains(Artifacts.Magnifier))
            {
                return 4;
            }
            return 3;
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
                { ScoreEffect.AddFu(() => GetBaseFuOfTile(tile), null).OnTile(tile) };
        }

        public void EraseBlock(Block block)
        {
            Permutation perm = GetAccumulatedPermutation();
            if(perm == null) return;
            perm.blocks = perm.blocks.Except(new[] { block }).ToArray();
            if (!perm.blocks.Any() || perm.GetPermType() == PermutationType.SEVEN_PAIRS)
                SetCurrentAccumulatedBlock(null);
            bool needUnfreeze = CurrentPlayingStage == GetMaxPlayingStage();
            CurrentPlayingStage--;
            if (needUnfreeze)
            {
                EventManager.Instance.OnUnfreezeEvent(this);
            }
        }
    }
}