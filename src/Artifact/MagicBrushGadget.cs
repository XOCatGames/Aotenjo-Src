using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    public class MagicBrushGadget : Gadget
    {
        [SerializeField]
        public Tile.Category category;

        public MagicBrushGadget(Tile.Category category) : base($"magic_brush_{category.ToString().ToLower()}", 33 + (int)category, 1, 10)
        {
            this.category = category;
        }

        protected override Gadget CreateCopy()
        {
            return new MagicBrushGadget(category);
        }

        public override int GetStackLimit()
        {
            return 5;
        }

        public override bool UseOnBlock(Player player, Block block)
        {
            if (uses == 0 || !block.IsNumbered()) return false;

            if (block.GetCategory() == category)
            {
                PaintBucketArtifact bucket = player.GetArtifacts().FirstOrDefault(a => a is PaintBucketArtifact) as PaintBucketArtifact;
                bucket?.ReceiveEmpower();
            }
            
            foreach (var item in block.tiles)
            {
                item.AddTransform(new TileTransformMiniBrushed(category, item.GetOrder()), player);
            }

            return true;
        }
        
        public override GadgetUseResult UseOnTiles(Player player, List<Tile> tiles)
        {
            if (tiles == null || tiles.Count == 0 || uses <= 0) return GadgetUseResult.Failed;
            if(!CanUseOnTiles(tiles, player)) return GadgetUseResult.Failed;
            Block formedBlock = Block.FormValidBlock(tiles.ToArray(), player);
            return GadgetUseResult.FromSuccess(UseOnBlock(player, formedBlock), tiles);
        }

        public override int GetMaxOnUseNum(Player player)
        {
            return 4;
        }

        public override bool CanObtainBy(Player player, bool inShop)
        {
            return false;
        }
        
        public override bool CanUseOnTiles(List<Tile> tiles, Player player)
        {
            if (tiles.Count < 3 || tiles.Any(t => !t.IsNumbered()) || uses <= 0) return false;
            Block formedBlock = Block.FormValidBlock(tiles.ToArray(), player);
            return formedBlock != null;
        }

        public override bool ShouldHighlightTile(Tile tile, Player player)
        {
            return tile.IsNumbered();
        }

        public override bool IsConsumable()
        {
            return true;
        }
    }
}