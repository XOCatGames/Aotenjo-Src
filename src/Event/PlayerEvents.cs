using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// Player 通过 EventBus 发布的事件类型。
    /// </summary>
    public static class PlayerEvents
    {
        public class PreSettlePermutationEvent : PlayerPermutationEvent
        {
            public PreSettlePermutationEvent(Player player, Permutation permutation) : base(player, permutation) { }
        }

        public class PreAppendSettleScoringEffectsEvent : PlayerPermutationEvent
        {
            public PreAppendSettleScoringEffectsEvent(Player player, Permutation permutation) : base(player, permutation) { }
        }

        public class PostSettlePermutationEvent : PlayerPermutationEvent
        {
            public PostSettlePermutationEvent(Player player, Permutation permutation) : base(player, permutation) { }
        }

        public abstract class PermutationAnimationEvent : PlayerPermutationEvent
        {
            public List<IAnimationEffect> effects;

            protected PermutationAnimationEvent(Player player, Permutation permutation, List<IAnimationEffect> effects)
                : base(player, permutation)
            {
                this.effects = effects;
            }
        }

        public class OnPrePostAddOnTileAnimationEffectEvent : PermutationAnimationEvent
        {
            public OnPrePostAddOnTileAnimationEffectEvent(Player player, Permutation permutation,
                List<IAnimationEffect> effects) : base(player, permutation, effects) { }
        }

        public class OnPostAddOnTileAnimationEffectEvent : PlayerPermutationEvent
        {
            public List<OnTileAnimationEffect> effects;

            public OnPostAddOnTileAnimationEffectEvent(Player player, Permutation permutation,
                List<OnTileAnimationEffect> effects) : base(player, permutation)
            {
                this.effects = effects;
            }
        }

        public class OnAddSingleTileScoringEffectEvent : PermutationAnimationEvent
        {
            public Tile tile;

            public OnAddSingleTileScoringEffectEvent(Player player, Permutation permutation,
                List<IAnimationEffect> effects, Tile tile) : base(player, permutation, effects)
            {
                this.tile = tile;
            }
        }

        public class PostAddSingleTileAnimationEffectEvent : PlayerPermutationEvent
        {
            public List<OnTileAnimationEffect> effects;
            public OnTileAnimationEffect effect;
            public Tile tile;

            public PostAddSingleTileAnimationEffectEvent(Player player, Permutation permutation,
                List<OnTileAnimationEffect> effects, OnTileAnimationEffect effect, Tile tile)
                : base(player, permutation)
            {
                this.effects = effects;
                this.effect = effect;
                this.tile = tile;
            }
        }

        public class PreAddScoringAnimationEffectEvent : PermutationAnimationEvent
        {
            public PreAddScoringAnimationEffectEvent(Player player, Permutation permutation,
                List<IAnimationEffect> effects) : base(player, permutation, effects) { }
        }

        public class OnPostAddOnBlockAnimationEffectEvent : PermutationAnimationEvent
        {
            public OnPostAddOnBlockAnimationEffectEvent(Player player, Permutation permutation,
                List<IAnimationEffect> effects) : base(player, permutation, effects) { }
        }

        public class OnPostAddScoringAnimationEffectEvent : PermutationAnimationEvent
        {
            public OnPostAddScoringAnimationEffectEvent(Player player, Permutation permutation,
                List<IAnimationEffect> effects) : base(player, permutation, effects) { }
        }

        public class OnPostAddRoundEndAnimationEffectEvent : PermutationAnimationEvent
        {
            public OnPostAddRoundEndAnimationEffectEvent(Player player, Permutation permutation,
                List<IAnimationEffect> effects) : base(player, permutation, effects) { }
        }

        public class OnAddSingleAnimationEffectEvent : PlayerEvent
        {
            public List<IAnimationEffect> effects;
            public IAnimationEffect effect;

            public OnAddSingleAnimationEffectEvent(Player player, List<IAnimationEffect> effects,
                IAnimationEffect effect) : base(player)
            {
                this.effects = effects;
                this.effect = effect;
            }
        }

        public class OnAddSingleDiscardTileAnimationEffectEvent : PlayerTileEvent
        {
            public List<IAnimationEffect> effects;
            public bool forced;

            public OnAddSingleDiscardTileAnimationEffectEvent(Player player, List<IAnimationEffect> effects,
                Tile tile, bool forced) : base(player, tile)
            {
                this.effects = effects;
                this.forced = forced;
            }
        }

        public class PostIngestEffectEvent : PlayerPermutationEvent
        {
            public Effect effect;
            public readonly List<Effect> followingEffects = new();
            public readonly List<Artifact> artifactsAtTrigger;

            public PostIngestEffectEvent(Player player, Permutation permutation, Effect effect,
                List<Artifact> artifactsAtTrigger = null)
                : base(player, permutation)
            {
                this.effect = effect;
                this.artifactsAtTrigger = artifactsAtTrigger ?? player.GetArtifacts();
            }
        }

        public class OnPreUpgradeYakuEvent : PlayerYakuEvent.Upgrade
        {
            public OnPreUpgradeYakuEvent(Player player, YakuType yakuType, int level)
                : base(player, yakuType, level) { }
        }

        public class SpendMoneyEvent : PlayerMoneyEvent
        {
            public SpendMoneyEvent(Player player, int amount) : base(player, amount) { }
        }

        public class EarnMoneyEvent : PlayerMoneyEvent
        {
            public EarnMoneyEvent(Player player, int amount) : base(player, amount) { }
        }

        public class PreRemoveArtifactEvent : PlayerArtifactEvent
        {
            public PreRemoveArtifactEvent(Player player, Artifact artifact) : base(player, artifact) { }
        }

        public class PreAddTileEvent : PlayerTileEvent
        {
            public PreAddTileEvent(Player player, Tile tile) : base(player, tile) { }
        }

        public class PostAddTileEvent : PlayerTileEvent
        {
            public PostAddTileEvent(Player player, Tile tile) : base(player, tile) { }
        }

        public class ObtainYakuEvent : PlayerYakuEvent.Obtain
        {
            public ObtainYakuEvent(Player player, YakuType yakuType) : base(player, yakuType) { }
        }

        public class UpgradeYakuEvent : PlayerYakuEvent.Upgrade
        {
            public UpgradeYakuEvent(Player player, YakuType yakuType, int level) : base(player, yakuType, level) { }
        }

        public class DeleteYakuEvent : PlayerYakuEvent.Delete
        {
            public DeleteYakuEvent(Player player, YakuType yakuType, int level) : base(player, yakuType, level) { }
        }

        public class RetrieveYakuMultiplierEvent : PlayerYakuEvent.RetrieveMultiplier
        {
            public RetrieveYakuMultiplierEvent(Player player, YakuType yakuType, double multiplier)
                : base(player, yakuType, multiplier) { }
        }

        public class ObtainGadgetEvent : PlayerGadgetEvent
        {
            public ObtainGadgetEvent(Player player, Gadget gadget) : base(player, gadget) { }
        }

        public class PreObtainArtifactEvent : PlayerArtifactEvent.DetermineGettability
        {
            public PreObtainArtifactEvent(Player player, Artifact artifact, bool result) : base(player, artifact, result) { }
        }

        public class PostObtainArtifactEvent : PlayerArtifactEvent
        {
            public PostObtainArtifactEvent(Player player, Artifact artifact) : base(player, artifact) { }
        }

        public class PostUseGadgetEvent : PlayerGadgetEvent
        {
            public PostUseGadgetEvent(Player player, Gadget gadget, Tile tile = null) : base(player, gadget, tile) { }
        }

        public class PreSetTransformEvent : PlayerSetTransformEvent
        {
            public PreSetTransformEvent(Player player, Gadget gadget, TileTransform transform, Tile tile)
                : base(player, gadget, transform, tile) { }
        }

        public class PreSetMaterialEvent : PlayerSetAttributeEvent
        {
            public PreSetMaterialEvent(Player player, Tile tile, TileAttribute attribute, bool isCopy)
                : base(player, tile, attribute, isCopy) { }
        }

        public class PreSetFontEvent : PlayerSetAttributeEvent
        {
            public PreSetFontEvent(Player player, Tile tile, TileAttribute attribute, bool isCopy)
                : base(player, tile, attribute, isCopy) { }
        }

        public class PreSetMaskEvent : PlayerSetAttributeEvent
        {
            public PreSetMaskEvent(Player player, Tile tile, TileAttribute attribute, bool isCopy)
                : base(player, tile, attribute, isCopy) { }
        }

        public class PreSetPropertiesEvent : PlayerSetPropertiesEvent
        {
            public PreSetPropertiesEvent(Player player, Tile tile, TileProperties properties, bool isCopy)
                : base(player, tile, properties, isCopy) { }
        }

        public class PreSetTilePropertiesEvent : PlayerSetPropertiesEvent
        {
            public PreSetTilePropertiesEvent(Player player, Tile tile, TileProperties properties, bool isCopy)
                : base(player, tile, properties, isCopy) { }
        }

        public class PostSetTilePropertiesEvent : PlayerTileEvent
        {
            public readonly bool appliedDebuff;

            public PostSetTilePropertiesEvent(Player player, Tile tile, bool appliedDebuff)
                : base(player, tile)
            {
                this.appliedDebuff = appliedDebuff;
            }
        }

        public class DetermineTileSelectivityEvent : PlayerTileEvent
        {
            public DetermineTileSelectivityEvent(Player player, Tile tile) : base(player, tile) { }
        }

        public class PreKongTileEvent : PlayerKongTileEvent
        {
            public PreKongTileEvent(Player player, Tile tile, Permutation permutation, Block block)
                : base(player, tile, permutation, block) { }
        }

        public class DetermineMaterialCompatibilityEvent : PlayerDetermineMaterialCompatibilityEvent
        {
            public DetermineMaterialCompatibilityEvent(Player player, Tile tile, TileMaterial material)
                : base(player, tile, material) { }
        }

        public class DetermineFontCompatibilityEvent : PlayerDetermineFontCompatibilityEvent
        {
            public DetermineFontCompatibilityEvent(Player player, Tile tile, TileFont font) : base(player, tile, font) { }
        }

        public class DetermineTileCompatibilityEvent : PlayerDetermineTileFaceCompatibilityEvent
        {
            public DetermineTileCompatibilityEvent(Player player, Tile tile, int category, int order)
                : base(player, tile, category, order) { }
        }

        public class DetermineDiscardTileEvent : PlayerDiscardTileEvent.Determine
        {
            public DetermineDiscardTileEvent(Player player, Tile tile, bool result, bool forceDiscard,
                bool consumeDiscardChances = false)
                : base(player, tile, result, forceDiscard, consumeDiscardChances) { }
        }

        public class DetermineForceDiscardTileEvent : PlayerDiscardTileEvent.DetermineForce
        {
            public DetermineForceDiscardTileEvent(Player player, Tile tile, bool result, bool keepPosition = false)
                : base(player, tile, result, keepPosition) { }
        }

        public class DetermineSelectingTileEvent : DeterminePlayerSelectingTileEvent
        {
            public DetermineSelectingTileEvent(Player player, Tile tile, bool result) : base(player, tile, result) { }
        }

        public class PreDiscardTileEvent : PlayerDiscardTileEvent.Pre
        {
            public PreDiscardTileEvent(Player player, Tile tile, bool keepPosition = false)
                : base(player, tile, keepPosition) { }
        }

        public class PostDiscardTileEvent : PlayerDiscardTileEvent.Post
        {
            public PostDiscardTileEvent(Player player, Tile tile) : base(player, tile) { }
        }

        public class PreRemoveTileEvent : PlayerTileEvent
        {
            public PreRemoveTileEvent(Player player, Tile tile) : base(player, tile) { }
        }

        public class PostRemoveTileEvent : PlayerTileEvent
        {
            public PostRemoveTileEvent(Player player, Tile tile) : base(player, tile) { }
        }

        public class DetermineYaojiuTileEvent : PlayerTileEvent
        {
            public DetermineYaojiuTileEvent(Player player, Tile tile) : base(player, tile) { }
        }

        public class DetermineShiftedPairEvent : PlayerDetermineShiftedPairEvent
        {
            public DetermineShiftedPairEvent(Player player, Block first, Block second, int step,
                bool categorySensitive, bool result) : base(player, first, second, step, categorySensitive, result) { }
        }

        public class PostGenerateDestinationEvent : PlayerEvent
        {
            public List<Destination> destinations;

            public PostGenerateDestinationEvent(Player player, List<Destination> destinations) : base(player)
            {
                this.destinations = destinations;
            }
        }

        public class ChoosePathEvent : PlayerChoosePathEvent
        {
            public ChoosePathEvent(Player player, Direction direction, Destination[] destinations)
                : base(player, direction, destinations) { }
        }

        public class DeterminePlayerWindEvent : PlayerEvent
        {
            public DeterminePlayerWindEvent(Player player) : base(player) { }
        }

        public class DeterminePrevalentWindEvent : PlayerEvent
        {
            public DeterminePrevalentWindEvent(Player player) : base(player) { }
        }

        public class OnDessertTileConsumedEvent : PlayerTileEvent
        {
            public TileMaterialDessert dessert;

            public OnDessertTileConsumedEvent(Player player, Tile tile, TileMaterialDessert dessert) : base(player, tile)
            {
                this.dessert = dessert;
            }
        }

        public class OnDessertTileConsumeAttemptEvent : PlayerConsumeDessertEvent
        {
            public OnDessertTileConsumeAttemptEvent(Player player, Tile tile, TileMaterialDessert dessert)
                : base(player, tile, dessert) { }
        }

        public class PostUpgradeYakuFromIBookEvent : PlayerYakuEvent.ReadBookResult
        {
            public PostUpgradeYakuFromIBookEvent(Player player, Yaku[] results, IBook book) : base(player, results, book) { }
        }

        public class RetrieveEffectiveJadeStackEvent : PlayerJadeEvent.RetrieveEffectiveStack
        {
            public RetrieveEffectiveJadeStackEvent(Player player, IJade jade, int stack) : base(player, jade, stack) { }
        }
    }
}
