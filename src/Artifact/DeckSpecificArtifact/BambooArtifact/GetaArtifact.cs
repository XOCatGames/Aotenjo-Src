using Aotenjo;

public class GetaArtifact : BambooArtifact
{
    public GetaArtifact() : base("geta", Rarity.COMMON)
    {
    }

    public override void SubscribeToPlayer(Player player)
    {
        base.SubscribeToPlayer(player);
        EventBus.Subscribe<PlayerRoundEvent.Skip.Pre>(OnSkipRound);
        
    }

    private static void OnSkipRound(PlayerEvent playerEvent)
    {
        BambooDeckPlayer bambooDeckPlayer = (BambooDeckPlayer)playerEvent.player;
        bambooDeckPlayer.RevealIndicator(1);
    }

    public override void UnsubscribeToPlayer(Player player)
    {
        base.UnsubscribeToPlayer(player);
        // 解绑与订阅保持相同的事件阶段，避免移除后仍执行效果。
        EventBus.Unsubscribe<PlayerRoundEvent.Skip.Pre>(OnSkipRound);
    }
}