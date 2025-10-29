using System.Linq;
using Aotenjo;

public class MalachiteVaseArtifact : Artifact
{
    public MalachiteVaseArtifact() : base("malachite_vase", Rarity.RARE)
    {
    }

    public override void SubscribeToPlayer(Player player)
    {
        base.SubscribeToPlayer(player);
        player.DetermineMaterialCompatibilityEvent += SuperCopper;
    }

    public override void UnsubscribeToPlayer(Player player)
    {
        base.UnsubscribeToPlayer(player);
        player.DetermineMaterialCompatibilityEvent -= SuperCopper;
    }

    private void SuperCopper(PlayerDetermineMaterialCompatibilityEvent eventData)
    {
        Tile tile = eventData.tile;
        TileMaterial mat = eventData.mat;
        bool rawRes = eventData.res;
        if (rawRes) return;
        if (tile.properties.material.GetRegName().Equals(TileMaterial.COPPER.GetRegName()))
        {
            if (mat.GetRegName() == TileMaterial.PLAIN.GetRegName() || MaterialSet.Ore.GetMaterials().Any(t =>
                    t.GetRegName() != TileMaterial.Ore().GetRegName() && t.GetRegName().Equals(mat.GetRegName())))
            {
                eventData.res = true;
            }
        }
    }
}