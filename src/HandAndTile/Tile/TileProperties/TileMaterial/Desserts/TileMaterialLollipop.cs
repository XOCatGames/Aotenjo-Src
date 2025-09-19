using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialLollipop : TileMaterialDessert
    {
        private const int MAX_USES = 3;
        private const int TRANSFORM_THRESHOLD = 1; // 消耗1次后变化材质
        private readonly int materialID;
        
        [SerializeReference] private TileMaterial decoratedMaterial; // 装饰的内部材质

        public TileMaterialLollipop(int id): base(id, "dessert_lollipop", MAX_USES) 
        {
            materialID = id;
            decoratedMaterial = null;
        }

        private TileMaterialLollipop(int id, int remaining, int totalConsumed, TileMaterial decoratedMat = null): base(id, "dessert_lollipop", remaining, Rarity.COMMON) 
        {
            materialID = id;
            this.totalUsesConsumed = totalConsumed;
            decoratedMaterial = decoratedMat;
        }

        protected override int GetMaterialTransformThreshold()
        {
            return TRANSFORM_THRESHOLD;
        }

        public override TileMaterial Copy() =>
            new TileMaterialLollipop(materialID, usesLeft, totalUsesConsumed, decoratedMaterial?.Copy());

        public override void AppendToListRoundEndEffect(Player player, Permutation perm, List<IAnimationEffect> effects, Tile tile)
        {
            effects.Add(new SimpleEffect("effect_lollipop_round_end", null, _ => decoratedMaterial = null).OnTile(tile));
            base.AppendToListRoundEndEffect(player, perm, effects, tile);
        }

        // 随机抽取一个甜品材质作为装饰材质
        private TileMaterial DrawRandomDessert(Player player)
        {
            TileMaterial[] desserts = new[]
            {
                Butter(),
                ChocolateDessert(),
                MilleFeuille(),
                SugarCube(),
                Jelly(),
                IceCream()
            };

            return desserts[player.GenerateRandomInt(desserts.Length)];
        }
        
        protected override void AddDessertEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            // 如果还没有装饰材质，第一次选中时随机生成一个
            if (decoratedMaterial == null && player.Selecting(tile) && tile.properties.material == this)
            {
                var drawRandomDessert = DrawRandomDessert(player);
                effects.Add(new SimpleEffect("effect_lollipop_transform", null, p =>
                {
                    decoratedMaterial = drawRandomDessert;
                    if(decoratedMaterial is TileMaterialDessert dessert)
                        dessert.usesLeft = this.usesLeft;
                }));
                drawRandomDessert.AppendBonusEffects(player, perm, tile, effects);
            }
            
            // 如果有装饰材质，使用装饰材质的效果
            if (decoratedMaterial != null)
            {
                decoratedMaterial.AppendBonusEffects(player, perm, tile, effects);
            }
        }

        // 重写显示相关方法，使用装饰材质的显示信息
        public override string GetLocalizeKey()
        {
            if (decoratedMaterial == null) return base.GetLocalizeKey();
            return decoratedMaterial.GetLocalizeKey();
        }

        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            if (decoratedMaterial == null) return base.GetDescription(localizer, player);
            return string.Format(localizer("lollipop_decorated_format"), 
                decoratedMaterial.GetDescription(localizer, player));
        }

        public override int GetSpriteID(Player player)
        {
            if (decoratedMaterial == null) return base.GetSpriteID(player);
            // 使用装饰材质的sprite但保持棒棒糖的特征（可以加偏移量）
            return decoratedMaterial.GetSpriteID(player);
        }

        public override Rarity GetRarity()
        {
            if (decoratedMaterial == null) return base.GetRarity();
            return decoratedMaterial.GetRarity();
        }

        // 委托其他效果方法到装饰材质
        public override void AppendUnusedEffects(Player player, Permutation perm, List<Effect> effects)
        {
            if (decoratedMaterial == null) return;
            decoratedMaterial.AppendUnusedEffects(player, perm, effects);
        }

        public override void AppendToListOnTileUnusedEffect(Player player, Permutation perm, List<Effect> effects,
            Tile scoringTile, Tile onEffectTile)
        {
            if (decoratedMaterial == null) return;
            decoratedMaterial.AppendToListOnTileUnusedEffect(player, perm, effects, scoringTile, onEffectTile);
        }
    }
}
