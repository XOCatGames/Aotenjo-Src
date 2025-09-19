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

        public override Gadget Copy()
        {
            return new MagicBrushGadget(category).SetUses(uses);
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
        
        public override bool UseOnTiles(Player player, List<Tile> tiles)
        {
            if(!CanUseOnTiles(tiles)) return false;
            Block formedBlock = Block.FormValidBlock(tiles.ToArray(), player);
            return UseOnBlock(player, formedBlock);
        }

        public override int GetMaxOnUseNum()
        {
            return 4;
        }

        public override bool CanObtainBy(Player player, bool inShop)
        {
            return false;
        }
        
        public override bool CanUseOnTiles(List<Tile> tiles)
        {
            if (tiles.Count < 3 || tiles.Any(t => !t.IsNumbered()) || uses <= 0) return false;
            Player player = GameManager.Instance.player;
            Block formedBlock = Block.FormValidBlock(tiles.ToArray(), player);
            return formedBlock != null;
        }

        public override bool ShouldHighlightTile(Tile tile)
        {
            return tile.IsNumbered();
        }

        public override bool IsConsumable()
        {
            return true;
        }
    }
}