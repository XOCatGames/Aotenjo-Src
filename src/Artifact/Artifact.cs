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
        private Action<Tile, Player> tileGiftAction;
        private int tileGiftCount;
        
        // 内置遗物通过各自子类覆写计分入口，基类只初始化身份与独立配置容器。
        // Lua 遗物的动态回调由 LuaArtifact/LuaCraftableArtifact 自己持有，避免基类再保存另一套行为。
        public Artifact(string name, Rarity rarity)
        {
            this.name = name;
            this.rarity = rarity;

            deckIn = new List<string>();
            deckBlocked = new List<string>();
            setIn = new List<string>();
            setBlocked = new List<string>();
            materialRequired = new List<string>();
            tags = new List<ArtifactTag>();
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

        public static (string, double) ToAddFanFormat(double input)
        {
            return ("<style=\"fan\">{0}</style>", input);
        }

        public static (string, double) ToAddFuFormat(double input)
        {
            return ("<style=\"fu\">{0}</style>", input);
        }

        public static (string, double) ToMulFanFormat(double input)
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

        public Artifact SetMaterialGift(Func<TileMaterial> materialFactory, int count)
        {
            return SetTileGift((tile, player) => tile.SetMaterial(materialFactory(), player), count);
        }

        public Artifact SetFontGift(Func<TileFont> fontFactory, int count)
        {
            return SetTileGift((tile, player) => tile.SetFont(fontFactory(), player), count);
        }

        private Artifact SetTileGift(Action<Tile, Player> giftAction, int count)
        {
            tileGiftAction = giftAction;
            tileGiftCount = count;
            return this;
        }

        public virtual bool CanBeSellByPlayer()
        {
            return true;
        }

        public virtual bool CanBeSold(Player player) => CanBeSellByPlayer();

        public virtual void OnSold(Player player) { }

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
                r != null && r.HasValidStructure && r.inputID.Contains(GetNameID()) &&
                player.CanCraftWithArtifactAffixes(r, true, this));
        }

        public virtual List<string> GetComponentIds()
        {
            return new List<string> { GetRegName() };
        }

        public virtual void ResetArtifactState(Player player)
        {
            ResetArtifactState();
            status = new ArtifactStatus();
        }

        public virtual void ResetArtifactState()
        {
        }

        private bool inShop = false;
        
        public virtual string GetDescription(Func<string, string> localizer)
        {
            string description;
            if(inShop)
            {
                if(localizer($"artifact_{name}_description_inshop") != ($"artifact_{name}_description_inshop"))
                    description = localizer($"artifact_{name}_description_inshop");
                else
                    description = localizer($"artifact_{name}_description");
            }
            else
            {
                description = localizer($"artifact_{name}_description");
            }

            if (tileGiftAction != null && tileGiftCount > 0)
                return string.Format(localizer("artifact_tile_gift_description_format"), tileGiftCount,
                    description);

            return description;
        }

        public virtual string GetDescription(Player player, Func<string, string> localizer)
        {
            return GetDescription(localizer);
        }

        public virtual string GetInShopDescription(Player player, Func<string, string> localizer)
        {
            inShop = true;
            try { return GetDescription(player, localizer); }
            finally { inShop = false; }
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
            Artifacts.LEGACY_NUMBER_ID_MAP.TryGetValue(GetRegName(), out int value);
            return value;
        }

        protected virtual int GetSpriteID()
        {
            return Artifacts.ARTIFACT_SPRITE_ID_MAP.TryGetValue(GetRegName(), out int value) ? value : 0;
        }
        
        public virtual Sprite GetSprite(Player player)
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
            if (tileGiftAction == null || tileGiftCount <= 0) return;

            foreach (Tile tile in player.DrawPlainTilesFromPool(tileGiftCount))
            {
                tileGiftAction(tile, player);
            }
        }

        public virtual void OnRemoved(Player player)
        {
            UnsubscribeToPlayer(player);
            IsBroken = false;
            if (IsTemporary && !status.temporaryOccupiesSlot)
                player.ChangeArtifactLimit(-1);
            IsTemporary = false;
            status.temporaryOccupiesSlot = false;
            IsSuppressed = false;
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

        #region 事件订阅系统
        
        [NonSerialized] private AttributeEventSubscriptions attributeSubscriptions;

        /// <summary>
        /// 订阅 Artifact 自身所有带有 [SubscribeToEvent] 的方法，保留全局事件范围。
        /// 子类重写时，必须调用 base.SubscribeToPlayer(player) 才会自动扫描。
        /// </summary>
        public virtual void SubscribeToPlayer(Player player)
        {
            // 先校验运行实例归属，并在绑定前登记；中途失败时沿用玩家的统一清理流程。
            // 共享订阅器负责清理旧委托、扫描继承链及清空总线后的重绑，宿主仍保持原有全局范围。
            // 词条在宿主事件之后绑定，避免改变现有库存顺序与词条生命周期。
            if (IsPreview || (RuntimeOwner != null &&
                (!ReferenceEquals(RuntimeOwner, player) || !RuntimeOwner.HoldsArtifact(this))))
                throw new InvalidOperationException("只有持有的运行遗物可以订阅事件。");
            RuntimeOwner?.TrackArtifactSubscription(this);
            (attributeSubscriptions ??= new AttributeEventSubscriptions()).Subscribe(this, throwOnFailure: true);
            Affix?.SubscribeToPlayer(player);
        }

        /// <summary>
        /// 取消自动注册的事件。子类重写时必须调用 base.UnsubscribeToPlayer(player)。
        /// </summary>
        public virtual void UnsubscribeToPlayer(Player player)
        {
            Affix?.UnsubscribeFromPlayer();
            attributeSubscriptions?.Unsubscribe();
            if (ReferenceEquals(RuntimeOwner, player)) player.ForgetArtifactSubscription(this);
        }

        /// <summary>
        /// 回合结束前移除损坏或临时 Artifact。
        /// </summary>
        [SubscribeToEvent]
        protected void RemoveFromPlayerInv(PlayerRoundEvent.End.Pre eventData)
        {
            if (!IsBroken && !IsTemporary)
            {
                return;
            }
            // 临时遗物到期只执行移除，不能触发主动出售回调。
            eventData.player.RemoveArtifact(this, true);
        }


        #endregion

        public virtual void PreGameInitialized(Player player)
        {
        }

        public string GetNameID()
        {
            return name;
        }

        public virtual string GetStory(Func<string, string> loc)
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
            return ArtifactUnlockRequirements.artifactRequirementMap.GetValueOrDefault(GetRegName(),
                UnlockRequirement.UnlockedByDefault());
        }

        public virtual void AppendDiscardTileEffects(Player player, Tile tile, List<IAnimationEffect> onDiscardTileEffects,
            bool withForce, bool isClone)
        {
        }

        public bool IsMaterial(Player player)
        {
            return ArtifactRecipes.recipes.Any(r =>
                r.inputID.Contains(GetNameID()) && (player == null ||
                                                    r.inputID.All(i =>
                                                        player.GetRunArtifact(i).IsAvailableGlobally(player))));
        }

        public bool IsCraftResult(Player player)
        {
            return ArtifactRecipes.recipes.Any(r =>
                r.outputID.Contains(GetNameID()) && (player == null ||
                                                     r.inputID.All(i =>
                                                         player.GetRunArtifact(i).IsAvailableGlobally(player))));
        }

        public virtual string GetSubHeader(Player player, Func<string, string> loc)
        {
            string rarityName = GetRarity().ToString().ToLower();
            string subHeader = loc("artifact_name");
            if (IsMaterial(player)) subHeader = $"{loc("ui_cailiao")} {subHeader}";
            if (IsCraftResult(player)) subHeader = $"{loc("ui_hecheng")} {subHeader}";
            
            if (tags.Any())
            {
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

            // 旧式临时遗物额外提供栏位；占用现有栏位的奖励必须单独记录。
            [SerializeField] public bool temporaryOccupiesSlot;

            [SerializeField] public bool isDebuffed;
        }

        private ArtifactAffixState runtimeAffix = new ArtifactAffixState();
        public ArtifactAffixInstance Affix { get; private set; }
        public ArtifactAffixState AffixState => runtimeAffix.Copy();
        internal ArtifactAffixState RuntimeAffix
        {
            get => runtimeAffix;
            set
            {
                // 旧实例先退订，替换后发布快照中的旧委托也会立即失效。
                // 此处只替换属性；已激活对象由赋予入口按库存顺序重绑一次，读档和候选对象不订阅。
                Affix?.UnsubscribeFromPlayer();
                runtimeAffix = value ?? new ArtifactAffixState();
                Affix = ArtifactAffixInstance.Create(this, runtimeAffix);
            }
        }

        internal T Query<T>(T evt, ArtifactAffixState candidate = null) where T : ArtifactQueryEvent
        {
            // 候选词条只存在于本次查询范围内，既不覆盖宿主状态，也不进入实局订阅表。
            var affix = candidate == null ? Affix : ArtifactAffixInstance.Create(this, candidate);
            affix?.PublishQuery(evt);
            return evt;
        }

        internal Player RuntimeOwner { get; set; }
        /// <summary>对局内稳定的实例编号；类型 ID 仍由 GetRegName 返回，预览不可用于获取。</summary>
        public int InstanceId { get; internal set; }
        public bool IsPreview { get; internal set; }

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

        public bool IsSuppressed
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

        // 非虚入口统一先收集本体、再发布宿主事件；原有虚方法仍供内置遗物与 Lua 实现本体效果。
        // Player 兼容入口和延迟动画沿用同一管线，避免子类漏调 base 导致词条未执行或顺序颠倒。
        public void CollectSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            AppendOnSelfEffects(player, permutation, effects);
            EventBus.PublishRequired(new ArtifactEffectEvent.Self(player, this, permutation, effects));
        }

        public void CollectTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            AppendOnTileEffects(player, permutation, tile, effects);
            EventBus.PublishRequired(new ArtifactEffectEvent.Tile(player, this, permutation, tile, effects));
        }

        public void CollectBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            AddOnBlockEffects(player, permutation, block, effects);
            EventBus.PublishRequired(new ArtifactEffectEvent.Block(player, this, permutation, block, effects));
        }

        public void CollectRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            AddOnRoundEndEffects(player, permutation, effects);
            EventBus.PublishRequired(new ArtifactEffectEvent.RoundEnd(player, this, permutation, effects));
        }

        public virtual void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
        }

        public virtual void AddOnTileEffectsPostEvents(Player player, Permutation permutation, Tile tile,
            List<Effect> effects)
        {
        }

        public virtual void AddOnBlockEffects(Player player, Permutation permutation, Block block,
            List<Effect> effects)
        {
        }

        public virtual void AppendPostBlockAnimationEffects(Player player, Permutation permutation, Block block,
            List<IAnimationEffect> effects)
        {
        }

        public virtual void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
        }

        public virtual void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
        }

        public virtual void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
        }

        #endregion
    }
}
