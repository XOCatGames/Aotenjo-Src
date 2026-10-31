using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Aotenjo
{
    public sealed class PinataArtifact : MechanicalArtifact
    {
        private const int GIFT_COUNT = 5;
        private static readonly int[] Requirements = { 5, 10, 20 };
        private static readonly int[] MoneyRewards = { 10, 15, 20 };
        private int stage;
        private int progress;
        private readonly HashSet<Tile> countedTiles = new();
        private readonly Dictionary<Tile, int> playedRods = new();
        [NonSerialized] private Player owner;

        public int Stage => stage;
        public int Progress => progress;
        public bool IsComplete => stage >= Requirements.Length;

        public PinataArtifact() : base("pinata", Rarity.RARE) =>
            SetMaterialGift(TileMaterial.MechDriveRod, GIFT_COUNT);

        public override void SubscribeToPlayer(Player player)
        {
            if (owner != null) UnsubscribeToPlayer(owner);
            base.SubscribeToPlayer(player);
            owner = player;
            EventBus.Subscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, BeginPlay);
            EventBus.Subscribe<PlayerEvents.PostSettlePermutationEvent>(player, EndPlay);
            EventBus.Subscribe<PlayerRoundEvent.Start.Pre>(player, ResetPlay);
            if (player is RainbowDeck.RainbowPlayer rainbow)
                rainbow.PostPlayFlowerTileEvent += EndFlowerPlay;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            EventBus.Unsubscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, BeginPlay);
            EventBus.Unsubscribe<PlayerEvents.PostSettlePermutationEvent>(player, EndPlay);
            EventBus.Unsubscribe<PlayerRoundEvent.Start.Pre>(player, ResetPlay);
            if (player is RainbowDeck.RainbowPlayer rainbow)
                rainbow.PostPlayFlowerTileEvent -= EndFlowerPlay;
            base.UnsubscribeToPlayer(player);
            owner = null;
            // Do not reset the current play here: obtaining a reward can rebind all artifacts mid-scoring.
        }

        private void ClearPlay() { countedTiles.Clear(); playedRods.Clear(); }
        private void ResetPlay(PlayerRoundEvent.Start.Pre evt) => ClearPlay();
        private void EndPlay(PlayerEvents.PostSettlePermutationEvent evt) => ClearPlay();
        private void EndFlowerPlay(Player player, FlowerTile flower)
        {
            countedTiles.Remove(flower);
            playedRods.Remove(flower);
        }

        private void BeginPlay(PlayerEvents.PreAppendSettleScoringEffectsEvent evt)
        {
            ClearPlay();
            foreach (Tile tile in evt.player.GetPlayingTiles())
                playedRods[tile] = CountRods(tile);
        }

        private static int CountRods(Tile tile) =>
            tile?.properties?.material is TileMaterialMechPart material
                ? material.GetPartCount(MechPartType.DriveRod) : 0;

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if ((owner != null && owner != player) || IsComplete || !player.IsPlayingTile(tile) || countedTiles.Contains(tile) ||
                tile.properties.mask is TileMaskSuppressed) return;
            int rods = playedRods.TryGetValue(tile, out int played) ? played : CountRods(tile);
            if (rods <= 0) return;
            effects.Add(MechanicalArtifactUtility.ActionEffect(this, "effect_pinata_hit", p =>
            {
                if (p != player || IsComplete || !countedTiles.Add(tile)) return;
                Advance(p, rods);
            }, "AddFu"));
        }

        private void Advance(Player player, int rods)
        {
            progress += rods;
            while (!IsComplete && progress >= Requirements[stage])
            {
                progress -= Requirements[stage];
                int completedStage = stage++;
                // Advance first: money can trigger Golden Seed and cause event reentrancy/rebinding.
                player.EarnMoney(MoneyRewards[completedStage]);
                MessageManager.Instance.OnArtifactEarnMoney(MoneyRewards[completedStage], this);
                if (completedStage == 1)
                {
                    Gadget reward = player.GenerateGadgets(1, g => g.IsConsumable(), false).FirstOrDefault();
                    player.RequestGadgetReward(reward, this);
                }
                else if (completedStage == 2)
                {
                    player.RequestFillEmptyGadgetSlots(this);
                }
            }
            if (IsComplete) progress = 0;
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player) =>
            IsComplete ? (null, 0) : ("{0}/" + Requirements[stage], progress);

        public override string GetDescription(Func<string, string> localizer) => GetDescription(null, localizer);
        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            if (IsComplete) return localizer("artifact_pinata_complete");
            string description = string.Format(CultureInfo.InvariantCulture, localizer("artifact_pinata_stage_" + stage),
                Requirements[stage] - progress,
                MechanicalArtifactUtility.GetFormattedPartName(MechPartType.DriveRod, localizer));
            return stage == 0
                ? string.Format(CultureInfo.InvariantCulture, localizer("artifact_tile_gift_description_format"),
                    GIFT_COUNT, description)
                : description;
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            stage = progress = 0;
            ClearPlay();
        }

        public override string Serialize() => stage + "," + progress;
        public override void Deserialize(string data)
        {
            stage = progress = 0;
            ClearPlay();
            string[] parts = (data ?? "").Split(',');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int savedStage) ||
                !int.TryParse(parts[1], out int savedProgress)) return;
            stage = Math.Max(0, Math.Min(Requirements.Length, savedStage));
            progress = IsComplete ? 0 : Math.Max(0, Math.Min(Requirements[stage] - 1, savedProgress));
        }
    }
}
