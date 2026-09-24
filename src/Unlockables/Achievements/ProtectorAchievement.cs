namespace Aotenjo
{
    public class ProtectorAchievement : Achievement
    {
        public ProtectorAchievement(string id) : base(id)
        {
        }
        
        [SubscribeToEvent]
        private void OnWonGame(RunStatusEvent.End eventData)
        {
            Player player = eventData.player;
            bool won = eventData.won;
            if (!won || player.Level > 16) return;
            if (player.stats.GetCustomStats("tile_destoryed") == 0)
            {
                SetComplete();
            }
        }
    }
}