using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialSecretColorPorcelain : TileMaterialPorcelain
    {
        [SerializeReference] private TileMaterial material;

        public TileMaterialSecretColorPorcelain(int ID) : base(ID, "secret_color_porcelain")
        {
            material = null;
        }

        private TileMaterialSecretColorPorcelain(int ID, TileMaterial mat) : this(ID)
        {
            material = mat;
        }

        public override TileMaterial Copy()
        {
            return new TileMaterialSecretColorPorcelain(spriteID, material);
        }

        public TileMaterial DrawNewMaterial(Player player)
        {
            LotteryPool<TileMaterial> pool = new LotteryPool<TileMaterial>();
            pool.AddRange(
                MaterialSet.Porcelain.GetMaterials()
                    .Where(m => !m.GetRegName().Equals(GetRegName()) && m.GetRarity() == Rarity.COMMON).ToList(), 4);
            pool.AddRange(MaterialSet.Porcelain.GetMaterials().Where(m => m.GetRarity() == Rarity.RARE).ToList());
            return pool.Draw(player.GenerateRandomInt);
        }

        public override Rarity GetRarity()
        {
            if (material != null) return material.GetRarity();
            return Rarity.COMMON;
        }


        private void ChangeMaterial(PlayerEvent data)
        {
            material = DrawNewMaterial(data.player);
        }

        public override void SubscribeToPlayerEvents(Player player)
        {
            base.SubscribeToPlayerEvents(player);
            EventBus.Subscribe<PlayerRoundEvent.Start.Pre>(ChangeMaterial);
            player.DetermineMaterialCompatibilityEvent += DetermineMaterial;
        }

        private void DetermineMaterial(PlayerDetermineMaterialCompatibilityEvent evt)
        {
            TileMaterial thisTileMaterial = evt.tile.properties.material;
            TileMaterial materialToCast = evt.mat;
            if (thisTileMaterial != this) return;
            if (materialToCast == material) return;
            if (material == null || materialToCast is TileMaterialSecretColorPorcelain) return;
            evt.res = evt.res || materialToCast.GetRegName() == material.GetRegName();
        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            EventBus.Unsubscribe<PlayerRoundEvent.Start.Pre>(ChangeMaterial);
            player.DetermineMaterialCompatibilityEvent -= DetermineMaterial;
        }

        public override int GetSpriteID(Player player)
        {
            if (material == null)
                return base.GetSpriteID(player);
            return material.GetSpriteID(player) + 50;
        }

        public override string GetLocalizeKey()
        {
            if (material == null) return base.GetLocalizeKey();
            return material.GetLocalizeKey();
        }

        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            if (material == null) return base.GetDescription(localizer, player);
            return string.Format(localizer("secret_color_porcelain_transformed_format"),
                material.GetDescription(localizer, player));
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            if (material != null)
            {
                material.AppendBonusEffects(player, perm, tile, effects);
            }
        }

        public override void AppendUnusedEffects(Player player, Permutation perm, List<Effect> effects)
        {
            if (material == null) return;
            material.AppendUnusedEffects(player, perm, effects);
        }

        public override void AppendToListOnTileUnusedEffect(Player player, Permutation perm, List<Effect> effects,
            Tile scoringTile, Tile onEffectTile)
        {
            if (material == null) return;
            material.AppendToListOnTileUnusedEffect(player, perm, effects, scoringTile, onEffectTile);
        }
    }
}