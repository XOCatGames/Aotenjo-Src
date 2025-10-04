using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class LuoyangShovelArtifact : Artifact
{

    private HashSet<Tile> affectedTiles = new HashSet<Tile>();
    
    public LuoyangShovelArtifact() : base("luoyang_shovel", Rarity.EPIC)
    {
    }

    public override void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
    {
        base.AppendOnUnusedTileEffects(player, perm, tile, effects);
        if (tile.properties.material is not TileMaterialPorcelain) return;
        if (!affectedTiles.Add(tile)) return;
        player.playHandEffectStack.Push(new SilentEffect(() => affectedTiles.Remove(tile)));
        player.playHandEffectStack.Push(new TileUnusedScoringEffectAppendEffect(player, tile, perm, player.playHandEffectStack));
        player.playHandEffectStack.Push(new TextEffect("effect_luoyang_shovel", this));
    }

    public override void ResetArtifactState(Player player)
    {
        base.ResetArtifactState(player);
        affectedTiles = new HashSet<Tile>();
    }

    public override void SubscribeToPlayer(Player player)
    {
        base.SubscribeToPlayer(player);
        player.PreRoundEndEvent += DestoryPorcelains;
    }
    
    public override void UnsubscribeToPlayer(Player player)
    {
        base.UnsubscribeToPlayer(player);
        player.PreRoundEndEvent -= DestoryPorcelains;
    }

    private void DestoryPorcelains(PlayerEvent playerEvent)
    {
        Player player = playerEvent.player;
        List<Tile> heldPorcelains = player.GetHandDeckCopy()
            .Where(t => t.properties.material is TileMaterialPorcelain)
            .ToList();
        heldPorcelains.ForEach(t => player.RemoveTileFromHand(t, destroyed:true));
        EventManager.Instance.OnRemoveTileEvent(heldPorcelains);
    }
}