using Aotenjo;

public class GetaArtifact : BambooArtifact
{
    public GetaArtifact() : base("geta", Rarity.COMMON)
    {
    }

    public override void SubscribeToPlayer(Player player)
    {
        base.SubscribeToPlayer(player);
        player.PostSkipRoundEvent += OnSkipRound;
    }

    private static void OnSkipRound(PlayerEvent playerEvent)
    {
        BambooDeckPlayer bambooDeckPlayer = (BambooDeckPlayer)playerEvent.player;
        bambooDeckPlayer.RevealIndicator(1);
    }

    public override void UnsubscribeToPlayer(Player player)
    {
        base.UnsubscribeToPlayer(player);
        player.PostSkipRoundEvent -= OnSkipRound;
    }
}