using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class YataMirrorArtifact : BambooArtifact
{
    public YataMirrorArtifact() : base("yata_mirror", Rarity.COMMON)
    {
    }

    public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
    {
        base.AddOnBlockEffects(player, permutation, block, effects);
        if (block.Any(t => !player.Selecting(t))) return;
        if (!permutation.blocks.Any(b => b.tiles[0] != block.tiles[0] && b.IsNumbered() && b.OfSameOrder(block)))
            return;
        BambooDeckPlayer bambooDeckPlayer = (BambooDeckPlayer)player;
        bambooDeckPlayer.RevealIndicator(1);
    }
}