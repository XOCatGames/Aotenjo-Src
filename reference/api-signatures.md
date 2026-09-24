# API 源码签名 / Source API signatures

[中文手册](../docs/zh/README.md) · [English handbook](../docs/en/README.md)

自动提取公开声明，用于精确查参数；构造函数也在其中。不是所有声明都有跨平台 Lua/AOT 绑定保证。
Extracted public declarations, including constructors. This is a parameter lookup index, not a guarantee of Lua/AOT binding for every member.

C# notation: `Action<A,B>` means a callback `(a,b)` with no return; `Func<A,B,R>` means `(a,b)` returning R. `List<T>` uses Count, arrays use Length; both start at 0.

## LuaArtifact

Namespace: `Aotenjo` · [源码 / source](../src/API/Artifact/LuaArtifact.cs)

```csharp
public string GetDataOrDefault(string key, string defaultValue);
public void SetData(string key, string value);
public override void Deserialize(string data);
public override string Serialize();
public LuaArtifact( string name, Rarity rarity,  Func<Tile, Player, Artifact, bool> shouldHighlightTile = null, Func<Player, Artifact, bool> isAvailableGlobally = null, Func<Player, Func<string, string>, Artifact, string> getDescription = null, Func<Player, Func<string, string>, Artifact, string> getInShopDescription = null, Func<Player, Artifact, (string, double)> getAdditionalInfo = null, Func<Player, Artifact, string> getNameWithColor = null, Func<Player, Artifact, string> getName = null, Func<Player, Artifact, string> getSubHeader = null, Action<Player, Artifact> onObtain = null, Action<Player, Artifact> onRemoved = null, Action<Player, Artifact> preGameInitialized = null, Action<Player, Artifact> resetArtifactState = null, Action<Player, Permutation, Tile, List<Effect>, Artifact> onTileEffect = null, Action<Player, Permutation, Tile, List<Effect>, Artifact> onTilePostEffect = null, Action<Player, Tile, List<IAnimationEffect>, bool, bool, Artifact> onDiscardTileEffect = null, Action<Player, Permutation, Tile, List<Effect>, Artifact> onUnusedTileEffect = null, Action<Player, Permutation, Block, List<Effect>, Artifact> onBlockEffect = null, Action<Player, Permutation, Block, List<IAnimationEffect>, Artifact> onBlockAnimEffect = null, Action<Player, Permutation, List<Effect>, Artifact> onSelfEffect = null, Action<Player, Permutation, List<IAnimationEffect>, Artifact> onRoundEndEffect = null, Func<Player, Artifact, string> getSpriteID = null, Action<Player, Artifact> onSubscribeToPlayer = null, Action<Player, Artifact> onUnsubscribeToPlayer = null );
public override string GetSpriteNamespaceID(Player player, string nmSpace = "unknown_mod");
public override bool ShouldHighlightTile(Tile tile, Player player);
public override bool IsAvailableGlobally(Player player);
public override string GetDescription(Func<string, string> localizer);
public override string GetDescription(Player player, Func<string, string> localizer);
public override string GetInShopDescription(Player player, Func<string, string> localizer);
public override (string, double) GetAdditionalDisplayingInfo(Player player);
public override string GetNameWithColor(Func<string, string> localizer);
public override string GetName(Func<string, string> localizer);
public override string GetSubHeader(Player player, Func<string, string> loc);
public override void OnObtain(Player player);
public override void OnRemoved(Player player);
public override void PreGameInitialized(Player player);
public override void ResetArtifactState(Player player);
public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects);
public override void AddOnTileEffectsPostEvents(Player player, Permutation permutation, Tile tile, List<Effect> effects);
public override void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects);
public override void AppendDiscardTileEffects(Player player, Tile tile, List<IAnimationEffect> onDiscardTileEffects, bool withForce, bool isClone);
public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects);
public override void AppendPostBlockAnimationEffects(Player player, Permutation permutation, Block block, List<IAnimationEffect> effects);
public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects);
public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects);
public override void SubscribeToPlayer(Player player);
public override void UnsubscribeToPlayer(Player player);
```

## LuaArtifactBuilder

Namespace: `Aotenjo` · [源码 / source](../src/API/Artifact/LuaArtifactBuilder.cs)

```csharp
public static LuaArtifactBuilder Create(string name, Rarity rarity);
public LuaArtifactBuilder WithHighlight(Func<Tile, Player, Artifact, bool> f);
public LuaArtifactBuilder WithAvailability(Func<Player, Artifact, bool> f);
public LuaArtifactBuilder WithDescription(Func<Player, Func<string, string>, Artifact, string> f);
public LuaArtifactBuilder WithInShopDescription(Func<Player, Func<string, string>, Artifact, string> f);
public LuaArtifactBuilder WithAdditionalInfo(Func<Player, Artifact, (string, double)> f);
public LuaArtifactBuilder WithNameWithColor(Func<Player, Artifact, string> f);
public LuaArtifactBuilder WithName(Func<Player, Artifact, string> f);
public LuaArtifactBuilder WithSubHeader(Func<Player, Artifact, string> f);
public LuaArtifactBuilder WithSpriteID(Func<Player, Artifact, string> f);
public LuaArtifactBuilder OnObtain(Action<Player, Artifact> f);
public LuaArtifactBuilder OnRemoved(Action<Player, Artifact> f);
public LuaArtifactBuilder PreGameInitialized(Action<Player, Artifact> f);
public LuaArtifactBuilder ResetArtifactState(Action<Player, Artifact> f);
public LuaArtifactBuilder OnTileEffect(Action<Player, Permutation, Tile, List<Effect>, Artifact> f);
public LuaArtifactBuilder OnTilePostEffect(Action<Player, Permutation, Tile, List<Effect>, Artifact> f);
public LuaArtifactBuilder OnDiscardTileEffect(Action<Player, Tile, List<IAnimationEffect>, bool, bool, Artifact> f);
public LuaArtifactBuilder OnUnusedTileEffect(Action<Player, Permutation, Tile, List<Effect>, Artifact> f);
public LuaArtifactBuilder OnBlockEffect(Action<Player, Permutation, Block, List<Effect>, Artifact> f);
public LuaArtifactBuilder OnBlockAnimEffect(Action<Player, Permutation, Block, List<IAnimationEffect>, Artifact> f);
public LuaArtifactBuilder OnSelfEffect(Action<Player, Permutation, List<Effect>, Artifact> f);
public LuaArtifactBuilder OnRoundEndEffect(Action<Player, Permutation, List<IAnimationEffect>, Artifact> f);
public LuaArtifactBuilder OnSubscribeToPlayer(Action<Player, Artifact> f);
public LuaArtifactBuilder OnUnsubscribeToPlayer(Action<Player, Artifact> f);
public LuaArtifactBuilder WithDeckIn(string[] ids);
public LuaArtifactBuilder WithDeckBlocked(string[] ids);
public LuaArtifactBuilder WithSetIn(string[] ids);
public LuaArtifactBuilder WithSetBlocked(string[] ids);
public LuaArtifactBuilder WithMaterialRequired(string[] ids);
public LuaArtifact Build();
public LuaCraftableArtifact BuildCraftable();
public LuaCraftableArtifact BuildAndRegisterCraftable();
public LuaArtifact BuildAndRegister();
```

## LuaArtifactRecipeBuilder

Namespace: `Aotenjo` · [源码 / source](../src/API/Artifact/LuaArtifactRecipeBuilder.cs)

```csharp
public static ArtifactRecipe Build(string id, List<Artifact> materials, Artifact result);
public static ArtifactRecipe BuildAndRegister(string id, List<Artifact> materials, Artifact result);
```

## LuaCraftableArtifact

Namespace: `Aotenjo` · [源码 / source](../src/API/Artifact/LuaCraftableArtifact.cs)

```csharp
public string GetDataOrDefault(string key, string defaultValue);
public void SetData(string key, string value);
public override void Deserialize(string data);
public override string Serialize();
public LuaCraftableArtifact( string name, Rarity rarity,  Func<Tile, Player, Artifact, bool> shouldHighlightTile = null, Func<Player, Artifact, bool> isAvailableGlobally = null, Func<Player, Func<string, string>, Artifact, string> getDescription = null, Func<Player, Func<string, string>, Artifact, string> getInShopDescription = null, Func<Player, Artifact, (string, double)> getAdditionalInfo = null, Func<Player, Artifact, string> getNameWithColor = null, Func<Player, Artifact, string> getName = null, Func<Player, Artifact, string> getSubHeader = null, Action<Player, Artifact> onObtain = null, Action<Player, Artifact> onRemoved = null, Action<Player, Artifact> preGameInitialized = null, Action<Player, Artifact> resetArtifactState = null, Action<Player, Permutation, Tile, List<Effect>, Artifact> onTileEffect = null, Action<Player, Permutation, Tile, List<Effect>, Artifact> onTilePostEffect = null, Action<Player, Tile, List<IAnimationEffect>, bool, bool, Artifact> onDiscardTileEffect = null, Action<Player, Permutation, Tile, List<Effect>, Artifact> onUnusedTileEffect = null, Action<Player, Permutation, Block, List<Effect>, Artifact> onBlockEffect = null, Action<Player, Permutation, Block, List<IAnimationEffect>, Artifact> onBlockAnimEffect = null, Action<Player, Permutation, List<Effect>, Artifact> onSelfEffect = null, Action<Player, Permutation, List<IAnimationEffect>, Artifact> onRoundEndEffect = null, Func<Player, Artifact, string> getSpriteID = null, Action<Player, Artifact> onSubscribeToPlayer = null, Action<Player, Artifact> onUnsubscribeToPlayer = null );
public override string GetSpriteNamespaceID(Player player, string nmSpace = "unknown_mod");
public override bool ShouldHighlightTile(Tile tile, Player player);
public override bool IsAvailableGlobally(Player player);
public override string GetDescription(Func<string, string> localizer);
public override string GetDescription(Player player, Func<string, string> localizer);
public override string GetInShopDescription(Player player, Func<string, string> localizer);
public override (string, double) GetAdditionalDisplayingInfo(Player player);
public override string GetNameWithColor(Func<string, string> localizer);
public override string GetName(Func<string, string> localizer);
public override string GetSubHeader(Player player, Func<string, string> loc);
public override void OnObtain(Player player);
public override void OnRemoved(Player player);
public override void PreGameInitialized(Player player);
public override void ResetArtifactState(Player player);
public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects);
public override void AddOnTileEffectsPostEvents(Player player, Permutation permutation, Tile tile, List<Effect> effects);
public override void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects);
public override void AppendDiscardTileEffects(Player player, Tile tile, List<IAnimationEffect> onDiscardTileEffects, bool withForce, bool isClone);
public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects);
public override void AppendPostBlockAnimationEffects(Player player, Permutation permutation, Block block, List<IAnimationEffect> effects);
public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects);
public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects);
public override void SubscribeToPlayer(Player player);
public override void UnsubscribeToPlayer(Player player);
```

## MiniBrushEvent

Namespace: `global / 全局` · [源码 / source](../src/API/Event/MiniBrushEvent.cs)

```csharp
public MiniBrushEvent(Gadget gadgetPtr, Tile tile);
```

## SoundEvent

Namespace: `Aotenjo` · [源码 / source](../src/API/Event/SoundEvent.cs)

```csharp
public SoundEvent(string soundName);
```

## Logger

Namespace: `Aotenjo` · [源码 / source](../src/API/Logger.cs)

```csharp
public static void Log(string message);
public static void LogWarning(string message);
public static void LogError(string message);
```

## MessageManager

Namespace: `Aotenjo` · [源码 / source](../src/API/MessageManager.cs)

```csharp
public static void RefreshEventBus();
public void OnAddTileEvent(List<Tile> tiles);
public void OnRemoveTileEvent(List<Tile> tiles);
public void OnOpenPirateChest(List<PirateChestReward> rewards, Player player);
public void OnSoundEvent(string soundName);
public void OnActivateArtifactEvent(Artifact artifact, Effect effect);
public void OnUseMiniBrushEvent(Gadget gadgetPtr, Tile targetTile);
public void OnUpgradeYakuEvent(YakuType type, int levelBefore, int levelAfter);
public void OnUnfreezeEvent(Player player);
public void OnUseRookEvent(RookGadget rookGadget, Tile tile);
public void OnDrawTiles(List<int> posDrew);
public void OnOpenYakuPack(int v);
public void OnTilesEnterHand(List<Tile> tiles);
public void OnSneakTile(Tile tile, SneakyPlayer player);
public void OnCompleteAchievement(string id);
public PlayerStats GetGlobalPlayerStats();
public void EnqueueToDiscard(Tile t, bool forced);
public void OnSpendMoney(int v);
public void OnUseFishingRodEvent(PlayerGadgetEvent evt);
public void OnSetProgressBarLength(float percentage);
public void OnChangeTileMask(Tile tile, string mask);
public void OnArtifactEarnMoney(int money, Artifact artifact);
```

## Mod

Namespace: `Aotenjo` · [源码 / source](../src/API/Mod/Mod.cs)

```csharp
public Mod(string name, string version, string author, string description);
public string GetModInfo();
public string GetRootDir();
public static Mod LoadFromDirectory(string modDir);
```

## LuaMaterialSet

Namespace: `Aotenjo` · [源码 / source](../src/API/TileProperties/LuaMaterialSet.cs)

```csharp
public override void SubscribeToPlayerEvents(Player player);
public override void UnsubscribeToPlayerEvents(Player player);
public override LotteryPool<TileMaterial> GenerateCommonMaterialPool();
public override LotteryPool<TileMaterial> GenerateRareMaterialPool();
public static LuaMaterialSet Create(int id, string regName, List<string> availableMaterials, Func<MaterialSet, LotteryPool<TileMaterial>> onGenerateRareMaterialPool, Func<MaterialSet, LotteryPool<TileMaterial>> onGenerateCommonMaterialPool, Action<Player, MaterialSet> onPlayerUnsubscribe, Action<Player, MaterialSet> onPlayerSubscribe);
```

## LuaMaterialSetBuilder

Namespace: `Aotenjo` · [源码 / source](../src/API/TileProperties/LuaMaterialSetBuilder.cs)

```csharp
public static LuaMaterialSetBuilder Create(string regName);
public LuaMaterialSetBuilder WithAvailableMaterials(string[] materials);
public LuaMaterialSetBuilder AddMaterial(string material);
public LuaMaterialSetBuilder OnGenerateRare(Func<MaterialSet, LotteryPool<TileMaterial>> f);
public LuaMaterialSetBuilder OnGenerateCommon(Func<MaterialSet, LotteryPool<TileMaterial>> f);
public LuaMaterialSetBuilder OnSubscribe(Action<Player, MaterialSet> f);
public LuaMaterialSetBuilder OnUnsubscribe(Action<Player, MaterialSet> f);
public LuaMaterialSet Build();
public LuaMaterialSet BuildAndRegister();
```

## LuaTileMaterial

Namespace: `Aotenjo` · [源码 / source](../src/API/TileProperties/LuaTileMaterial.cs)

```csharp
public override TileMaterial Copy();
public override Rarity GetRarity();
public override bool IsDebuff();
public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects);
public override void AppendUnusedEffects(Player player, Permutation perm, List<Effect> effects);
public override void AppendToListOnTileUnusedEffect(Player player, Permutation perm, List<Effect> effects, Tile scoringTile, Tile onEffectTile);
public override void AppendToListRoundEndEffect(Player player, Permutation perm, List<IAnimationEffect> effects, Tile tile);
public override void AppendToListDiscardEffect(Player player, Permutation perm, List<IAnimationEffect> effects, Tile tile, bool withForce, bool isClone);
public override string GetDescription(Func<string, string> localizer, Player player);
public override void SubscribeToPlayerEvents(Player player);
public override void UnsubscribeToPlayerEvents(Player player);
public LuaTileMaterial(int ID, string nameKey);
public string GetDataOrDefault(string key, string defaultValue);
public void SetData(string key, string value);
```

## LuaTileMaterialBuilder

Namespace: `Aotenjo` · [源码 / source](../src/API/TileProperties/LuaTileMaterialBuilder.cs)

```csharp
public static LuaTileMaterialBuilder Create(string nameKey);
public LuaTileMaterialBuilder WithRarity(Rarity rarity);
public LuaTileMaterialBuilder WithData(string key, string value);
public LuaTileMaterialBuilder WithDebuff(Func<TileMaterial, bool> f);
public LuaTileMaterialBuilder OnScoringEffect(Action<Player, Permutation, Tile, List<Effect>, TileMaterial> f);
public LuaTileMaterialBuilder OnUnusedEffect(Action<Player, Permutation, List<Effect>, TileMaterial> f);
public LuaTileMaterialBuilder OnDerivedTileUnusedEffect(Action<Player, Permutation, List<Effect>, Tile, Tile, TileMaterial> f);
public LuaTileMaterialBuilder OnRoundEndEffect(Action<Player, Permutation, List<IAnimationEffect>, Tile, TileMaterial> f);
public LuaTileMaterialBuilder OnDiscardEffect(Action<Player, Permutation, List<IAnimationEffect>, Tile, bool, bool, TileMaterial> f);
public LuaTileMaterialBuilder WithDescription(Func<Func<string, string>, Player, TileMaterial, string> f);
public LuaTileMaterialBuilder OnSubscribe(Action<Player, TileMaterial> f);
public LuaTileMaterialBuilder OnUnsubscribe(Action<Player, TileMaterial> f);
public LuaTileMaterial Build();
public LuaTileMaterial BuildAndRegister();
```

## CustomYakuBuilder

Namespace: `Aotenjo` · [源码 / source](../src/API/Yaku/CustomYakuBuilder.cs)

```csharp
public static YakuType RegisterCustomYaku(string id, int baseFan, double growthFactor, double levelingFan, Func<Permutation, Player, bool> predicate, string[] includedYakus, string[] groups, int[] yakuCategories, Rarity rarity, string exampleTiles);
public static void AddInheritanceRelation(string nativeYakuTypeID, YakuType[] inheritedYakuTypes);
```

## Artifact

Namespace: `Aotenjo` · [源码 / source](../src/Artifact/Artifact.cs)

```csharp
public Artifact(string name, Rarity rarity, Action<Player, Permutation, Tile, List<Effect>> onTileEffect, Action<Player, Permutation, Block, List<Effect>> onBlockEffect, Action<Player, Permutation, List<Effect>> selfEffect);
public Artifact(string name, Rarity rarity);
public virtual bool ShouldHighlightTile(Tile tile, Player player);
public bool IsUnlocked(PlayerStats stats);
public string GetRegName();
public virtual string GetSpriteNamespaceID(Player player, string nmSpace = "aotenjo");
public static (string, double) ToAddFanFormat(double input);
public static (string, double) ToAddFuFormat(double input);
public static (string, double) ToMulFanFormat(double input);
public Artifact SetHighlightRequirement(Func<Tile, Player, bool> pred);
public Artifact SetPrerequisite(Predicate<Player> pred);
public Artifact SetMaterialGift(Func<TileMaterial> materialFactory, int count);
public Artifact SetFontGift(Func<TileFont> fontFactory, int count);
public virtual bool CanBeSellByPlayer();
public override string ToString();
public bool IsCrafted();
public virtual bool IsUnique();
public virtual bool CanBeBoughtWithoutSlotLimit(Player player);
public virtual List<Artifact> GetComponents();
public virtual void ResetArtifactState(Player player);
public virtual void ResetArtifactState();
public virtual string GetDescription(Func<string, string> localizer);
public virtual string GetDescription(Player player, Func<string, string> localizer);
public virtual string GetInShopDescription(Player player, Func<string, string> localizer);
public string GetChanceMultiplier(Player player);
public virtual int GetBuyingPrice(bool firstTimeBuying);
public virtual int GetBasePrice();
public virtual bool IsAvailableGlobally(Player player);
public virtual bool IsAvailableGlobally(MahjongDeck deck, MaterialSet set);
public virtual bool IsAvailableGlobally(MahjongDeck deck);
public virtual bool IsAvailableGlobally(MaterialSet set);
public virtual bool IsAvailableInShops(Player player);
public virtual int GetSellingPrice();
public virtual string GetNameWithColor(Func<string, string> localizer);
public virtual string GetName(Func<string, string> localizer);
public virtual string GetNameKey();
public virtual int GetNumberID();
public Sprite GetSprite(Player player);
public virtual Rarity GetRarity();
public virtual bool CanObtainBy(Player player);
public virtual void OnObtain(Player player);
public virtual void OnRemoved(Player player);
public virtual (string, double) GetAdditionalDisplayingInfo(Player player);
public virtual Artifact SetBroken(Player player);
public virtual void SubscribeToPlayer(Player player);
public virtual void UnsubscribeToPlayer(Player player);
public virtual void PreGameInitialized(Player player);
public string GetNameID();
public string GetStory(Func<string, string> loc);
public string GetUnlockRequirementDescription(Func<string, string> loc);
public UnlockRequirement GetUnlockRequirement();
public virtual void AppendDiscardTileEffects(Player player, Tile tile, List<IAnimationEffect> onDiscardTileEffects, bool withForce, bool isClone);
public bool IsMaterial(Player player);
public bool IsCraftResult(Player player);
public virtual string GetSubHeader(Player player, Func<string, string> loc);
public virtual string Serialize();
public virtual void Deserialize(string data);
public virtual void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects);
public virtual void AddOnTileEffectsPostEvents(Player player, Permutation permutation, Tile tile, List<Effect> effects);
public virtual void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects);
public virtual void AppendPostBlockAnimationEffects(Player player, Permutation permutation, Block block, List<IAnimationEffect> effects);
public virtual void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects);
public virtual void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects);
public virtual void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects);
public static Artifact CreateOnTileEffectArtifact(string name, Rarity rarity, Action<Player, Permutation, Tile, List<Effect>> action);
public static Artifact CreateOnBlockEffectArtifact(string name, Rarity rarity, Action<Player, Permutation, Block, List<Effect>> action);
public static Artifact CreateOnSelfEffectArtifact(string name, Rarity rarity, Action<Player, Permutation, List<Effect>> action);
```

## Artifacts

Namespace: `Aotenjo` · [源码 / source](../src/Artifact/Artifacts.cs)

```csharp
public static Artifact GetArtifact(string name);
```

## ArtifactRecipe

Namespace: `Aotenjo` · [源码 / source](../src/ArtifactRecipe/ArtifactRecipe.cs)

```csharp
public virtual bool CheckFulfillRecipeRequirement(Player player);
public virtual void OnFulfillRecipeResult(Player player);
public static ArtifactRecipe Create(string name, List<string> inputID, string outputID);
public static ArtifactRecipe Create(string name, List<Artifact> inputID, Artifact outputID);
```

## ArtifactEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/ArtifactEffect.cs)

```csharp
public ArtifactEffect(string name, Artifact artifact);
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
```

## ChangeSuitEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/ChangeSuitEffect.cs)

```csharp
public ChangeSuitEffect(Tile tile, Tile.Category category);
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
```

## CleanseEffect

Namespace: `global / 全局` · [源码 / source](../src/Effects/CleanseEffect.cs)

```csharp
public CleanseEffect(Artifact artifact, Tile tile);
public override string GetEffectDisplay(Func<string, string> func);
public override string GetSoundEffectName();
public override Artifact GetEffectSource();
public override void Ingest(Player player);
```

## ConditionalEffect

Namespace: `global / 全局` · [源码 / source](../src/Effects/ConditionalEffect.cs)

```csharp
public ConditionalEffect(Func<Player, bool> condition, Effect effect, string descriptionKey);
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
```

## CorruptEffect

Namespace: `global / 全局` · [源码 / source](../src/Effects/CorruptEffect.cs)

```csharp
public CorruptEffect(Tile tile);
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
public override string GetSoundEffectName();
```

## DerivedEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/DerivedEffect.cs)

```csharp
public DerivedEffect(Effect effect);
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
public override bool ShouldWaitUntilFinished();
```

## DuplicateTileEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/DuplicateTileEffect.cs)

```csharp
public DuplicateTileEffect(Artifact artifact, Tile tile);
public DuplicateTileEffect SetWithNoProperties();
public override void Ingest(Player player);
```

## EarnMoneyEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/EarnMoneyEffect.cs)

```csharp
public EarnMoneyEffect(int amount);
public EarnMoneyEffect(int amount, Artifact artifact);
public override bool NoDefaultSound();
public override string GetSoundEffectName();
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
```

## Effect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/Effect.cs)

```csharp
public abstract string GetEffectDisplay(Func<string, string> func);
public abstract Artifact GetEffectSource();
public abstract void Ingest(Player player);
public virtual string GetEffectDisplay(Player player, Func<string, string> localizationMethod);
public virtual bool NoDefaultSound();
public virtual string GetSoundEffectName();
public virtual string GetEffectAnimationTrigger();
public virtual Effect GetEffect();
public virtual bool WillTrigger();
public virtual bool ShouldWaitUntilFinished();
public OnTileAnimationEffect OnTile(Tile tile, bool isClone = false);
public OnBlockAnimationEffect OnBlock(Block block, bool isClone = false);
public OnMultipleTileAnimationEffect OnMultipleTiles(List<Tile> tiles, Tile mainTile);
public MaybeEffect MaybeTriggerWithChance(int chance, string locKey);
public DerivedEffect AsDerivedEffect();
public bool HasTag(EffectTag tag);
public OnBossAnimationEffect OnBoss();
public void TriggerPostSyncEffect();
```

## EffectTag

Namespace: `Aotenjo` · [源码 / source](../src/Effects/EffectTag.cs)

```csharp
public static EffectTag Of(string tag);
public override bool Equals(object obj);
public override int GetHashCode();
```

## FractureEffect

Namespace: `global / 全局` · [源码 / source](../src/Effects/FractureEffect.cs)

```csharp
public FractureEffect(Artifact source, Tile target, string soundEffectName = "effect_fracture_name");
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
public override string GetSoundEffectName();
```

## FreezeEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/FreezeEffect.cs)

```csharp
public FreezeEffect(IceBladeArtifact artifact, Tile tile);
public override string GetSoundEffectName();
```

## GrowEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/GrowEffect.cs)

```csharp
public GrowEffect(Tile tile, Artifact artifact);
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
public override string GetSoundEffectName();
```

## GrowFuEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/GrowFuEffect.cs)

```csharp
public GrowFuEffect(Artifact artifact, Tile tile, int amount, string name = "grow_name");
public override void Ingest(Player player);
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override string GetSoundEffectName();
public override string GetEffectAnimationTrigger();
```

## IAnimationEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/IAnimationEffect.cs)

```csharp
public Effect GetEffect();
public virtual void TriggerPostSyncEffect();
```

## IncreaseTargetEffect

Namespace: `global / 全局` · [源码 / source](../src/Effects/IncreaseTargetEffect.cs)

```csharp
public IncreaseTargetEffect(double v1, string v2, Artifact artifact = null);
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
```

## IncreDiscardEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/IncreDiscardEffect.cs)

```csharp
public IncreDiscardEffect(string name, Artifact artifact, int num);
public override string GetEffectDisplay(Func<string, string> func);
public override void Ingest(Player player);
```

## LedEarnMoneyEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/LedEarnMoneyEffect.cs)

```csharp
public LedEarnMoneyEffect(int amount, LedDetectedColor detectedColor, int partSlot);
```

## MaybeEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/MaybeEffect.cs)

```csharp
public MaybeEffect(string effectName, int chance, Effect transformMaterialEffect);
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
```

## ScoreEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/ScoreEffect.cs)

```csharp
public ScoreEffect HideWhenZero();
public override bool WillTrigger();
public override bool NoDefaultSound();
public override string GetSoundEffectName();
public override string GetEffectDisplay(Func<string, string> func);
public override void Ingest(Player player);
public override Artifact GetEffectSource();
public static ScoreEffect AddFu(double val, Artifact source);
public static ScoreEffect AddFu(ValueSupplier val, Artifact source);
public static ScoreEffect AddFan(double val, Artifact source);
public static ScoreEffect AddFan(ValueSupplier val, Artifact source);
public static ScoreEffect MulFan(double val, Artifact source);
public static ScoreEffect MulFan(ValueSupplier val, Artifact source);
```

## SilentEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/SilentEffect.cs)

```csharp
public SilentEffect(Action action);
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
public override bool WillTrigger();
```

## SimpleEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/SimpleEffect.cs)

```csharp
public SimpleEffect(string text, Artifact source, Action<Player> consume, string soundName = "AddFu");
public SimpleEffect(Func<Player, Func<string, string>, string> provider, Artifact source, Action<Player> consume, string soundName = "AddFu");
public override void Ingest(Player player);
public override string GetEffectDisplay(Player player, Func<string, string> localizationMethod);
```

## SuppressEffect

Namespace: `global / 全局` · [源码 / source](../src/Effects/SuppressEffect.cs)

```csharp
public SuppressEffect(Tile tile);
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
public override string GetSoundEffectName();
```

## SuppressTileGroupAnimationEffect

Namespace: `global / 全局` · [源码 / source](../src/Effects/SuppressTileGroupAnimationEffect.cs)

```csharp
public SuppressTileGroupAnimationEffect(List<Tile> tiles, Tile tile);
public override List<Tile> GetAffectedTiles(Player player);
public override Tile GetMainTile(Player player);
```

## TextEffect

Namespace: `Aotenjo` · [源码 / source](../src/Effects/TextEffect.cs)

```csharp
public TextEffect(string text, Artifact source = null, string soundName = "AddFu");
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override string GetSoundEffectName();
public override void Ingest(Player player);
```

## TransformColorEffect

Namespace: `global / 全局` · [源码 / source](../src/Effects/TransformColorEffect.cs)

```csharp
public TransformColorEffect(TileFont font, Artifact artifact, Tile tile, string display);
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
```

## TransformEffect

Namespace: `global / 全局` · [源码 / source](../src/Effects/TransformEffect.cs)

```csharp
public TransformEffect(string name, TileProperties toBecome, Artifact source, Tile target);
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
```

## TransformMaskEffect

Namespace: `global / 全局` · [源码 / source](../src/Effects/TransformMaskEffect.cs)

```csharp
public TransformMaskEffect(TileMask mask, Artifact artifact, Tile tile, string display);
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
```

## TransformMaterialEffect

Namespace: `global / 全局` · [源码 / source](../src/Effects/TransformMaterialEffect.cs)

```csharp
public TransformMaterialEffect(TileMaterial mat, Artifact artifact, Tile tile, string display, string soundName = "AddFu");
public override void Ingest(Player player);
```

## UpgradePatternEffect

Namespace: `global / 全局` · [源码 / source](../src/Effects/UpgradePatternEffect.cs)

```csharp
public override string GetEffectDisplay(Func<string, string> func);
public override Artifact GetEffectSource();
public override void Ingest(Player player);
```

## EventBus

Namespace: `Aotenjo` · [源码 / source](../src/Event/NewEventSystem/EventBus.cs)

```csharp
public static void Subscribe<T>(Action<T> handler, int priority = 0, bool once = false);
public static void Subscribe<T>(Player player, Action<T> handler, int priority = 0, bool once = false);
public static void Subscribe<T>(Player player, Action<Permutation, Player, List<IAnimationEffect>> handler, int priority = 0, bool once = false);
public static void Subscribe<T>(Player player, Action<Permutation, Player, List<OnTileAnimationEffect>> handler, int priority = 0, bool once = false);
public static void Subscribe<T>(Player player, Action<Permutation, Player, List<OnTileAnimationEffect>, OnTileAnimationEffect, Tile> handler, int priority = 0, bool once = false);
public static void Subscribe<T>(Player player, Action<Player, List<IAnimationEffect>, IAnimationEffect> handler, int priority = 0, bool once = false);
public static void Subscribe<T>(Player player, Action<Player, List<IAnimationEffect>, Tile, bool> handler, int priority = 0, bool once = false);
public static void Subscribe<T>(Player player, Action<Permutation, Player, Effect> handler, int priority = 0, bool once = false);
public static void Subscribe<T>(Player player, Action<Player, Gadget> handler, int priority = 0, bool once = false);
public static void Subscribe<T>(Player player, Action<Player, List<Destination>> handler, int priority = 0, bool once = false);
public static void Subscribe<T>(Player player, Action<Player, Tile, TileMaterialDessert> handler, int priority = 0, bool once = false);
public static void SubscribeOnce<T>(Action<T> handler, int priority = 0);
public static void Unsubscribe<T>(Action<T> handler);
public static void Unsubscribe<T>(Player player, Action<T> handler);
public static void Unsubscribe<T>(Player player, Action<Permutation, Player, List<IAnimationEffect>> handler);
public static void Unsubscribe<T>(Player player, Action<Permutation, Player, List<OnTileAnimationEffect>> handler);
public static void Unsubscribe<T>(Player player, Action<Permutation, Player, List<OnTileAnimationEffect>, OnTileAnimationEffect, Tile> handler);
public static void Unsubscribe<T>(Player player, Action<Player, List<IAnimationEffect>, IAnimationEffect> handler);
public static void Unsubscribe<T>(Player player, Action<Player, List<IAnimationEffect>, Tile, bool> handler);
public static void Unsubscribe<T>(Player player, Action<Permutation, Player, Effect> handler);
public static void Unsubscribe<T>(Player player, Action<Player, Gadget> handler);
public static void Unsubscribe<T>(Player player, Action<Player, List<Destination>> handler);
public static void Unsubscribe<T>(Player player, Action<Player, Tile, TileMaterialDessert> handler);
public static void Publish<T>(T evt);
public static void ClearAll();
```

## Block

Namespace: `Aotenjo` · [源码 / source](../src/HandAndTile/Hand/Block/Block.cs)

```csharp
public Block();
public Block(Tile[] tiles);
public static bool IsFourWinds(Tile[] tiles);
public Block(string representation);
public virtual bool All(Predicate<Tile> predicate);
public virtual bool Any(Predicate<Tile> predicate);
public virtual bool CompatWithNumbers(string numbers);
public virtual bool IsABC();
public virtual bool IsAAA();
public virtual bool IsAAAA();
public bool IsNumbered();
public virtual Tile.Category GetCategory();
public virtual bool Succ(Block other, int step);
public virtual bool CompatWith(Block other);
public virtual bool CompatWithRepresentation(string representation);
public virtual bool IsAAAOf(string representation);
public virtual bool OfCategory(Tile.Category category);
public virtual bool OfSameCategory(Block other);
public virtual bool OfSameOrder(Block other);
public virtual string ToFormat();
public virtual string GetSpriteString();
public override string ToString();
public static Block FormValidBlock(Tile[] tiles, Player player);
public bool Kong(Tile tile);
public bool Kong(Tile tile, BlockCombinator combinator);
public bool Jumped(Tile tile, Player player);
public bool SelectingBy(Player player);
public Jiang(Tile tile1, Tile tile2);
public Jiang(string stringRepresentation);
public override string ToString();
public virtual string ToFormat();
public Tile.Category GetCategory();
public bool All(Predicate<Tile> predicate);
public bool Contains(Tile tile);
public bool Any(Predicate<Tile> predicate);
public PairBlock ToPairBlock();
public bool IsMirageOf(Block block1, Player status, bool catSensitive = false);
```

## Permutation

Namespace: `Aotenjo` · [源码 / source](../src/HandAndTile/Hand/Permutation/Permutation.cs)

```csharp
public Permutation(string representation);
public Permutation(Block[] array, Block.Jiang jiang);
public virtual double GetFu();
public virtual double GetFan(Player player);
public virtual double GetFan(Player player, List<YakuType> yakus);
public virtual double GetNumericalScore(Player player);
public virtual Score GetScore(Player player);
public virtual PermutationType GetPermType();
public virtual Block GetLastBlock();
public virtual List<YakuType> GetYakus(Player player, bool allContainedYakus = false);
public virtual List<YakuType> GetYakus(Player player, Predicate<YakuType> pred, bool allContainedYakus = false);
public Tile.Category[] GetCategoriesIncluded();
public bool ContainsYaku(YakuType yaku, Player player);
public bool ContainsYakuRecursive(YakuType yaku, Player player);
public virtual bool TilesFulfullAll(Predicate<Tile> tilePred);
public virtual bool BlocksFulfillAll(Predicate<Block> predicate);
public virtual bool JiangFulfillAll(Predicate<Tile> predicate);
public virtual bool JiangFulfillAny(Predicate<Tile> predicate);
public Tile.Category GetMostlyPlayedCategory();
public override String ToString();
public virtual Hand ToHand();
public virtual List<Tile> ToTiles();
public virtual Permutation DeepClone();
public virtual string ToFormat();
public virtual bool IsFullHand(Player player);
public Permutation Sort();
```

## Tile

Namespace: `Aotenjo` · [源码 / source](../src/HandAndTile/Tile/Tile.cs)

```csharp
public Tile(Tile tile);
public virtual Tile Copy();
public Tile(Category category, int order);
public Tile(Category category, int order, TileProperties properties);
public virtual double GetBaseFu();
public Category GetCategory();
public int GetOrder();
public virtual bool IsYaoJiu(Player player);
public virtual bool IsHonor(Player player);
public Tile SetMaterial(TileMaterial newMaterial, Player player, bool isCopy = false);
public Tile SetFont(TileFont newFont, Player player, bool isCopy = false);
public Tile SetMask(TileMask newMask, Player player, bool isCopy = false);
public Tile AddMask(TileMask newMask, Player player, bool isCopy = false);
public Tile SetProperties(TileProperties toBecome, Player player, bool isCopy = false);
public Tile(String representation);
public virtual void SubscribeToPlayerEvents(Player player);
public virtual void UnsubscribeFromPlayer(Player player);
public override String ToString();
public virtual int CompareTo(Tile o);
public virtual bool CompatWith(Tile cand);
public bool CompatWith(string representation);
public virtual bool IsSameCategory(Tile cand);
public virtual bool CompatWithCategory(Category cat);
public virtual bool IsSameOrder(Tile cand);
public virtual bool Succ(Tile a);
public bool IsNumbered();
public bool IsNumbered(int ord);
public static bool CategoryIsNumbered(Category tileCategory);
public static int CategoryToInteger(Category category);
public static string CategoryToNameKey(Category category);
public static Category GetCategoryFromChar(char c, int ord);
public static char GetCharFromCategory(Category category);
public bool IsRotationalSymmetric();
public virtual Pair<Category, int> GetLastVisibleDisplay();
public bool ContainsBlue(Player player);
public bool ContainsRed(Player player);
public bool ContainsGreen(Player player);
public bool FulfillAllGreen(Player player);
public bool ContainsNoColor(Player player);
public Tile ModifyOrder(int v, Player player);
public Tile SetOrderForced(int v);
public Tile ModifyCategory(Category category, Player player);
public Tile SetCategoryForced(Category category);
public Tile ModifyCarvedDesign(Tile tile, Player player);
public Tile ModifyCarvedDesign(Category newCat, int newOrd, Player player);
public int GetBaseOrder();
public string GetLocalizedName(Func<string, string> loc);
public string GetSpriteString();
public bool IsModified();
public Category GetBaseCategory();
public void AddTransform(TileTransform tileTransform, Player player);
public TileTransform GetLastTransform();
public void Suppress(Player player);
public bool CompatWithMaterial(TileMaterial mat, Player player);
public void AppendToListUnusedEffect(Player player, Permutation perm, List<Effect> effects);
public void AppendToListOnTileUnusedEffect(Player player, Permutation perm, List<Effect> effects, Tile onTile);
public void AddTransformForced(TileTransform transform);
public void ClearTransform(Player player);
public List<TileTransform> GetTransforms();
public virtual void AppendOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> onRoundEndEffects);
public void AppendDiscardEffects(Player player, Permutation permutation, List<IAnimationEffect> onDiscardTileEffects, bool withForce, Tile tile, bool isClone);
public bool IsPlayerWind(Player player);
public bool IsPrevalentWind(Player player);
```

## MaterialSet

Namespace: `Aotenjo` · [源码 / source](../src/HandAndTile/Tile/TileProperties/TileMaterial/MaterialSet/MaterialSet.cs)

```csharp
public List<TileMaterial> GetMaterials();
public string GetRegName();
public virtual string GetName(Func<string, string> loc);
public virtual bool IsUnlocked(PlayerStats globalStats);
public virtual void SubscribeToPlayerEvents(Player player);
public virtual void UnsubscribeToPlayerEvents(Player player);
public virtual LotteryPool<TileMaterial> GenerateCommonMaterialPool();
public virtual LotteryPool<TileMaterial> GenerateRareMaterialPool();
public static MaterialSet GetMaterialSet(string materialSet);
public virtual UnlockRequirement GetUnlockRequirement();
```

## TileMaterial

Namespace: `Aotenjo` · [源码 / source](../src/HandAndTile/Tile/TileProperties/TileMaterial/TileMaterial.cs)

```csharp
public TileMaterial(int id, string nameKey, Effect effect);
public virtual int GetOrnamentSpriteID(Player player);
public virtual int GetShadowID();
public override string GetShortLocalizeKey();
public virtual TileMaterial Copy();
public static TileMaterial Chocolate();
public static TileMaterial Ore();
public static TileMaterial Jade();
public static TileMaterial Agate();
public static TileMaterial Voidstone();
public static TileMaterial BlueAndWhitePorcelain();
public static TileMaterial BonePorcelain();
public static TileMaterial MysteriousColorPorcelain();
public static TileMaterial Ghost();
public static TileMaterial GoldMouse();
public static TileMaterial Taotie();
public static TileMaterial Succubus();
public static TileMaterial Nest();
public static TileMaterial Mo();
public static TileMaterial Demon();
public static TileMaterial NanmuWood();
public static TileMaterial PaleWood();
public static TileMaterial EmeraldWood();
public static TileMaterial MistWood();
public static TileMaterial HellWood();
public static TileMaterial JacarandaWood();
public static TileMaterial PaoRosaWood();
public static TileMaterial Butter();
public static TileMaterial ChocolateDessert();
public static TileMaterial Jelly();
public static TileMaterial MilleFeuille();
public static TileMaterial SugarCube();
public static TileMaterial IceCream();
public static TileMaterial Lollipop();
public static TileMaterial MechGear();
public static TileMaterial MechDriveRod();
public static TileMaterial MechNetworkCard();
public static TileMaterial MechLed();
public static TileMaterial MechShield();
public static TileMaterial MechIntegratedChip();
public static TileMaterial MechReactor();
public static TileMaterial[] Materials();
public static TileMaterial GetMaterial(string material);
public override string GetSubheader(Func<string, string> loc);
```

## YakuType

Namespace: `Aotenjo` · [源码 / source](../src/HandAndTile/Yaku/YakuType.cs)

```csharp
public YakuType(FixedYakuType fixedYakuType);
public YakuType(string customId);
public override bool Equals(object obj);
public override int GetHashCode();
public static YakuType FromString(string key);
public override string ToString();
public static implicit operator YakuType(FixedYakuType fixedType);
public override void WriteJson(JsonWriter writer, YakuType value, JsonSerializer serializer);
public override YakuType ReadJson(JsonReader reader, Type objectType, YakuType existingValue, bool hasExistingValue, JsonSerializer serializer);
```

## Player

Namespace: `Aotenjo` · [源码 / source](../src/Player/Player.cs)

```csharp
public void SetArtifactLimit(int n);
public void SetHandLimit(int n);
public Permutation GetAccumulatedPermutation();
public void SetCurrentAccumulatedBlock(Permutation perm);
public virtual List<Artifact> GetArtifacts();
public virtual List<Tile> GetTilePool();
public virtual String GetArtifactText();
public Player(List<Tile> tilePool, string randomSeed, MahjongDeck deck, MaterialSet set, int ascensionLevel = 0);
public void InitPointers();
public SkillSet GetSkillSet();
public YakuType[] GetLearntYakus();
public YakuType[] GetNonNativeYaku();
public YakuPackConsumeResult ConsumeYakuPack(YakuPack pack);
public YakuPackConsumeResult BuyAndConsumeYakuPack(YakuPack pack, int price);
public int GetHandLimit();
public int GetDiscardLimit();
public int GetArtifactLimit();
public List<Tile> GetHandDeckCopy();
public List<Tile> GetSelectedTilesCopy();
public virtual List<Tile> GetPlayingTiles();
public virtual List<Tile> GetScoringTiles(Permutation permutation);
public List<Tile> GetUnusedTilesInHand();
public double GetLevelTarget();
public double GetBasicLevelTarget();
public double GetBasicLevelTarget(int projectedLevel);
public void IncreaseTargetMultiplier(double v);
public void ResetScore();
public void ApplyEffect(Effect effect, Stack<IAnimationEffect> followingEffectStack = null);
public List<Effect> GetPostScoreEffectsFromTile(Permutation permutation, Tile tile);
public List<IAnimationEffect> GetScoreEffectsFromBlock(Permutation permutation, Block block);
public List<Effect> GetScoreEffectsFromArtifacts(Permutation permutation);
public List<Tile> DrawTilesFromPool(int n);
public List<Tile> DrawTilesFromPool(int n, Predicate<Tile> pred);
public List<Tile> DrawPlainTilesFromPool(int n);
public int GetLevelBaseBonusMoney();
public int GetAotenjoBonusMoney();
public int GetDiscardBonusMoney();
public int GetInterestBonusMoney();
public int GetRoundEndTotalMoney();
public bool DetermineForceDiscard(Tile tile);
public bool CanDiscardTile(Tile tile, bool forceDiscard, bool consumeDiscardChance);
public bool PreDiscardTile(Tile tile, bool forced);
public virtual int DiscardTile(Tile tile, bool forced);
public int MoveFromHandToDiscard(Tile tile);
public int MoveFromDiscardToPool(Tile tile);
public int MoveFromHandToPool(Tile tile);
public void AddPrioritizedDrawingTile(Tile tile);
public int DrawTileToHandDeck(bool sortDeck = true);
public int ReplaceTileAndKeepPosition(Tile tile);
public void SortDeck();
public virtual bool RemoveTileFromDiscarded(Tile toRemove, string message = "");
public bool RemoveTileFromPool(Tile toRemove);
public bool RemoveTileFromHand(Tile toRemove, bool forced = false, bool destroyed = false);
public void AddTileToPool(Tile toAdd);
public bool AddNewTileToPool(Tile toAdd);
public void AddTileToDiscarded(Tile newTile);
public void AddTileToHand(Tile toAdd);
public virtual void InitHandDeck();
public List<Tile> Play(Hand hand);
public void SkipSettle();
public bool OnRoundEndButtonPressed();
public bool OnRoundEnd();
public virtual void ResetTilePool();
public void EncounterNextBoss(Boss nextB);
public void SetCurrentBoss(Boss boss);
public void RestoreCurrentLevel();
public void SetCurrentLevel(GameLevel level);
public void RestartCurrentLevel();
public string GetOrCreateBossNameForLevel(int levelNumber);
public void GenerateNewUpcomingBosses();
public void SettleMoney();
public void EarnMoney(int money);
public Permutation GetCurrentSelectedPerm();
public void ClearCachedSelectedPerm();
public bool TrySetPair(Tile t1, Tile t2);
public List<Artifact> TryDrawRandomArtifact(int n);
public int BuyArtifact(Artifact artifact, bool reduced, int price);
public bool ObtainArtifact(Artifact artifact, bool forced = false);
public List<Tile> GetRiverTiles();
public bool SellArtifact(Artifact artifact);
public bool RemoveArtifact(Artifact artifact, bool resetArtifactState, bool reshuffleIntoPool = true);
public int GenerateRandomInt(int v, string category);
public Func<int, int> GetRng(string category);
public int GenerateRandomInt(int v);
public List<Tile> GenerateRandomTileWithEffects(int v, bool normal = false);
public List<Tile> GenerateRandomTileGroupWithEffects(int n, int normalWeight = 80, int commonWeight = 19, int epicWeight = 1, int fontedPercentage = 25, bool canGenerateHonorSeq = true, bool canBeMixed = true);
public List<PropertiesPack> GeneratePropertyPacks(int commonWeight = 98, int rareWeight = 2);
public TileProperties GenerateRandomTileProperties(int plainWeight, int commonWeight, int rareWeight, int fontedPercentage);
public void TriggerPrePostAddOnTileAnimationEffect(List<IAnimationEffect> effects);
public void TriggerPostAddOnTileAnimationEffect(List<OnTileAnimationEffect> effects);
public void TriggerPostAddOnArtifactAnimationEffect(List<IAnimationEffect> effects);
public void TriggerPostAddOnBlockAnimationEffect(List<IAnimationEffect> effects);
public void TriggerOnAddSingleAnimationEffectEvent(List<IAnimationEffect> neighbors, IAnimationEffect effect);
public virtual List<Tile> GetAllTiles();
public int GetSelectionCount();
public int GetMoney();
public void SpendMoney(int v);
public virtual void OnRoundStart();
public int GetCurrentBlockCount();
public bool Selecting(Tile tile);
public virtual bool IsPlayingTile(Tile tile);
public int GetGadgetLimit();
public bool AddGadget(Gadget gadget, bool allowPartial = false);
public void RemoveGadget(Gadget gadget);
public void SetArtifactOrder(Artifact[] array);
public void OnArtifactOrderChanged();
public void SetGadgets(List<Gadget> gadgets);
public List<Gadget> GenerateFreeGadgets();
public List<Gadget> GenerateGadgets(int n, bool inShop = true, int commonWeight = 10, int rareWeight = 2);
public List<Gadget> GenerateGadgets(int n, Predicate<Gadget> pred, bool inShop = true, int commonWeight = 9, int rareWeight = 2);
public int KongTile(Tile tile, Block block, Permutation perm);
public virtual void OnKong(Block block, Permutation perm);
public void DestroyYaku(YakuType yakuTypeID);
public void SetLevel(int level);
public void UpgradeYaku(YakuType yaku, int level);
public void SetAscensionLevel(int level);
public void ListAvailableArtifacts(int count);
public List<Block> GetCurrentSelectedBlocks();
public virtual BlockCombinator GetCombinator();
public int GetPlayerWind();
public int GetPrevalentWind();
public virtual List<YakuPack> TryDrawYakuPack(int drawCount, List<YakuPack> globalYakuPacks);
public void PostUsedGadget(Gadget gadget, Tile tile = null);
public Gadget GetLastUsedConsumableGadget();
public virtual List<Destination> GenerateDestinations();
public List<Artifact> DrawRandomArtifact(Rarity rarity, int count);
public List<Gadget> DrawRandomGadget(Rarity rarity, int count, bool inShop = true);
public void RedrawHandTiles(List<Tile> hand);
public PermutationType[] GetAvailablePermTypes();
public static string GetLevelTitle(Func<string, string> loc, int Level);
public int GetAscensionLevel();
public List<Effect> GetUnusedEffectsFromTile(Permutation perm, Tile tile);
public void SetHandTiles(Tile[] tiles);
public PlayerYakuEvent.Upgrade OnPreUpgradeYaku(YakuType yaku, int level);
public bool DetermineMaterialCompatibility(Tile tile, TileMaterial mat);
public bool DetermineFontCompatibility(Tile tile, TileFont font);
public bool DetermineTileCompatibility(Tile tile, int cat, int order);
public List<Effect> GetOnOtherTileUnusedEffectsFromTile(Permutation perm, Tile tile, Tile onTile);
public void OnChoosePath(Direction direction, IEnumerable<Destination> destinations);
public bool OnSetTransform(Tile tile, TileTransform tileTransform, Gadget gadget = null);
public List<Tile> GetInitialWall();
public int TileSettlingOrder(Tile t1, Permutation perm);
public void TriggerPreAddScoringEffectEvent(List<IAnimationEffect> inRoundAnimationQueue);
public void TriggerAddExtraScoringEffects(List<IAnimationEffect> inRoundAnimationQueue);
public List<Tile> GetSettledTiles();
public Tile RandomlyMergeTwoTile(Tile tile1, Tile tile2);
public List<Yaku> FindRelevantYakus(Skill.SkillType skill);
public int GetExtraLevel(Yaku yaku);
public virtual string GetExtraInformationFromTile(Tile tile, Func<string, string> loc);
public List<StarterBoostEffect> GetStarterBoosts();
public bool CanInsertGadgets(List<Gadget> gadgetsToInsert);
public bool CanInsertArtifacts(List<Artifact> artifactsToInsert);
public void OnChangeMaterial(Tile tile, TileMaterial newMaterial, bool isCopy);
public void OnChangeFont(Tile tile, TileFont newFont, bool isCopy);
public bool OnChangeMask(Tile tile, TileMask newMask, bool isCopy);
public void OnchangeProperties(Tile tile, TileProperties toBecome, bool isCopy);
public void PreChangedProperties(Tile tile, TileProperties newProperties);
public virtual int GetMaxPlayingStage();
public Block GenerateRandomBlock();
public virtual void AppendOnRoundEndEffect(List<IAnimationEffect> onRoundEndEffects);
public virtual void AppendAdditionalTileRoundEndEffects(List<IAnimationEffect> effects, Permutation permutation);
public void TriggerOnAddRoundEndAnimationEffectEvent(List<IAnimationEffect> onRoundEndEffects);
public virtual void AppendDiscardTileEffect(List<IAnimationEffect> onDiscardTileEffects, Tile tile, bool withForce, bool isClone);
public void TriggerOnAddDiscardTileAnimationEffectEvent(List<IAnimationEffect> onDiscardTileEffects, Tile tile, bool withForce);
public bool OnPreModifyCarvedDesign(Tile t, Category newCat, int newOrd);
public void OnPostModifyCarvedDesign(Tile tile, Category cat, int order);
public void TriggerDessertTileConsumedEvent(Tile tile, TileMaterialDessert dessert);
public bool TriggerDessertTileConsumeAttemptEvent(Tile tile, TileMaterialDessert dessert);
public bool IsArtifactDebuffed(Artifact artifact);
public Boss GetNextBoss();
public Boss GetBossAtRound(int prevalentWind);
public void SyncSelectingTiles(List<Tile> tiles);
public void SetGadgetLimit(int v);
public void ReplaceYakuPack(int from, int to);
public void UpgradeYakuPack(YakuPack from, YakuPack to);
public bool DetermineYaojiu(Tile tile);
public bool DetermineHonor(Tile tile);
public bool DetermineShiftedPair(Block b1, Block b2, int step, bool categorySensitive);
public bool IsPlayerWind(int v);
public bool IsPrevalentWind(int v);
public virtual bool GenerateRandomDeterminationResult(int v);
public virtual List<Tile> GetUniqueFullDeck();
public virtual List<Tile> GetFullDeck();
public bool HarderBossesEnabled();
public virtual bool CanSelectTile(Tile tile);
public void TriggerPreSettlePermutationEvent();
public virtual void TriggerPostSettlePermutationEvent(Permutation permutation);
public void TriggerPreAppendSettleScoringEffectsEvent();
public void TriggerOnAddSingleTileAnimationEffectEvent(Permutation perm, List<OnTileAnimationEffect> tileAnimationQueue, OnTileAnimationEffect eff, Tile tile);
public void SetMaterial(Tile[] tiles, TileMaterial material);
public void DestroyWall();
public void SetFont(Tile[] tiles, TileFont font);
public void CopyTile(Tile[] tiles);
public void AddTiles(Tile[] tiles);
public double GetFanForYaku(YakuType yakuType, Permutation permutation);
public virtual double GetYakuMultiplier(YakuType yakuType);
public void UpgradeYakuFromYakuPack(YakuPackConsumeResult res);
public void PostReadIBook(IBook book, List<Yaku> drawnYakus);
public int GetYakuPackResultCount();
public void PostUpgradeJade(IJade jade);
public int GetEffectiveJadeStack(IJade jade);
public bool DetermineNeighborArtifacts(Artifact artifactLeft, Artifact right);
public void TriggerOnAddSingleTileScoringEffectEvent(List<IAnimationEffect> effects, Tile tile, Permutation permutation);
public int GetYakuPackPrice(IBook yakuPack);
public double GetBaseFuOfTile(Tile tile);
public virtual List<IAnimationEffect> GetBaseEffectFromTile(Tile tile);
public virtual bool EraseBlock(Block block);
public virtual void PostRoundStart();
public List<(YakuPack, YakuPack)> GenerateYakuPackUpgradeOptions(YakuPack[] globalTable);
```

## SkillSet

Namespace: `Aotenjo` · [源码 / source](../src/Player/SkillSet.cs)

```csharp
public void SetPlayer(Player p);
public static SkillSet NewSkillSet();
public static SkillSet StandardSkillSet();
public static SkillSet ExtendedSkillSet();
public static SkillSet ScarletSkillSet();
public static SkillSet RainbowSkillSet();
public static SkillSet ShortenedSkillSet();
public YakuPackConsumeResult Consume(YakuPack pack, Player p);
public void SetLevel(YakuType yaku, int level);
public void AddLevel(YakuType yaku, int level);
public void IncreaseLevel(YakuType yaku);
public void DecreaseLevel(YakuType yaku);
public void TryUnlockYakuIfLocked(YakuType yaku, bool fullHand);
public int GetUnlockProgress(YakuType yaku);
public int GetLevel(YakuType yaku);
public int GetUnlockStatus(YakuType yaku);
public int GetLevel(Yaku yaku);
public int GetExtraLevel(YakuType yakuType);
public double CalculateFan(YakuType yaku, int blockCount, int extraLevel = 0);
public double CalculateInheritedFan(YakuType yaku, int blockCount);
public double CalculateIncrementFanFromLevel(YakuType yaku, int blockCount, int extraLevel = 0);
public YakuType[] GetYakus();
public Yaku[] GetUnlockedYakus();
public YakuPackConsumeResult ConsumeMultiple(IEnumerable<YakuPack> pack, Player player);
```

## LotteryPool

Namespace: `global / 全局` · [源码 / source](../src/Utils/LotteryPool.cs)

```csharp
public virtual LotteryPool<T> Add(LotteryPool<T> item, int weight);
public virtual LotteryPool<T> Add(T item, int weight);
public virtual LotteryPool<T> AddRange(IEnumerable<T> items, int individualWeights = 1);
public virtual T Draw(Func<int, int> rng, bool withReplacement = true);
public virtual List<T> DrawRange(Func<int, int> rng, int count, bool withReplacement = true);
public virtual void Clear();
public virtual bool IsEmpty();
public LotteryItem(T item);
public override T Draw(Func<int, int> rng, bool withReplacement);
public static T DrawFromCollection(IEnumerable<T> items, Func<int, int> rng);
public static List<T> DrawFromCollection(IEnumerable<T> items, Func<int, int> rng, int count);
```
