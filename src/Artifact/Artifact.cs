using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using static Aotenjo.Tile;

namespace Aotenjo
{
    [Serializable]
    public class Artifact : IPriced, IUnlockable, ITileHighlighter
    {
        [BoxGroup("基础信息")] [LabelText("稀有度")] 
        public Rarity rarity;

        /// <summary>
        /// 遗物标签，可多选，用于部分遗物的效果判断（例如铜质匕首）
        /// </summary>
        public List<ArtifactTag> tags;

        [BoxGroup("基础信息")] [ShowInInspector] [LabelText("ID")]
        private readonly string name;

        public Func<Tile, Player, bool> canProcOn = (_, _) => true;
        public Predicate<Player> isAvailable = _ => true;
        
        //TODO: 需要清理的技术债，远古时期使用函数来表达遗物是一个错误的决定
        public Action<Player, Permutation, Block, List<Effect>> onBlockEffect;
        public Action<Player, Permutation, Tile, List<Effect>> onTileEffect;
        public Action<Player, Permutation, List<Effect>> selfEffect;

        public Artifact(string name, Rarity rarity,
            Action<Player, Permutation, Tile, List<Effect>> onTileEffect,
            Action<Player, Permutation, Block, List<Effect>> onBlockEffect,
            Action<Player, Permutation, List<Effect>> selfEffect)
        {
            this.name = name;
            this.rarity = rarity;
            this.onTileEffect = onTileEffect;
            this.onBlockEffect = onBlockEffect;
            this.selfEffect = selfEffect;

            deckIn = new List<string>();
            deckBlocked = new List<string>();
            setIn = new List<string>();
            setBlocked = new List<string>();
            materialRequired = new List<string>();
            tags = new List<ArtifactTag>();
        }

        public Artifact(string name, Rarity rarity) : this(name, rarity, (_, _, _, _) => { }, (_, _, _, _) => { },
            (_, _, _) => { })
        {
        }

        public int Price => GetBasePrice();

        /// <summary>
        /// 是否在悬停时高亮显示牌，通常用于遗物的效果触发条件
        /// </summary>
        /// <param name="tile">需要判断的麻将牌</param>
        /// <param name="player">玩家实例</param>
        /// <returns>高亮结果</returns>
        public virtual bool ShouldHighlightTile(Tile tile, Player player)
        {
            return canProcOn(tile, player);
        }

        public bool IsUnlocked(PlayerStats stats)
        {
            return GetUnlockRequirement().IsUnlocked(stats);
        }

        public string GetRegName()
        {
            return name;
        }

        public virtual string GetSpriteNamespaceID(Player player, string nmSpace = "aotenjo")
        {
            return $"artifact:{nmSpace}:{name}";
        }

        protected static (string, double) ToAddFanFormat(double input)
        {
            return ("<style=\"fan\">{0}</style>", input);
        }

        protected static (string, double) ToAddFuFormat(double input)
        {
            return ("<style=\"fu\">{0}</style>", input);
        }

        protected static (string, double) ToMulFanFormat(double input)
        {
            return ("<style=\"mul\">X{0}</style>", input);
        }

        public Artifact SetHighlightRequirement(Func<Tile, Player, bool> pred)
        {
            canProcOn = pred;
            return this;
        }

        public Artifact SetPrerequisite(Predicate<Player> pred)
        {
            isAvailable = pred;
            return this;
        }

        public virtual bool CanBeSellByPlayer()
        {
            return true;
        }

        public override string ToString()
        {
            return GetNameKey();
        }

        public bool IsCrafted()
        {
            return false;
        }

        public virtual bool IsUnique()
        {
            return false;
        }

        public virtual bool CanBeBoughtWithoutSlotLimit(Player player)
        {
            return ArtifactRecipes.recipes.Any(r =>
                r.inputID.All(id => player.GetArtifacts().Any(a => a.GetNameID() == id) || GetNameID() == id));
        }

        public virtual List<Artifact> GetComponents()
        {
            return new List<Artifact> { this };
        }

        public virtual void ResetArtifactState(Player player)
        {
            ResetArtifactState();
            status = new ArtifactStatus();
        }

        public virtual void ResetArtifactState()
        {
        }

        public virtual string GetDescription(Func<string, string> localizer)
        {
            return localizer($"artifact_{name}_description");
        }

        public virtual string GetDescription(Player player, Func<string, string> localizer)
        {
            return GetDescription(localizer);
        }

        public virtual string GetInShopDescription(Player player, Func<string, string> localizer)
        {
            return GetDescription(player, localizer);
        }

        public string GetChanceMultiplier(Player player)
        {
            if (player is ScarletPlayer scarletPlayer && scarletPlayer.GetCategoryLevel(Category.Bing) >= 2)
                return "<style=\"green\">2</style>"; // 概率翻倍：1/? -> 2/?
            return "1";
        }

        public virtual int GetBuyingPrice(bool firstTimeBuying)
        {
            return GetBasePrice() + (firstTimeBuying ? -1 : 0);
        }

        public virtual int GetBasePrice()
        {
            switch (GetRarity())
            {
                case Rarity.COMMON: return 4;
                case Rarity.RARE: return 7;
                case Rarity.EPIC: return 12;
                default: return -1;
            }
        }

        public virtual bool IsAvailableGlobally(Player player)
        {
            return IsAvailableGlobally(player.deck, player.materialSet);
        }

        public virtual bool IsAvailableGlobally(MahjongDeck deck, MaterialSet set)
        {
            if (set == null || deck == null) return false;
            if (deckBlocked.Any(d => deck.regName.Equals(d))) return false;
            if (deckIn.Count != 0 && deckIn.All(d => !deck.regName.Equals(d))) return false;
            if (setIn.Count != 0 &&
                !setIn.All(s =>
                {
                    return MaterialSet.GetMaterialSet(s).availableMaterials.Any(m =>
                    {
                        var availableMaterials = set.availableMaterials;
                        return availableMaterials.Any(m2 => m2.Equals(m)) ||
                               availableMaterials.Any(m2 => m2.Equals(m + "_material"));
                    });
                })) return false;

            if (setBlocked != null && setBlocked.Any(s => set.regName.Equals(s))) return false;

            if (materialRequired != null && materialRequired.Any() &&
                !materialRequired.All(mat =>
                    set.availableMaterials.Any(m => m == mat || m.Replace("_material", "") == mat))) return false;

            return true;
        }

        public virtual bool IsAvailableGlobally(MahjongDeck deck)
        {
            if (deckBlocked.Any(d => deck.regName.Equals(d))) return false;
            if (deckIn.Count != 0 && deckIn.All(d => !deck.regName.Equals(d))) return false;
            return true;
        }

        public virtual bool IsAvailableGlobally(MaterialSet set)
        {
            if (setIn.Count != 0 && setIn.All(s => !set.regName.Equals(s))) return false;
            return true;
        }

        public virtual bool IsAvailableInShops(Player player)
        {
            return isAvailable(player);
        }

        public virtual int GetSellingPrice()
        {
            if (IsBroken || IsTemporary) return 0;
            return (int)(GetBasePrice() * 0.5);
        }

        public virtual string GetNameWithColor(Func<string, string> localizer)
        {
            return $"<style=\"artifact_{GetRarity().ToString().ToLower()}\">{localizer(GetNameKey())}</style>";
        }

        public virtual string GetName(Func<string, string> localizer)
        {
            return localizer(GetNameKey());
        }

        public virtual string GetNameKey()
        {
            return $"artifact_{name}_name";
        }

        public virtual int GetNumberID()
        {
            Artifacts.ARTIFACT_SPRITE_ID_MAP.TryGetValue(this, out int value);
            return value;
        }

        protected virtual int GetSpriteID()
        {
            return GetNumberID();
        }
        
        public Sprite GetSprite(Player player)
        {
            Sprite sprite = SpriteManager.Instance.GetSingleSprite(GetSpriteNamespaceID(player));
            if (sprite == null)
                sprite = SpriteManager.Instance.GetSprites("Artifacts").ElementAtOrDefault(GetSpriteID());
            return sprite;
        }

        public virtual Rarity GetRarity()
        {
            return rarity;
        }

        public virtual bool CanObtainBy(Player player)
        {
            return true;
        }

        public virtual void OnObtain(Player player)
        {
            SubscribeToPlayer(player);
        }

        public virtual void OnRemoved(Player player)
        {
            UnsubscribeToPlayer(player);
            IsBroken = false;
            if (IsTemporary) player.SetArtifactLimit(player.GetArtifactLimit() - 1);
            IsTemporary = false;
            IsDebuffed = false;
        }

        /// <summary>
        /// 遗物的额外信息, 例如叠层数或是番数加成
        /// </summary>
        /// <param name="player">玩家实例</param>
        /// <returns>(格式, 数字)</returns>
        public virtual (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return (null, 0f);
        }

        public virtual Artifact SetBroken(Player player)
        {
            IsBroken = true;
            return this;
        }

        public virtual void SubscribeToPlayer(Player player)
        {
            if (IsBroken) EventBus.Subscribe<PlayerRoundEvent.End.Pre>(RemoveFromPlayerInv);
            if (IsTemporary) EventBus.Subscribe<PlayerRoundEvent.End.Pre>(RemoveFromPlayerInv);
        }


        public virtual void UnsubscribeToPlayer(Player player)
        {
            if (IsBroken) EventBus.Unsubscribe<PlayerRoundEvent.End.Pre>(RemoveFromPlayerInv);
            if (IsTemporary) EventBus.Unsubscribe<PlayerRoundEvent.End.Pre>(RemoveFromPlayerInv);
        }

        private void RemoveFromPlayerInv(PlayerEvent evt)
        {
            evt.player.SellArtifact(this);
        }

        public virtual void PreGameInitialized(Player player)
        {
        }

        public string GetNameID()
        {
            return name;
        }

        public string GetStory(Func<string, string> loc)
        {
            return loc($"artifact_{name}_story");
        }

        public string GetUnlockRequirementDescription(Func<string, string> loc)
        {
            return GetUnlockRequirement().GetDescription(loc);
        }

        public UnlockRequirement GetUnlockRequirement()
        {
            if (Constants.DEBUG_MODE) return UnlockRequirement.UnlockedByDefault();
            return ArtifactUnlockRequirements.artifactRequirementMap.GetValueOrDefault(this,
                UnlockRequirement.UnlockedByDefault());
        }

        public virtual void AddDiscardTileEffects(Player player, Tile tile, List<IAnimationEffect> onDiscardTileEffects,
            bool withForce, bool isClone)
        {
        }

        public bool IsMaterial(Player player)
        {
            return ArtifactRecipes.recipes.Any(r =>
                r.inputID.Contains(GetNameID()) && (player == null ||
                                                    r.inputID.All(i =>
                                                        Artifacts.GetArtifact(i).IsAvailableGlobally(player))));
        }

        public bool IsCraftResult(Player player)
        {
            return ArtifactRecipes.recipes.Any(r =>
                r.outputID.Contains(GetNameID()) && (player == null ||
                                                     r.inputID.All(i =>
                                                         Artifacts.GetArtifact(i).IsAvailableGlobally(player))));
        }

        public virtual string GetSubHeader(Player player, Func<string, string> loc)
        {
            string rarityName = GetRarity().ToString().ToLower();
            string subHeader = loc("artifact_name");
            if (IsMaterial(player)) subHeader = $"{loc("ui_cailiao")} {subHeader}";
            if (IsCraftResult(player)) subHeader = $"{loc("ui_hecheng")} {subHeader}";
            
            if (tags.Any())
            {
                string tagsText = "";
                Func<ArtifactTag, string> toTagName = t =>
                    GameLocalizationManager.GetLocalizedText($"artifact_tag_{t.ToString().ToLower()}");
                subHeader = $"{string.Join(" ", tags.Select(toTagName))} {subHeader}";
            }
            
            subHeader = $"{loc($"rarity_artifact_{rarityName}_name")} {subHeader}";
            if (IsBroken) subHeader = $"{loc("ui_artifact_broken_info")} {subHeader}";
            if (IsTemporary) subHeader = $"{loc("ui_artifact_temporary_info")} {subHeader}";

            
            return subHeader;
        }

        #region 遗物状态

        [Serializable]
        public class ArtifactStatus
        {
            [SerializeField] public bool isBroken;

            [SerializeField] public bool isTemporary;

            [SerializeField] public bool isDebuffed;
        }

        public ArtifactStatus status = new();

        public bool IsBroken
        {
            get => status.isBroken;
            set => status.isBroken = value;
        }

        public bool IsTemporary
        {
            get => status.isTemporary;
            set => status.isTemporary = value;
        }

        public bool IsDebuffed
        {
            get => status.isDebuffed;
            set => status.isDebuffed = value;
        }

        #endregion

        #region 遗物归属数据

        /*
         * 这些是遗物的元数据，在Unity中以ScriptableObject的形式存储
         * 请勿在运行时修改这些数据
         */

        public List<string> deckIn;
        public List<string> deckBlocked;
        public List<string> setIn;
        public List<string> setBlocked;
        public List<string> materialRequired;

        #endregion

        #region 序列化相关

        public virtual string Serialize()
        {
            return "";
        }

        public virtual void Deserialize(string data)
        {
        }

        #endregion


        #region 默认计分行为

        public virtual void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            onTileEffect?.Invoke(player, permutation, tile, effects);
        }

        public virtual void AddOnTileEffectsPostEvents(Player player, Permutation permutation, Tile tile,
            List<Effect> effects)
        {
        }

        public virtual void AddOnBlockEffects(Player player, Permutation permutation, Block block,
            List<Effect> effects)
        {
            onBlockEffect?.Invoke(player, permutation, block, effects);
        }

        public virtual void AppendPostBlockAnimationEffects(Player player, Permutation permutation, Block block,
            List<IAnimationEffect> effects)
        {
        }

        public virtual void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            selfEffect?.Invoke(player, permutation, effects);
        }

        public virtual void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
        }

        public virtual void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
        }

        #endregion

        #region 工厂方法

        public static Artifact CreateOnTileEffectArtifact(string name, Rarity rarity,
            Action<Player, Permutation, Tile, List<Effect>> action)
        {
            return new Artifact(name, rarity, action, (_, _, _, _) => { }, (_, _, _) => { });
        }

        public static Artifact CreateOnBlockEffectArtifact(string name, Rarity rarity,
            Action<Player, Permutation, Block, List<Effect>> action)
        {
            return new Artifact(name, rarity, (_, _, _, _) => { }, action, (_, _, _) => { });
        }

        public static Artifact CreateOnSelfEffectArtifact(string name, Rarity rarity,
            Action<Player, Permutation, List<Effect>> action)
        {
            return new Artifact(name, rarity, (_, _, _, _) => { }, (_, _, _, _) => { }, action);
        }

        #endregion
    }
}